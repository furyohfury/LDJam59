using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public sealed class WorldMap : Singleton<WorldMap>
    {
        public List<GridTile> Tiles => _tilesMap.Values.ToList();
        public Vector2Int PlayerStartPosition { get => _playerStartPosition; set => _playerStartPosition = value; }
        public Vector2Int Size = new Vector2Int(5, 5);
        [SerializeField]
        private Tilemap _tilemap;
        [SerializeField]
        private Sprite _emptyTileSpriteWhite;
        [SerializeField]
        private Color _blackTileColor;
        private Vector2Int _playerStartPosition = Vector2Int.zero;
        private readonly Dictionary<Vector2Int, GridTile> _tilesMap = new Dictionary<Vector2Int, GridTile>();

        public void Init()
        {
            _tilemap.size = new Vector3Int(Size.x, Size.y, 1);

            for (int i = 0; i < Size.x; i++)
            {
                for (var j = 0; j < Size.y; j++)
                {
                    Tile runtimeTile = ScriptableObject.CreateInstance<Tile>();
                    runtimeTile.sprite = _emptyTileSpriteWhite;
                    Color color = (i + j) % 2 == 0
                        ? Color.white
                        : _blackTileColor;
                    runtimeTile.color = color;
                    var pos = new Vector2Int(i, j);
                    _tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), runtimeTile);
                    _tilesMap.Add(pos, new GridTile(pos)
                                       {
                                           Tile = runtimeTile
                                       });
                }
            }

            PlayerStartPosition = Size / 2;
        }

        public bool IsTileFree(Vector2Int position)
        {
            return _tilesMap.TryGetValue(position, out GridTile tile) && tile.Entity == null;
        }

        public bool IsTileFree(GridTile tile)
        {
            return IsTileFree(tile.Position);
        }

        public bool CanPlayerGoTo(Vector2Int position)
        {
            return _tilesMap.TryGetValue(position, out GridTile tile) && (tile.Entity == null || tile.Entity.TryGetComponent(out ChargePickup _));
        }

        public Vector2 GetTilePosition(Vector2Int position)
        {
            Vector3 worldPos = _tilemap.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));

            return new Vector2(worldPos.x, worldPos.y);
        }

        public GridTile GetTile(Vector2Int position)
        {
            return _tilesMap[position];
        }

        public void SwapEntityTile(Entity entity, Vector2Int position)
        {
            foreach (var tile in _tilesMap.Values)
            {
                if (tile.Entity == entity)
                {
                    tile.Entity = null;
                }
            }

            _tilesMap[position].Entity = entity;
        }
    }
}
