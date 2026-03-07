using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class PlayerStateViewData : DataAggregatorBase
    {
        private readonly PlayerContext _playerContext;

        public event Action Changed;

        public PlayerStateViewData(PlayerContext playerContext)
        {
            _playerContext = playerContext;
            _playerContext.PlayerChanging += OnPlayerChanged;
            _playerContext.PlayerChanged += OnPlayerChanged;

            OnPlayerChanged();
        }

        private void OnPlayerChanging()
        {
            _playerContext.StatModule.Stats.StatChanged -= OnStatChanged;
            _playerContext.StatusEffectModule.StatusEffectsController.Changed -= OnStatusEffectsControllerChanged;
        }

        private void OnPlayerChanged()
        {
            _playerContext.StatModule.Stats.StatChanged += OnStatChanged;
            _playerContext.StatusEffectModule.StatusEffectsController.Changed += OnStatusEffectsControllerChanged;
        }

        public float CurrentHp => _playerContext.StatModule.Stats.Get(StatId.HpCurrent);
        public float MaxHp => _playerContext.StatModule.Stats.Get(StatId.HpMax);
        public float Level => _playerContext.StatModule.Stats.Get(StatId.Level);
        public float Experience => _playerContext.StatModule.Stats.Get(StatId.Experience);

        public IEnumerable<StatusEffectSnapshot> Buffs =>
            _playerContext.StatusEffectModule.StatusEffectsController.GetStatusEffectsForCategory(StatusEffectCategory.Buff).Select(s => s.Snapshot);

        public IEnumerable<StatusEffectSnapshot> Debuffs =>
            _playerContext.StatusEffectModule.StatusEffectsController.GetStatusEffectsForCategory(StatusEffectCategory.Debuff).Select(s => s.Snapshot);

        private void OnStatChanged(StatId statId)
        {
            if (statId == StatId.HpCurrent || statId == StatId.HpMax)
            {
                Changed?.Invoke();
            }
        }

        private void OnStatusEffectsControllerChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();

            _playerContext.PlayerChanged -= OnPlayerChanged;
            _playerContext.PlayerChanging -= OnPlayerChanging;

            OnPlayerChanging();
        }
    }
}
