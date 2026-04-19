using UnityEngine;

namespace Game
{
    [CreateAssetMenu(
        fileName = nameof(RoundData),
        menuName = nameof(Game) + "/" + nameof(RoundData))]
    public class RoundData : ScriptableObject
    {
        public int ChargePickupsCount = 3;
        public Vector2Int ChargePickupsValue = new Vector2Int(3, 5);
        public int MinimalSignalDistanceFromPlayer = 5;
        public int EnemiesSpawnNumber = 2;
        public int[] PlayerSpeedCycle = new[]
                                        {
                                            1, 2
                                            , 3
                                        };
    }
}
