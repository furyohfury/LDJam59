using TMPro;
using UnityEngine;

namespace Game
{
    public class PlayerScoreUI : Singleton<PlayerScoreUI>
    {
        [SerializeField]
        private TMP_Text _text;

        public void Init()
        {
            PlayerScore.Instance.OnScoreChanged += OnScoreChanged;
            OnScoreChanged(PlayerScore.Instance.Score);
        }

        private void OnScoreChanged(int obj)
        {
            _text.text = obj.ToString();
        }

        private void OnDestroy()
        {
            PlayerScore.Instance.OnScoreChanged -= OnScoreChanged;
        }
    }
}
