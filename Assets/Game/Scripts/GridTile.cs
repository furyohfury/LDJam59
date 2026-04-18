using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class GridTile
    {
        public readonly Vector2Int Position;
        public Entity Entity;
        public Tile Tile;

        public GridTile(Vector2Int position)
        {
            Position = position;
        }
    }
}
