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
            RoundData roundData = _rounds[Mathf.Min(_index++, _rounds.Length - 1)];
            EnemySystem.Instance.SpawnEnemies(roundData.EnemiesSpawnNumber);
            ChargePickupSystem.Instance.SpawnChargePickups(roundData.ChargePickupsCount, roundData.ChargePickupsValue);
            SignalSystem.Instance.SpawnSignal(roundData.MinimalSignalDistanceFromPlayer);
            Player.Instance.SetSpeedCycle(roundData.PlayerSpeedCycle);
            Player.Instance.Speed = Player.Instance.SpeedCycle.Speeds[0];
            Player.Instance.UpdatePossibleCellsGlow();
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
