using System;
using System.Collections.Generic;
using DG.Tweening;
using TriInspector;
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
        public GridTile PlayerGridTile { get => _playerGridTile; private set => _playerGridTile = value; }
        public int Speed = 2;
        public Entity Entity;
        [SerializeField]
        private SpeedCycle _speedCycle;
        [SerializeField]
        private GameObject _possibleCellGlowPrefab;
        [SerializeField]
        private int _charge = 10;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Ease _moveEase = Ease.Linear;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Color _damagedColor = Color.red;
        [SerializeField]
        private float _damagedAnimDuration = 0.3f;
        [SerializeField]
        private Ease _damagedAnimEase = Ease.OutQuint;

        private GridTile _playerGridTile = new GridTile(Vector2Int.zero);
        private int _currentSpeedIndex;
        private readonly List<GameObject> _glows = new List<GameObject>();

        public void Init(GridTile tile)
        {
            Speed = _speedCycle.Speeds[0];
            PlayerGridTile = tile;
            UpdatePossibleCellsGlow();
        }

        public async Awaitable Move(Vector2Int direction)
        {
            Vector2Int initialTilePos = PlayerGridTile.Position;
            Vector2Int distance = direction * Speed;
            Vector2Int newTile = PlayerGridTile.Position + distance;

            if (!WorldMap.Instance.CanPlayerGoTo(newTile))
                return;

            Vector2 tilePosition = WorldMap.Instance.GetTilePosition(newTile);
            Speed = _speedCycle.Speeds[++_currentSpeedIndex % _speedCycle.Speeds.Length];
            PlayerGridTile = WorldMap.Instance.GetTileOrNull(newTile);
            DestroyExistingGlows();
            _animator.SetTrigger("MoveLeft");
            Charge--;
            float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
            transform.DOMove(tilePosition, duration).SetEase(_moveEase);

            await Awaitable.WaitForSecondsAsync(duration);

            UpdatePossibleCellsGlow();
            ProcessIntermediateTilesEntities(direction, distance, initialTilePos);
            ProcessEndTileEntities();
            WorldMap.Instance.SwapEntityTile(Entity, newTile);
        }

        private void ProcessIntermediateTilesEntities(Vector2Int direction, Vector2Int distance, Vector2Int initialTilePos)
        {
            if (Speed <= 1)
                return;

            for (var i = 1; i < distance.magnitude; i++)
            {
                GridTile intermediateTile = WorldMap.Instance.GetTileOrNull(initialTilePos + direction * i);

                if (intermediateTile.HasEntity()
                    && intermediateTile.Entity.TryGetComponent(out Enemy enemy))
                {
                    enemy.Die();
                }
            }
        }

        private void ProcessEndTileEntities()
        {
            if (PlayerGridTile.HasEntity())
            {
                if (PlayerGridTile.Entity.TryGetComponent(out ChargePickup chargePickup))
                {
                    ChargePickupSystem.Instance.ConsumePickup(chargePickup);
                }
                else if (PlayerGridTile.Entity.TryGetComponent(out Signal signal))
                {
                    SignalSystem.Instance.ConsumeSignal(signal);
                }
                else if (PlayerGridTile.Entity.TryGetComponent(out EnemyDamageRange enemyDamageRange))
                {
                    int damage = enemyDamageRange.Enemy.Damage;
                    TakeDamage(damage);
                }
                else if (PlayerGridTile.Entity.TryGetComponent(out Enemy enemy))
                {
                    int damage = enemy.Damage;
                    TakeDamage(damage);
                    enemy.Die();
                }
            }
        }

        [Button]
        private void TakeDamage(int damage)
        {
            Charge -= damage;
            DOTween.Sequence()
                   .Append(_spriteRenderer.DOColor(_damagedColor, _damagedAnimDuration)
                                          .SetEase(_damagedAnimEase))
                   .Append(_spriteRenderer.DOColor(Color.white, _damagedAnimDuration)
                                          .SetEase(_damagedAnimEase));
        }

        public void UpdatePossibleCellsGlow()
        {
            var directions = new Vector2Int[]
                             {
                                 new Vector2Int(0, 1), new Vector2Int(0, -1)
                                 , new Vector2Int(1, 0), new Vector2Int(-1, 0)
                             };

            DestroyExistingGlows();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int tile = PlayerGridTile.Position + direction * Speed;
                if (WorldMap.Instance.CanPlayerGoTo(tile))
                {
                    GameObject glow = Instantiate(_possibleCellGlowPrefab, WorldMap.Instance.GetTilePosition(tile), Quaternion.identity, transform);
                    _glows.Add(glow);
                }
            }
        }

        private void DestroyExistingGlows()
        {
            for (int i = 0; i < _glows.Count; i++)
            {
                GameObject glow = _glows[i];
                Destroy(glow);
            }

            _glows.Clear();
        }
    }
}
