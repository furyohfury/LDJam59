namespace Game
{
    public class PlayerScoreUI : Singleton<PlayerScoreUI>
    {
        public void Init()
        {
            PlayerScore.Instance.OnScoreChanged += OnScoreChanged;
        }

        private void OnScoreChanged(int obj)
        {
        }

        private void OnDestroy()
        {
            PlayerScore.Instance.OnScoreChanged -= OnScoreChanged;
        }
    }
}
