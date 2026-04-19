using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class PlayerChargeUI : Singleton<PlayerChargeUI>
    {
        [SerializeField]
        private TMP_Text _text;

        public void Init()
        {
            Player.Instance.OnChargeChanged += OnChargeChanged;
            OnChargeChanged(Player.Instance.Charge);
        }

        private void OnChargeChanged(int obj)
        {
            _text.text = obj.ToString();
        }

        private void OnDestroy()
        {
            Player.Instance.OnChargeChanged -= OnChargeChanged;
        }
    }
}
