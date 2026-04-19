using UnityEngine;

namespace Game
{
    public sealed class CameraPositioner : Singleton<CameraPositioner>
    {
        [SerializeField]
        private float _padding = 1f;

        public void Init()
        {
            FitCameraToGrid(WorldMap.Instance.Size, WorldMap.Instance.AnchorSize, _padding);
            // Vector2Int center = WorldMap.Instance.Size / 2;
            // Vector2 tilePosition = WorldMap.Instance.GetTilePosition(center);
            // transform.position = new Vector3(tilePosition.x, tilePosition.y, transform.position.z);
        }

        private void FitCameraToGrid(Vector2Int gridSize, Vector3 tileAnchor, float padding = 1.0f)
        {
            Camera cam = Camera.main;

            // 1. Получаем размер сетки (Grid)
            // Если клетки стандартные 1x1, то физический размер совпадает с gridSize
            float totalWidth = gridSize.x;
            float totalHeight = gridSize.y;

            // 2. Рассчитываем размер (Size) для ортографической камеры
            float sizeByHeight = totalHeight / 2f;
            float sizeByWidth = totalWidth / cam.aspect / 2f;

            // Выбираем максимальный размер, чтобы всё влезло по обеим осям
            cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth) + padding;

            // 3. Вычисляем центр с учетом якоря (tileAnchor)
            // Формула центра для сетки, начинающейся с (0,0):
            // ((Кол-во клеток - 1) / 2) + смещение якоря
            float centerX = (gridSize.x - 1) * 0.5f + tileAnchor.x;
            float centerY = (gridSize.y - 1) * 0.5f + tileAnchor.y;

            // Устанавливаем позицию камеры (Z обычно -10 для 2D)
            cam.transform.position = new Vector3(centerX, centerY, -10f);
        }
    }
}
