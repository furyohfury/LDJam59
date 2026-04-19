using UnityEngine;

namespace Game
{
    public sealed class GameRoundsSystem : Singleton<GameRoundsSystem>
    {
        [SerializeField]
        private RoundData[] _rounds;
        private int _index = 0;

        public void Init()
        {
            SignalSystem.Instance.OnSignalConsumed += OnSignalConsumed;
        }

        public void StartFirstWave()
        {
            _index = 0;
            StartNextRound();
        }

        public void StartRound(int index)
        {
            _index = index;
            StartNextRound();
        }

        private void StartNextRound()
        {
            RoundData roundData = _rounds[_index++ % _rounds.Length];
            EnemySystem.Instance.SpawnEnemies(roundData.EnemiesSpawnNumber);
            ChargePickupSystem.Instance.SpawnChargePickups(roundData.ChargePickupsCount, roundData.ChargePickupsValue);
            SignalSystem.Instance.SpawnSignal(roundData.MinimalSignalDistanceFromPlayer);
            Player.Instance.UpdatePossibleCellsGlow();
            Player.Instance.SpeedCycle.Speeds = roundData.PlayerSpeedCycle;
        }

        private void OnSignalConsumed()
        {
            StartNextRound();
        }

        private void OnDestroy()
        {
            SignalSystem.Instance.OnSignalConsumed -= OnSignalConsumed;
        }
    }
}
