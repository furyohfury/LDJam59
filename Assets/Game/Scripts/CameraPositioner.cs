using UnityEngine;

namespace Game
{
    public sealed class CameraPositioner : Singleton<CameraPositioner>
    {
        public void Init()
        {
            Vector2Int center = WorldMap.Instance.Size / 2;
            Vector2 tilePosition = WorldMap.Instance.GetTilePosition(center);
            transform.position = new Vector3(tilePosition.x, tilePosition.y, transform.position.z);
        }
    }
}
