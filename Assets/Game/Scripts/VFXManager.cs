using UnityEngine;

namespace Game
{
    public sealed class VFXManager : Singleton<VFXManager>
    {
        [SerializeField]
        private GameObject _chargePickupVFX;
        [SerializeField]
        private GameObject _signalPickupVFX;

        public GameObject SpawnChargePickupVFX(Vector3 position)
        {
            return Instantiate(_chargePickupVFX, position, Quaternion.identity, transform);
        }

        public GameObject SpawnSignalPickupVFX(Vector3 position)
        {
            return Instantiate(_signalPickupVFX, position, Quaternion.identity, transform);
        }
    }
}
