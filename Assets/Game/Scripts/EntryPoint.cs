using UnityEngine;

namespace Game
{
    public sealed class EntryPoint : MonoBehaviour
    {
        public int InitialChargePickupsCount = 3;
        public Vector2Int InitialChargePickupsRandomValue = new Vector2Int(3, 5);
        public int MinimalSignalDistanceFromPlayer = 5;
        public Vector2Int MapSize = new Vector2Int(15, 15);
        public int EnemiesStartNumber = 2;

        private void Start()
        {
            WorldMap.Instance.Init(MapSize);
            PlayerSpawner.Instance.Spawn();
            Player.Instance.UpdatePossibleCellsGlow();
            PlayerDeathObserver.Instance.Init();
            PlayerChargeUI.Instance.Init();
            CameraPositioner.Instance.Init();
            
            GameRoundsSystem.Instance.Init();
            GameRoundsSystem.Instance.StartFirstWave();
        }
    }
}
