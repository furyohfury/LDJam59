using UnityEngine;

namespace Game
{
    public sealed class EntryPoint : MonoBehaviour
    {
        public Vector2Int MapSize = new Vector2Int(15, 15);
        public int StartRoundIndex = 0;

        private void Start()
        {
            WorldMap.Instance.Init(MapSize);
            PlayerSpawner.Instance.Spawn();
            Player.Instance.UpdatePossibleCellsGlow();
            PlayerDeathObserver.Instance.Init();
            PlayerChargeUI.Instance.Init();
            CameraPositioner.Instance.Init();

            GameRoundsSystem.Instance.Init();

            if (StartRoundIndex == 0)
            {
                GameRoundsSystem.Instance.StartFirstWave();
            }
            else
            {
                GameRoundsSystem.Instance.StartRound(StartRoundIndex);
            }
        }
    }
}
