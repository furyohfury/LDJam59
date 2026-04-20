using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class TutorialButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TutorialUI _tutorialUI;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnCloseButton);
        }

        private void OnCloseButton()
        {
            VFXManager.Instance.PlayButtonClick();
            _tutorialUI.Show();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnCloseButton);
        }
    }
}
