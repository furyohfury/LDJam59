using System.Collections.Generic;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    public sealed class EnemySystem : Singleton<EnemySystem>
    {
        [SerializeField]
        private Enemy[] _enemiesPrefabs;
        [SerializeField]
        private int _startNumber = 2;
        private readonly HashSet<Enemy> _enemies = new HashSet<Enemy>();
        [SerializeField]
        private Transform _container;

        public void Init()
        {
            SpawnEnemies(_startNumber);
        }

        private void SpawnEnemies(float enemiesCount)
        {
            HashSet<GridTile> freeTiles = new HashSet<GridTile>();
            List<GridTile> tiles = WorldMap.Instance.Tiles;

            foreach (var tile in tiles)
            {
                if (WorldMap.Instance.IsTileFree(tile))
                {
                    freeTiles.Add(tile);
                }
            }

            for (var i = 0; i < Mathf.Min(enemiesCount, freeTiles.Count); i++)
            {
                GridTile randomTile = freeTiles.GetRandom();
                Enemy randomPrefab = _enemiesPrefabs[Random.Range(0, _enemiesPrefabs.Length)];
                Enemy enemy = Instantiate(randomPrefab, WorldMap.Instance.GetTilePosition(randomTile.Position), Quaternion.identity, _container);
                randomTile.Entity = enemy.Entity;
                _enemies.Add(enemy);
                freeTiles.Remove(randomTile);
            }
        }
    }
}
