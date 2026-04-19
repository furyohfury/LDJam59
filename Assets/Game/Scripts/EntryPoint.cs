using UnityEngine;

namespace Game
{
    public sealed class EntryPoint : MonoBehaviour
    {
        public int InitialChargePickups = 3;
        public Vector2Int InitialChargePickupsBorders =  new Vector2Int(3, 5);

        private void Start()
        {
            WorldMap.Instance.Init();
            PlayerSpawner.Instance.Spawn();
            EnemySystem.Instance.Init();
            Player.Instance.UpdatePossibleCellsGlow();
            PlayerDeathObserver.Instance.Init();
            PlayerChargeUI.Instance.Init();
            ChargePickupSystem.Instance.SpawnChargePickups(InitialChargePickups, InitialChargePickupsBorders);
        }
    }
}
