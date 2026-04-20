using System;
using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    public sealed class SignalSystem : Singleton<SignalSystem>
    {
        public event Action OnSignalConsumed;

        [SerializeField]
        private Signal _prefab;
        [SerializeField]
        private Transform _container;
        private Signal _activeSignal;

        public void SpawnSignal(int minPlayerDistance)
        {
            if (_activeSignal != null)
            {
                Destroy(_activeSignal.gameObject);
                _activeSignal = null;
            }

            HashSet<GridTile> freeTiles = WorldMap.Instance.GetFreeTiles()
                                                  .Where(tile => Vector2Int.Distance(tile.Position, Player.Instance.PlayerGridTile.Position)
                                                                 >= minPlayerDistance)
                                                  .ToHashSet();

            GridTile randomTile = freeTiles.GetRandom();
            Vector2 tilePosition = WorldMap.Instance.GetTilePosition(randomTile);
            _activeSignal = Instantiate(_prefab, tilePosition, Quaternion.identity, _container);
            randomTile.Entity = _activeSignal.Entity;
        }

        public void ConsumeSignal(Signal signal)
        {
            if (_activeSignal != signal)
            {
                Debug.LogError("Different signal from stored");
                return;
            }

            WorldMap.Instance.RemoveEntity(signal.Entity);
            VFXManager.Instance.SpawnSignalPickupVFX(_activeSignal.transform.position);
            Destroy(signal.gameObject);
            _activeSignal = null;
            PlayerScore.Instance.CountPickUpSignalBounty();
            Player.Instance.Charge += GlobalSettingsProvider.Instance.Settings.SignalPickupChargeBonus;

            OnSignalConsumed?.Invoke();
        }
    }
}
