using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Extensions;
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
        public GridTile PlayerGridTile
        {
            get => _playerGridTile;
            private set => _playerGridTile = value;
        }
        [Header("Params")]
        public int Speed = 2;
        public SpeedCycle SpeedCycle;
        [SerializeField]
        private float _visualMoveSpeed = 1f;
        [SerializeField]
        private int _charge = 20;
        [SerializeField]
        private Ease _moveEase = Ease.Linear;
        [SerializeField]
        private float _moveOverShoot = 1.7058f;
        [SerializeField]
        private Color _damagedColor = Color.red;
        [SerializeField]
        private float _damagedAnimDuration = 0.3f;
        [SerializeField]
        private Ease _damagedAnimEase = Ease.OutQuint;

        [Header("Refs")]
        public Entity Entity;
        [SerializeField]
        private GameObject _possibleCellGlowPrefab;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _moveClip;
        [SerializeField]
        private AudioClip[] _takeDamageClips;
        [SerializeField]
        private AudioClip _deathClip;

        private GridTile _playerGridTile = new GridTile(Vector2Int.zero);
        private int _currentSpeedIndex;
        private readonly List<GameObject> _glows = new List<GameObject>();
        private static readonly int MoveLeft = Animator.StringToHash("Left");
        private static readonly int MoveRight = Animator.StringToHash("Right");
        private static readonly int MoveUp = Animator.StringToHash("Up");
        private static readonly int MoveDown = Animator.StringToHash("Down");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int DieKey = Animator.StringToHash("Die");
        private static readonly int DischargeKey = Animator.StringToHash("Discharge");
        private readonly Dictionary<Vector2Int, int> _animatorKeys = new Dictionary<Vector2Int, int>
                                                                     {
                                                                         {
                                                                             new Vector2Int(-1, 0), MoveLeft
                                                                         },
                                                                         {
                                                                             new Vector2Int(1, 0), MoveRight
                                                                         },
                                                                         {
                                                                             new Vector2Int(0, -1), MoveDown
                                                                         },
                                                                         {
                                                                             new Vector2Int(0, 1), MoveUp
                                                                         }
                                                                     };

        public void Init(GridTile tile)
        {
            Speed = SpeedCycle.Speeds[0];
            PlayerGridTile = tile;
            UpdatePossibleCellsGlow();
            Charge = GlobalSettingsProvider.Instance.Settings.PlayerInitialCharge;
        }

        public async Awaitable Move(Vector2Int direction)
        {
            Vector2Int initialTilePos = PlayerGridTile.Position;
            Vector2Int distance = direction * Speed;
            Vector2Int newTile = PlayerGridTile.Position + distance;

            if (!WorldMap.Instance.CanPlayerGoTo(_playerGridTile.Position, newTile))
                return;

            Vector2 tilePosition = WorldMap.Instance.GetTilePosition(newTile);
            PlayerGridTile = WorldMap.Instance.GetTileOrNull(newTile);
            DestroyExistingGlows();
            int animationKey = _animatorKeys[direction];
            _animator.SetTrigger(animationKey);
            float duration = Speed / _visualMoveSpeed;
            transform.DOMove(tilePosition, duration).SetEase(_moveEase, _moveOverShoot);
            _audioSource.clip = _moveClip;
            _audioSource.volume = VFXManager.Instance.Volume;
            _audioSource.Play();
            PlayerController.Instance.Disable();

            await Awaitable.WaitForSecondsAsync(duration);

            PlayerController.Instance.Enable();
            Charge--;
            ProcessIntermediateTilesEntities(direction, initialTilePos);
            ProcessEndTileEntities();
            Speed = SpeedCycle.Speeds[++_currentSpeedIndex % SpeedCycle.Speeds.Length];
            UpdatePossibleCellsGlow();

            if (Charge <= 0)
            {
                Discharge();
            }
            else
            {
                _animator.SetTrigger(Idle);
            }

            WorldMap.Instance.SwapEntityTile(Entity, newTile);
        }

        private void ProcessIntermediateTilesEntities(Vector2Int direction, Vector2Int initialTilePos)
        {
            if (Speed <= 1)
                return;

            for (var i = 1; i <= Speed - 1; i++)
            {
                GridTile intermediateTile = WorldMap.Instance.GetTileOrNull(initialTilePos + direction * i);

                if (intermediateTile.HasEntity())
                {
                    bool hasEnemy = intermediateTile.Entity.TryGetComponent(out Enemy enemy);
                    Debug.Log($"Checking enemies on  {intermediateTile.Position} : {hasEnemy}");
                    if (hasEnemy)
                    {
                        EnemySystem.Instance.KillEnemy(enemy);
                    }
                    else if (GlobalSettingsProvider.Instance.Settings.ConsumeChargePickupsOnTraverse
                             && intermediateTile.Entity.TryGetComponent(out ChargePickup chargePickup))
                    {
                        ChargePickupSystem.Instance.ConsumePickup(chargePickup);
                    }
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
                    EnemySystem.Instance.KillEnemy(enemy);
                }
            }
        }

        [Button]
        private void TakeDamage(int damage)
        {
            Charge -= damage;

            if (Charge <= 0)
            {
                Die();
            }
            else
            {
                _audioSource.clip = _takeDamageClips.GetRandom();
                _audioSource.volume = VFXManager.Instance.Volume;
                _audioSource.Play();
                DOTween.Sequence()
                       .Append(_spriteRenderer.DOColor(_damagedColor, _damagedAnimDuration).SetEase(_damagedAnimEase))
                       .Append(_spriteRenderer.DOColor(Color.white, _damagedAnimDuration).SetEase(_damagedAnimEase));
            }
        }

        private void Die()
        {
            PlayDeathSFX();
            _animator.SetTrigger(DieKey);
        }

        private void Discharge()
        {
            PlayDeathSFX();
            _animator.SetTrigger(DischargeKey);
        }

        private void PlayDeathSFX()
        {
            _audioSource.clip = _deathClip;
            _audioSource.volume = VFXManager.Instance.Volume;
            _audioSource.Play();
        }

        public void UpdatePossibleCellsGlow()
        {
            var directions = new Vector2Int[]
                             {
                                 new Vector2Int(0, 1), new Vector2Int(0, -1),
                                 new Vector2Int(1, 0), new Vector2Int(-1, 0)
                             };

            DestroyExistingGlows();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int tile = PlayerGridTile.Position + direction * Speed;
                if (WorldMap.Instance.CanPlayerGoTo(_playerGridTile.Position, tile))
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

        public void SetSpeedCycle(int[] playerSpeedCycle)
        {
            SpeedCycle.Speeds = playerSpeedCycle.Clone() as int[];
        }
    }
}
