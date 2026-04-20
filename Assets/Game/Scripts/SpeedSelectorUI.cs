using DG.Tweening;
using TMPro;
using TriInspector;
using UnityEngine;

namespace Game
{
    public class SpeedSelectorUI : Singleton<SpeedSelectorUI>
    {
        [Header("Animation Settings")]
        [SerializeField]
        private float _duration = 0.35f;
        [SerializeField]
        private Ease _easeType = Ease.OutQuart;
        [SerializeField]
        private float _sideScale = 0.6f;
        [SerializeField]
        private float _sideAlpha = 0.4f;

        [Header("Anchors (Positions)")]
        [SerializeField]
        private RectTransform _hiddenLeft;
        [SerializeField]
        private RectTransform _visibleLeft;
        [SerializeField]
        private RectTransform _center;
        [SerializeField]
        private RectTransform _visibleRight;
        [SerializeField]
        private RectTransform _hiddenRight;

        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI[] _textElements; // Ровно 4 объекта

        private int[] _numbers;
        private int _currentIndex;
        private bool _isAnimating;

        [Button]
        private void DebugInit()
        {
            Init(new[]
                 {
                     1, 2,
                     3
                 });
        }

        public async Awaitable Init(int[] nums, int startIndex = 0)
        {
            if (_isAnimating)
            {
                await WaitForAnimationAsync();
            }

            _numbers = nums;
            _currentIndex = startIndex;
            _isAnimating = false;

            DOTween.Kill(this);
            SetupInitialPositions();
        }

        private async Awaitable WaitForAnimationAsync()
        {
            while (_isAnimating)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        private void SetupInitialPositions()
        {
            int prev = (_currentIndex - 1 + _numbers.Length) % _numbers.Length;
            int next = (_currentIndex + 1) % _numbers.Length;

            // Принудительно сбрасываем состояние всех элементов
            for (int i = 0; i < _textElements.Length; i++)
            {
                _textElements[i].gameObject.SetActive(true);
                _textElements[i].DOKill();
            }

            SetState(_textElements[0], _numbers[prev], _visibleLeft, _sideScale, _sideAlpha);
            SetState(_textElements[1], _numbers[_currentIndex], _center, 1f, 1f);
            SetState(_textElements[2], _numbers[next], _visibleRight, _sideScale, _sideAlpha);

            // Четвертый элемент — в резерв
            SetState(_textElements[3], 0, _hiddenRight, _sideScale, 0f);
        }

        [Button]
        public void NextValue()
        {
            if (_isAnimating || _numbers == null)
                return;
            _isAnimating = true;

            _currentIndex = (_currentIndex + 1) % _numbers.Length;

            TextMeshProUGUI shiftingElement = GetHiddenElement();
            int nextNext = (_currentIndex + 1) % _numbers.Length;

            // Подготовка "вплывающего" элемента
            SetState(shiftingElement, _numbers[nextNext], _hiddenRight, _sideScale, 0f);

            Sequence seq = DOTween.Sequence().SetTarget(this);

            foreach (var txt in _textElements)
            {
                // Используем вспомогательный метод сравнения позиций с допуском
                if (IsAtPosition(txt, _visibleLeft))
                    seq.Join(Animate(txt, _hiddenLeft, _sideScale, 0f));

                else if (IsAtPosition(txt, _center))
                    seq.Join(Animate(txt, _visibleLeft, _sideScale, _sideAlpha));

                else if (IsAtPosition(txt, _visibleRight))
                    seq.Join(Animate(txt, _center, 1f, 1f));

                else if (txt == shiftingElement)
                    seq.Join(Animate(txt, _visibleRight, _sideScale, _sideAlpha));
            }

            seq.OnComplete(() => _isAnimating = false);
        }

        [Button]
        public void PreviousValue()
        {
            if (_isAnimating || _numbers == null)
                return;
            _isAnimating = true;

            _currentIndex = (_currentIndex - 1 + _numbers.Length) % _numbers.Length;

            TextMeshProUGUI shiftingElement = GetHiddenElement();
            int prevPrev = (_currentIndex - 1 + _numbers.Length) % _numbers.Length;

            SetState(shiftingElement, _numbers[prevPrev], _hiddenLeft, _sideScale, 0f);

            Sequence seq = DOTween.Sequence().SetTarget(this);

            foreach (var txt in _textElements)
            {
                if (IsAtPosition(txt, _visibleRight))
                    seq.Join(Animate(txt, _hiddenRight, _sideScale, 0f));

                else if (IsAtPosition(txt, _center))
                    seq.Join(Animate(txt, _visibleRight, _sideScale, _sideAlpha));

                else if (IsAtPosition(txt, _visibleLeft))
                    seq.Join(Animate(txt, _center, 1f, 1f));

                else if (txt == shiftingElement)
                    seq.Join(Animate(txt, _visibleLeft, _sideScale, _sideAlpha));
            }

            seq.OnComplete(() => _isAnimating = false);
        }

        // Сравнение позиций с небольшим допуском (Epsilon)
        private bool IsAtPosition(TextMeshProUGUI txt, RectTransform target)
        {
            return Vector2.Distance(txt.rectTransform.anchoredPosition, target.anchoredPosition) < 0.5f;
        }

        private TextMeshProUGUI GetHiddenElement()
        {
            foreach (var txt in _textElements)
            {
                // Свободным считается тот, кто в одной из скрытых точек
                if (IsAtPosition(txt, _hiddenLeft)
                    || IsAtPosition(txt, _hiddenRight))
                    return txt;
            }
            // Если из-за микро-движений не нашли, берем самый прозрачный (запасной вариант)
            TextMeshProUGUI cleanest = _textElements[0];
            foreach (var txt in _textElements)
            {
                if (txt.alpha < cleanest.alpha)
                    cleanest = txt;
            }
            return cleanest;
        }

        private Tweener Animate(
            TextMeshProUGUI txt,
            RectTransform target,
            float scale,
            float alpha)
        {
            txt.rectTransform.DOKill(); // Останавливаем текущие анимации конкретного объекта
            txt.rectTransform.DOAnchorPos(target.anchoredPosition, _duration).SetEase(_easeType);
            txt.rectTransform.DOScale(Vector3.one * scale, _duration).SetEase(_easeType);
            return txt.DOFade(alpha, _duration).SetEase(_easeType);
        }

        private void SetState(
            TextMeshProUGUI txt,
            int val,
            RectTransform anchor,
            float scale,
            float alpha)
        {
            txt.DOKill();
            txt.text = val.ToString();
            txt.rectTransform.anchoredPosition = anchor.anchoredPosition;
            txt.rectTransform.localScale = Vector3.one * scale;
            txt.alpha = alpha;
        }
    }
}
