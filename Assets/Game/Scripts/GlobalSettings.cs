using UnityEngine;

namespace Game
{
    [CreateAssetMenu(
        fileName = nameof(GlobalSettings),
        menuName = nameof(Game) + "/" + nameof(GlobalSettings))]
    public class GlobalSettings : ScriptableObject
    {
        public Vector2Int MapSize = new Vector2Int(19, 11);
        [Tooltip("Число на которое делится количество клеток сетки чтобы получить количество одноклеточных препятствий")] [SerializeField]
        public int _singleSizedObstacleSizeFraction = 20;
        [Tooltip("Число на которое делится количество клеток сетки чтобы получить количество двухклеточных препятствий")] [SerializeField]
        public int _twoSizedObstacleSizeFraction = 30;
        public int KillEnemyScoreBounty = 100;
        public int PickUpSignalScoreBounty = 1000;
        public int PlayerInitialCharge = 20;
        public int KillEnemyChargeBonus = 5;
        public int SignalPickupChargeBonus = 20;
    }
}
