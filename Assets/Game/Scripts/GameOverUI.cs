using DG.Tweening;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public sealed class GameOverUI : Singleton<GameOverUI>
    {
        [SerializeField]
        private Image _bg;
        [SerializeField]
        private Button _retryButton;
        [SerializeField]
        private Button _quitButton;
        [SerializeField]
        private float _showAnimDuration;
        [SerializeField]
        private Ease _showAnimEase = Ease.Linear;

        private void OnEnable()
        {
            _retryButton.onClick.AddListener(OnRetryButton);
            _quitButton.onClick.AddListener(OnQuitButton);
        }

        private void Start()
        {
            gameObject.SetActive(false);
            Hide();
        }

        [Button]
        public void Show()
        {
            gameObject.SetActive(true);
            _bg.DOFade(1, _showAnimDuration).SetEase(_showAnimEase);
            _retryButton.image.DOFade(1, _showAnimDuration).SetEase(_showAnimEase);
            _quitButton.image.DOFade(1, _showAnimDuration).SetEase(_showAnimEase);
        }

        [Button]
        public void Hide()
        {
            DOTween.Sequence()
                   .Append(_bg.DOFade(0, _showAnimDuration).SetEase(_showAnimEase))
                   .Join(_retryButton.image.DOFade(0, _showAnimDuration).SetEase(_showAnimEase))
                   .Join(_quitButton.image.DOFade(0, _showAnimDuration).SetEase(_showAnimEase))
                   .AppendCallback(() => gameObject.SetActive(false));
        }

        private void OnRetryButton()
        {
            VFXManager.Instance.PlayButtonClick();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnQuitButton()
        {
            VFXManager.Instance.PlayButtonClick();
            SceneManager.LoadScene("MenuScene");
        }

        private void OnDisable()
        {
            _retryButton.onClick.RemoveListener(OnRetryButton);
            _quitButton.onClick.RemoveListener(OnQuitButton);
        }
    }
}
