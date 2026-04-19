using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public sealed class WorldMap : Singleton<WorldMap>
    {
        public List<GridTile> Tiles => _tilesMap.Values.ToList();
        public Vector2Int PlayerStartPosition { get => _playerStartPosition; set => _playerStartPosition = value; }
        public Vector3 AnchorSize => _tilemap.tileAnchor;
        [Header("Params")]
        public Vector2Int Size = new Vector2Int(5, 5);
        [Tooltip("Число на которое делится количество клеток сетки чтобы получить количество одноклеточных препятствий")] [SerializeField]
        private int _singleSizedObstacleSizeFraction = 20;
        [Tooltip("Число на которое делится количество клеток сетки чтобы получить количество двухклеточных препятствий")] [SerializeField]
        private int _twoSizedObstacleSizeFraction = 30;
        [Header("References")]
        [SerializeField]
        private Tilemap _tilemap;
        [SerializeField]
        private Sprite _emptyTileSpriteWhite;
        [SerializeField]
        private Color _blackTileColor;
        [SerializeField]
        private Sprite _borderStraightSprite;
        [SerializeField]
        private Sprite[] _singleObstacleSprites;
        [SerializeField]
        private Sprite _twoSizedObstacleSpriteUpper;

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
            GenerateObstacles();
        }

        public bool IsTileFree(Vector2Int position)
        {
            return _tilesMap.TryGetValue(position, out GridTile tile) && tile.Entity == null && !tile.IsObstacle;
        }

        public bool IsTileFree(GridTile tile)
        {
            return IsTileFree(tile.Position);
        }

        public bool CanPlayerGoTo(Vector2Int position)
        {
            return _tilesMap.TryGetValue(position, out GridTile tile) && !tile.IsObstacle && (tile.Entity == null
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

        public void GenerateObstacles()
        {
            // 1. Вычисляем количество
            int totalCells = Size.x * Size.y;
            int singleCount = totalCells / _singleSizedObstacleSizeFraction;
            int doubleCount = totalCells / _twoSizedObstacleSizeFraction;
            Debug.Log($"Single obstacle count: {singleCount}, Double obstacle count: {doubleCount}");

            // 2. Создаем список всех игровых координат
            List<Vector2Int> freeCells = new List<Vector2Int>();
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    if (PlayerStartPosition != new Vector2Int(x, y))
                        freeCells.Add(new Vector2Int(x, y));
                }
            }

            // Перемешиваем список для рандомного спавна (Fisher-Yates shuffle)
            for (int i = 0; i < freeCells.Count; i++)
            {
                Vector2Int temp = freeCells[i];
                int randomIndex = Random.Range(i, freeCells.Count);
                freeCells[i] = freeCells[randomIndex];
                freeCells[randomIndex] = temp;
            }

            // 3. Спавним двухклеточные препятствия
            int spawnedDouble = 0;
// Используем while, так как индекс i будем контролировать вручную
            int ind = freeCells.Count - 1;

            while (ind >= 0
                   && spawnedDouble < doubleCount)
            {
                // На всякий случай проверяем, не стал ли i больше текущего размера после удаления соседа
                if (ind >= freeCells.Count)
                {
                    ind = freeCells.Count - 1;
                    continue;
                }

                Vector2Int pos = freeCells[ind];
                bool isVertical = Random.value > 0.5f;
                Vector2Int neighborPos = isVertical
                    ? pos + Vector2Int.up
                    : pos + Vector2Int.right;

                // Проверяем, что сосед в списке (значит он свободен и в границах)
                if (freeCells.Contains(neighborPos))
                {
                    PlaceDoubleObstacle(pos, neighborPos, isVertical);

                    // Удаляем сначала соседа, потом текущий элемент
                    freeCells.Remove(neighborPos);
                    freeCells.RemoveAt(freeCells.IndexOf(pos));

                    spawnedDouble++;
                    // После удаления двух элементов пересчитываем индекс i
                    ind = freeCells.Count - 1;
                }
                else
                {
                    // Если соседа нет, просто переходим к следующей клетке
                    ind--;
                }
            }

            // 4. Спавним одноклеточные препятствия
            int spawnedSingle = 0;

            for (int i = freeCells.Count - 1; i >= 0 && spawnedSingle < singleCount; i--)
            {
                Vector2Int pos = freeCells[i];
                PlaceSingleObstacle(pos);

                freeCells.RemoveAt(i);
                spawnedSingle++;
            }
        }

        private void PlaceSingleObstacle(Vector2Int pos)
        {
            // 1. Создаем НОВЫЙ тайл препятствия
            Tile obstacleTile = ScriptableObject.CreateInstance<Tile>();
            obstacleTile.sprite = _singleObstacleSprites.GetRandom();

            // 2. Рассчитываем цвет для этого НОВОГО тайла
            obstacleTile.color = (pos.x + pos.y) % 2 == 0
                ? Color.white
                : _blackTileColor;

            Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);

            // 3. Кладем его на карту
            _tilemap.SetTile(tilePos, obstacleTile);

            // 4. Обновляем данные в словаре
            if (_tilesMap.ContainsKey(pos))
            {
                GridTile gridTile = _tilesMap[pos];
                gridTile.IsObstacle = true;

                // ВАЖНО: Обновляем ссылку в словаре, чтобы она указывала на текущий тайл на карте
                gridTile.Tile = obstacleTile;
            }
        }

        private void PlaceDoubleObstacle(Vector2Int basePos, Vector2Int secondPos, bool isVertical)
        {
            // 1. Создаем экземпляры
            Tile lowerTile = ScriptableObject.CreateInstance<Tile>();
            Tile upperTile = ScriptableObject.CreateInstance<Tile>();

            lowerTile.sprite = _twoSizedObstacleSpriteUpper;
            upperTile.sprite = _twoSizedObstacleSpriteUpper;

            // 2. Рассчитываем и назначаем цвета СРАЗУ тайлам
            lowerTile.color = (basePos.x + basePos.y) % 2 == 0
                ? Color.white
                : _blackTileColor;
            upperTile.color = (secondPos.x + secondPos.y) % 2 == 0
                ? Color.white
                : _blackTileColor;

            Vector3Int p1 = new Vector3Int(basePos.x, basePos.y, 0);
            Vector3Int p2 = new Vector3Int(secondPos.x, secondPos.y, 0);

            // 3. Устанавливаем на Tilemap
            _tilemap.SetTile(p1, lowerTile);
            _tilemap.SetTile(p2, upperTile);

            // 4. Логика поворотов (исправленная для стыковки основаниями)
            if (isVertical)
            {
                ApplyRotation(p1, 180f); // Нижняя часть (перевернута)
                ApplyRotation(p2, 0f); // Верхняя часть (оригинал)
            }
            else
            {
                // Для горизонтального: левая часть смотрит влево, правая вправо
                // Исходим из того, что 0 градусов — это "верх"
                ApplyRotation(p1, 90f); // Левая часть (поворот влево)
                ApplyRotation(p2, 270f); // Правая часть (поворот вправо)
            }

            // 5. Обновляем словарь (Данные + Ссылки на новые тайлы)
            if (_tilesMap.ContainsKey(basePos))
            {
                _tilesMap[basePos].IsObstacle = true;
                _tilesMap[basePos].Tile = lowerTile; // Обновляем ссылку!
            }

            if (_tilesMap.ContainsKey(secondPos))
            {
                _tilesMap[secondPos].IsObstacle = true;
                _tilesMap[secondPos].Tile = upperTile; // Обновляем ссылку!
            }
        }

// Вспомогательный метод для чистоты кода
        private void ApplyRotation(Vector3Int position, float angle)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);
            _tilemap.SetTransformMatrix(position, matrix);
        }
    }
}
