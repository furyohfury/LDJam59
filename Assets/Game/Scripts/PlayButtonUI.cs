using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public sealed class PlayButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button _playButton;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayButtonPressed);
        }

        private void OnPlayButtonPressed()
        {
            VFXManager.Instance.PlayButtonClick();
            SceneManager.LoadScene("SampleScene");
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonPressed);
        }
    }
}
