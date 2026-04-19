using UnityEngine;

namespace Game
{
    public sealed class GameRoundsSystem : MonoBehaviour
    {
        public void Init()
        {
            SignalSystem.Instance.OnSignalConsumed += OnSignalConsumed;
        }

        private void OnSignalConsumed()
        {
        }

        private void OnDestroy()
        {
            SignalSystem.Instance.OnSignalConsumed -= OnSignalConsumed;
        }
    }
}
