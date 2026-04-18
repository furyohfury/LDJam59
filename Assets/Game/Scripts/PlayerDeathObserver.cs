using UnityEngine;

namespace Game
{
    public sealed class PlayerDeathObserver : Singleton<PlayerDeathObserver>
    {
        public void Init()
        {
            Player.Instance.OnChargeChanged += InstanceOnOnChargeChanged;
        }

        private void InstanceOnOnChargeChanged(int obj)
        {
            if (obj <= 0)
            {
                Debug.Log("player dead");
                // TODO logic
            }
        }

        private void OnDestroy()
        {
            Player.Instance.OnChargeChanged -= InstanceOnOnChargeChanged;
        }
    }
}
