using UnityEngine;

namespace Game
{
    public sealed class PlayerSpawner : Singleton<PlayerSpawner>
    {
        [SerializeField]
        private Player _prefab;
        [SerializeField]
        private Transform _container;

        public void Spawn()
        {
            Player player = Instantiate(_prefab, _container);
            Vector2Int startPosition = WorldMap.Instance.PlayerStartPosition;
            GridTile tile = WorldMap.Instance.GetTileOrNull(startPosition);
            tile.Entity = player.Entity;
            player.transform.position = WorldMap.Instance.GetTilePosition(startPosition);
            player.Init(tile);
        }
    }
}
