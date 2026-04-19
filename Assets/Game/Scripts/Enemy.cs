using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Enemy : MonoBehaviour
    {
        public Entity Entity;
        public int Damage;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Vector2Int[] _damageRange;
        [SerializeField]
        private EnemyDamageRange _damageRangePrefab;
        private readonly Dictionary<GridTile, EnemyDamageRange> _damageRanges = new Dictionary<GridTile, EnemyDamageRange>();

        public void Init()
        {
            GridTile selfTile = WorldMap.Instance.GetEntityTile(Entity);
            Vector2Int selfPos = selfTile.Position;

            foreach (Vector2Int range in _damageRange)
            {
                GridTile damageRangeTile = WorldMap.Instance.GetTileOrNull(selfPos + range);

                if (damageRangeTile != null
                    && WorldMap.Instance.IsTileFree(damageRangeTile))
                {
                    EnemyDamageRange damageRange = Instantiate(_damageRangePrefab, WorldMap.Instance.GetTilePosition(damageRangeTile)
                        , Quaternion.identity
                        , transform);
                    damageRange.Enemy = this;
                    _damageRanges.Add(damageRangeTile, damageRange);
                    damageRangeTile.Entity = damageRange.Entity;
                }
            }
        }

        public async Awaitable Die()
        {
            _animator.SetTrigger("Die");
            float length = _animator.GetCurrentAnimatorStateInfo(0).length;

            await Awaitable.WaitForSecondsAsync(length);

            foreach (EnemyDamageRange damageRange in _damageRanges.Values)
            {
                WorldMap.Instance.RemoveEntity(damageRange.Entity);
                Destroy(damageRange.gameObject);
            }

            _damageRanges.Clear();
        }
    }
}
