using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class GridTile
    {
        public readonly Vector2Int Position;
        public Entity Entity;
        public Tile Tile;
        public bool IsObstacle;
        public bool IsBlockObstacle;

        public GridTile(Vector2Int position)
        {
            Position = position;
        }

        public bool HasEntity()
        {
            return Entity != null;
        }
    }
}
