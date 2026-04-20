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
                PlayerController.Instance.Disable();
                GameOverUI.Instance.Show();
            }
        }

        private void OnDestroy()
        {
            Player.Instance.OnChargeChanged -= InstanceOnOnChargeChanged;
        }
    }
}
