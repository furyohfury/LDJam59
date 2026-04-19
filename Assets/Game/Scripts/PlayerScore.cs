using System;

namespace Game
{
    public sealed class PlayerScore : Singleton<PlayerScore>
    {
        public event Action<int> OnScoreChanged;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnScoreChanged?.Invoke(_score);
            }
        }

        private int _score;

        public void CountEnemyKill()
        {
            Score += GlobalSettingsProvider.Instance.Settings.KillEnemyScoreBounty;
        }

        public void CountPickUpSignalBounty()
        {
            Score += GlobalSettingsProvider.Instance.Settings.PickUpSignalScoreBounty;
        }
    }
}
