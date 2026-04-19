using UnityEngine;

namespace Game
{
    public sealed class EntryPoint : MonoBehaviour
    {
        public int StartRoundIndex = 0;

        private void Start()
        {
            WorldMap.Instance.Init(GlobalSettingsProvider.Instance.Settings.MapSize);
            PlayerSpawner.Instance.Spawn();
            Player.Instance.UpdatePossibleCellsGlow();
            PlayerDeathObserver.Instance.Init();
            PlayerChargeUI.Instance.Init();
            CameraPositioner.Instance.Init();
            PlayerScoreUI.Instance.Init();

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
