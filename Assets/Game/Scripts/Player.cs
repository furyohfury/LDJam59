using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class Player : Singleton<Player>
    {
        public event Action<int> OnChargeChanged;

        public int Charge
        {
            get => _charge;
            set
            {
                _charge = Mathf.Max(0, value);
                OnChargeChanged?.Invoke(_charge);
            }
        }
        public int Speed = 2;
        public Entity Entity;
        [SerializeField]
        private SpeedCycle _speedCycle;
        [SerializeField]
        private GameObject _possibleCellGlowPrefab;
        [SerializeField]
        private int _charge = 10;
        private GridTile _playerGridTile = new GridTile(Vector2Int.zero);
        private int _currentSpeedIndex;
        private readonly List<GameObject> _glows = new List<GameObject>();

        public void Init(GridTile tile)
        {
            Speed = _speedCycle.Speeds[0];
            _playerGridTile = tile;
            UpdatePossibleCellsGlow();
        }

        public void Move(Vector2Int direction)
        {
            direction *= Speed;
            Vector2Int newTile = _playerGridTile.Position + direction;

            if (!WorldMap.Instance.CanPlayerGoTo(newTile))
                return;

            Vector2 tilePosition = WorldMap.Instance.GetTilePosition(newTile);
            transform.position = tilePosition;
            Speed = _speedCycle.Speeds[++_currentSpeedIndex % _speedCycle.Speeds.Length];
            _playerGridTile = WorldMap.Instance.GetTile(newTile);
            UpdatePossibleCellsGlow();
            Charge--;

            if (_playerGridTile.HasEntity()
                && _playerGridTile.Entity.TryGetComponent(out ChargePickup chargePickup))
            {
                ChargePickupSystem.Instance.ConsumePickup(chargePickup);
            }

            WorldMap.Instance.SwapEntityTile(Entity, newTile);
        }

        public void UpdatePossibleCellsGlow()
        {
            var directions = new Vector2Int[]
                             {
                                 new Vector2Int(0, 1), new Vector2Int(0, -1)
                                 , new Vector2Int(1, 0), new Vector2Int(-1, 0)
                             };

            for (int i = 0; i < _glows.Count; i++)
            {
                GameObject glow = _glows[i];
                Destroy(glow);
            }

            _glows.Clear();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int tile = _playerGridTile.Position + direction * Speed;
                if (WorldMap.Instance.CanPlayerGoTo(tile))
                {
                    GameObject glow = Instantiate(_possibleCellGlowPrefab, WorldMap.Instance.GetTilePosition(tile), Quaternion.identity, transform);
                    _glows.Add(glow);
                }
            }
        }
    }
}
