using UnityEngine;

namespace Game
{
    public sealed class EntryPoint : MonoBehaviour
    {
        public int InitialChargePickups = 3;
        public Vector2Int InitialChargePickupsBorders = new Vector2Int(3, 5);
        public int SignalDistanceFromPlayer = 3;
        public Vector2Int MapSize = new Vector2Int(15, 15);
        public int EnemiesStartNumber = 2;

        private void Start()
        {
            WorldMap.Instance.Init(MapSize);
            PlayerSpawner.Instance.Spawn();
            EnemySystem.Instance.Init(EnemiesStartNumber);
            Player.Instance.UpdatePossibleCellsGlow();
            PlayerDeathObserver.Instance.Init();
            PlayerChargeUI.Instance.Init();
            ChargePickupSystem.Instance.SpawnChargePickups(InitialChargePickups, InitialChargePickupsBorders);
            SignalSystem.Instance.SpawnSignal(SignalDistanceFromPlayer);
        }
    }
}
