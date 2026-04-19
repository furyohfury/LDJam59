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
        [Header("Params")]
        public Vector2Int Size = new Vector2Int(5, 5);
        public Vector3 AnchorSize => _tilemap.tileAnchor;
        [Header("References")]
        [SerializeField]
        private Tilemap _tilemap;
        [SerializeField]
        private Sprite _emptyTileSpriteWhite;
        [SerializeField]
        private Color _blackTileColor;
        [SerializeField]
        private Sprite _borderStraightSprite;
        private Vector2Int _playerStartPosition = Vector2Int.zero;
        private readonly Dictionary<Vector2Int, GridTile> _tilesMap = new Dictionary<Vector2Int, GridTile>();

        public void Init(Vector2Int size)
        {
            Size = size;
            _tilemap.size = new Vector3Int(Size.x + 2, Size.y + 2, 1);

            for (int i = -1; i <= Size.x; i++)
            {
                for (int j = -1; j <= Size.y; j++)
                {
                    var pos = new Vector3Int(i, j, 0);
                    Tile runtimeTile = ScriptableObject.CreateInstance<Tile>();

                    float rotation = 0f;
                    Sprite selectedSprite = null;

                    bool isLeft = i == -1;
                    bool isRight = i == Size.x;
                    bool isBottom = j == -1;
                    bool isTop = j == Size.y;

                    // 1. ПРОВЕРКА УГЛОВ (Исходник — ВЕРХНИЙ ПРАВЫЙ)
                    if (isRight && isTop)
                    {
                        selectedSprite = _emptyTileSpriteWhite;
                        rotation = 0;
                        runtimeTile.color = Color.black;
                    } // Пр-Верх
                    else if (isLeft && isTop)
                    {
                        selectedSprite = _emptyTileSpriteWhite;
                        rotation = 90;
                        runtimeTile.color = Color.black;
                    } // Лев-Верх
                    else if (isLeft && isBottom)
                    {
                        selectedSprite = _emptyTileSpriteWhite;
                        rotation = 180;
                        runtimeTile.color = Color.black;
                    } // Лев-Ниж
                    else if (isRight && isBottom)
                    {
                        selectedSprite = _emptyTileSpriteWhite;
                        rotation = 270;
                        runtimeTile.color = Color.black;
                    } // Пр-Ниж

                    // 2. ПРОВЕРКА СТЕН (Исходник — ВЕРХНЯЯ ГРАНИЦА)
                    else if (isTop)
                    {
                        selectedSprite = _borderStraightSprite;
                        rotation = 0;
                    } // Верх
                    else if (isLeft)
                    {
                        selectedSprite = _borderStraightSprite;
                        rotation = 90;
                    } // Лево
                    else if (isBottom)
                    {
                        selectedSprite = _borderStraightSprite;
                        rotation = 180;
                    } // Низ
                    else if (isRight)
                    {
                        selectedSprite = _borderStraightSprite;
                        rotation = 270;
                    } // Право

                    // 3. НАСТРОЙКА И УСТАНОВКА
                    if (selectedSprite != null)
                    {
                        runtimeTile.sprite = selectedSprite;
                    }
                    else
                    {
                        runtimeTile.sprite = _emptyTileSpriteWhite;
                        runtimeTile.color = (i + j) % 2 == 0
                            ? Color.white
                            : _blackTileColor;

                        _tilesMap.Add(new Vector2Int(i, j), new GridTile(new Vector2Int(i, j))
                                                            {
                                                                Tile = runtimeTile
                                                            });
                    }

                    _tilemap.SetTile(pos, runtimeTile);

                    if (rotation != 0)
                    {
                        // TRS: Translation (Zero), Rotation (Euler), Scale (One)
                        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotation), Vector3.one);
                        _tilemap.SetTransformMatrix(pos, matrix);
                    }
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
            return _tilesMap.TryGetValue(position, out GridTile tile) && (tile.Entity == null
                                                                          || tile.Entity.TryGetComponent(out ChargePickup _)
                                                                          || tile.Entity.TryGetComponent(out Signal _)
                                                                          || tile.Entity.TryGetComponent(out Enemy _)
                                                                          || tile.Entity.TryGetComponent(out EnemyDamageRange _));
        }

        public Vector2 GetTilePosition(Vector2Int position)
        {
            Vector3 worldPos = _tilemap.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));

            return new Vector2(worldPos.x, worldPos.y);
        }

        public Vector2 GetTilePosition(GridTile tile)
        {
            return GetTilePosition(tile.Position);
        }

        public GridTile GetTileOrNull(Vector2Int position)
        {
            return _tilesMap.GetValueOrDefault(position);
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

        public void RemoveEntity(Entity entity)
        {
            foreach (var tile in _tilesMap.Values)
            {
                if (tile.Entity == entity)
                {
                    tile.Entity = null;
                }
            }
        }

        public HashSet<GridTile> GetFreeTiles()
        {
            HashSet<GridTile> freeTiles = new HashSet<GridTile>();
            List<GridTile> tiles = Tiles;

            foreach (var tile in tiles)
            {
                if (IsTileFree(tile))
                {
                    freeTiles.Add(tile);
                }
            }

            return freeTiles;
        }

        public GridTile GetEntityTile(Entity entity)
        {
            foreach (GridTile tile in _tilesMap.Values)
            {
                if (tile.Entity == entity)
                {
                    return tile;
                }
            }

            return null;
        }
    }
}
