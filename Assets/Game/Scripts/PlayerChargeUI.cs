using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class PlayerChargeUI : Singleton<PlayerChargeUI>
    {
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private float _punchScaleAnim = 1.1f;
        [SerializeField]
        private float _punchAnimDuration = 0.1f;
        private Tweener _tween;

        public void Init()
        {
            Player.Instance.OnChargeChanged += OnChargeChanged;
            OnChargeChanged(Player.Instance.Charge);
        }

        private void OnChargeChanged(int obj)
        {
            _text.text = obj.ToString();
            _tween?.Kill(true);
            _tween = _text.transform.DOPunchScale(Vector3.one * _punchScaleAnim, _punchAnimDuration);
        }

        private void OnDestroy()
        {
            Player.Instance.OnChargeChanged -= OnChargeChanged;
        }
    }
}
