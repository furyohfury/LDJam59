using System.Collections.Generic;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    public sealed class ChargePickupSystem : Singleton<ChargePickupSystem>
    {
        [SerializeField]
        private ChargePickup[] _prefabs;
        [SerializeField]
        private Transform _container;
        private readonly HashSet<ChargePickup> _chargePickups = new HashSet<ChargePickup>();

        public void SpawnChargePickups(int amount, Vector2Int chargePickupsBorders)
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

            for (var i = 0; i < Mathf.Min(amount, freeTiles.Count); i++)
            {
                GridTile randomTile = freeTiles.GetRandom();
                ChargePickup randomPrefab = _prefabs[Random.Range(0, _prefabs.Length)];
                ChargePickup pickup = Instantiate(randomPrefab, WorldMap.Instance.GetTilePosition(randomTile.Position), Quaternion.identity
                    , _container);
                int chargeAmount = Random.Range(chargePickupsBorders.x, chargePickupsBorders.y);
                pickup.SetChargeValue(chargeAmount);
                randomTile.Entity = pickup.Entity;
                _chargePickups.Add(pickup);
                freeTiles.Remove(randomTile);
            }
        }

        public void ConsumePickup(ChargePickup chargePickup)
        {
            Player.Instance.Charge += chargePickup.Charge;
            _chargePickups.Remove(chargePickup);
            Destroy(chargePickup.gameObject);
            // TODO sfx, vfx
        }
    }
}
