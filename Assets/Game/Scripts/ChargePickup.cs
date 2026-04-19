using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class ChargePickup : MonoBehaviour
    {
        public int Charge;
        public Entity Entity;
        [SerializeField]
        private TextMeshProUGUI _text;

        public void SetChargeValue(int value)
        {
            Charge = value;
            _text.text = value.ToString();
        }
    }
}
