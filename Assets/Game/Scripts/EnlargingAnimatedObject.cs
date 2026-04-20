using DG.Tweening;
using UnityEngine;

namespace Game
{
    public sealed class EnlargingAnimatedObject : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _maxScale = new Vector3(1.2f, 1.2f, 1);
        [SerializeField]
        private float _duration = 1f;
        [SerializeField]
        private Ease _easeType = Ease.Linear;

        private void Start()
        {
            StartPulse();
        }

        private void StartPulse()
        {
            transform.DOScale(_maxScale, _duration)
                     .SetEase(_easeType)
                     .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
