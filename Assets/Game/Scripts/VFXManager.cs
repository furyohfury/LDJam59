using TriInspector;
using UnityEngine;

namespace Game
{
    public sealed class VFXManager : Singleton<VFXManager>
    {
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                _bgMusicSource.volume = _volume * _bgMusicVolumeMult;
                _uiAudioSource.volume = _volume;
            }
        }
        [SerializeField]
        private GameObject _chargePickupVFX;
        [SerializeField]
        private GameObject _signalPickupVFX;
        [SerializeField]
        private AudioSource _bgMusicSource;
        [SerializeField]
        private AudioSource _uiAudioSource;
        [SerializeField]
        private AudioClip _uiClickSound;
        [SerializeField]
        [Range(0, 1f)]
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnVolumeChanged))]
#endif
        private float _volume = 1f;
        [SerializeField]
        private float _bgMusicVolumeMult = 0.25f;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            _bgMusicSource.volume = _volume * _bgMusicVolumeMult;
            _uiAudioSource.volume = _volume;
        }

        public GameObject SpawnChargePickupVFX(Vector3 position)
        {
            return Instantiate(_chargePickupVFX, position, Quaternion.identity, transform);
        }

        public GameObject SpawnSignalPickupVFX(Vector3 position)
        {
            return Instantiate(_signalPickupVFX, position, Quaternion.identity, transform);
        }

        public void PlayButtonClick()
        {
            _uiAudioSource.PlayOneShot(_uiClickSound);
        }

#if UNITY_EDITOR
        private void OnVolumeChanged()
        {
            Volume = _volume;
        }
#endif
    }
}
