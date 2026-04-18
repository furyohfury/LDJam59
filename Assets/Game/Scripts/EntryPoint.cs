using UnityEngine;

namespace Game
{
    public sealed class EntryPoint : MonoBehaviour
    {
        private void Start()
        {
            WorldMap.Instance.Init();
            PlayerSpawner.Instance.Spawn();
            EnemySystem.Instance.Init();
            Player.Instance.UpdatePossibleCellsGlow();
            PlayerDeathObserver.Instance.Init();
        }
    }
}
