using System.Collections.Generic;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    public sealed class EnemySystem : Singleton<EnemySystem>
    {
        [SerializeField]
        private Enemy[] _enemiesPrefabs;
        private readonly HashSet<Enemy> _enemies = new HashSet<Enemy>();
        [SerializeField]
        private Transform _container;

        public void Init(int enemiesCount)
        {
            SpawnEnemies(enemiesCount);
        }

        private void SpawnEnemies(float enemiesCount)
        {
            HashSet<GridTile> freeTiles = WorldMap.Instance.GetFreeTiles();

            for (var i = 0; i < Mathf.Min(enemiesCount, freeTiles.Count); i++)
            {
                GridTile randomTile = freeTiles.GetRandom();
                Enemy randomPrefab = _enemiesPrefabs[Random.Range(0, _enemiesPrefabs.Length)];
                Enemy enemy = Instantiate(randomPrefab, WorldMap.Instance.GetTilePosition(randomTile.Position), Quaternion.identity, _container);
                randomTile.Entity = enemy.Entity;
                _enemies.Add(enemy);
                freeTiles.Remove(randomTile);
                enemy.Init();
            }
        }

        public void KillEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
            WorldMap.Instance.RemoveEntity(enemy.Entity);
            Destroy(enemy.gameObject);
        }
    }
}
