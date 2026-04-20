using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class TutorialUI : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;
        private float _enlargeAnimDuration;
        [SerializeField]
        private Ease _openAnimEase = Ease.OutBounce;
        [SerializeField]
        private Ease _closeAnimEase = Ease.InBounce;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseButton);
        }

        public void Show()
        {
            if (gameObject.activeSelf)
            {
                return;
            }

            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            _enlargeAnimDuration = 0.5f;
            transform.DOScale(Vector3.one, _enlargeAnimDuration)
                     .SetEase(_openAnimEase);
        }

        private void OnCloseButton()
        {
            DOTween.Sequence()
                   .Append(transform.DOScale(Vector3.zero, _enlargeAnimDuration)
                                    .SetEase(_closeAnimEase))
                   .AppendCallback(() => gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseButton);
        }
    }
}
