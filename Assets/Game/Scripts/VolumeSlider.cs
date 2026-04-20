using UnityEngine;
using UnityEngine.UI;

namespace Game.Game
{
    [DefaultExecutionOrder(10)]
    public sealed class VolumeSlider : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Button _iconButton;
        [Header("Icons")]
        [SerializeField]
        private Sprite _zeroVolumeIcon;
        [SerializeField]
        private Sprite _nonZeroVolumeIcon;

        private float _volume;
        private float _cachedButtonValue;

        private void Start()
        {
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
            _iconButton.onClick.AddListener(OnIconButtonPressed);
            _volume = VFXManager.Instance.Volume;
            _cachedButtonValue = _volume;
            _slider.value = _volume;
            OnSliderValueChanged(_volume);
        }

        private void OnIconButtonPressed()
        {
            if (_volume != 0)
            {
                _cachedButtonValue = _volume;
                _slider.value = 0;
            }
            else
            {
                _slider.value = _cachedButtonValue;
            }
        }

        private void OnSliderValueChanged(float val)
        {
            if (_volume == 0
                && val != 0)
            {
                _icon.sprite = _nonZeroVolumeIcon;
            }
            else if (val == 0)
            {
                _icon.sprite = _zeroVolumeIcon;
            }

            _volume = val;
            VFXManager.Instance.Volume = val;
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}
