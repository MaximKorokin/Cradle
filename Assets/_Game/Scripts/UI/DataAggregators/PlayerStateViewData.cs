using Assets._Game.Scripts.Entities.Modules;
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
        private readonly StatModule _statsModule;
        private readonly StatusEffectModule _statusEffectModule;

        public event Action Changed;

        public PlayerStateViewData(PlayerContext playerContext)
        {
            _statsModule = playerContext.StatsModule;
            _statusEffectModule = playerContext.StatusEffectModule;

            _statsModule.Stats.StatChanged += OnStatChanged;
            _statusEffectModule.StatusEffectsController.Changed += OnStatusEffectsControllerChanged;
        }

        public float CurrentHp => _statsModule.Stats.Get(StatId.HpCurrent);
        public float MaxHp => _statsModule.Stats.Get(StatId.HpMax);
        public float Level => _statsModule.Stats.Get(StatId.Level);
        public float Experience => _statsModule.Stats.Get(StatId.Experience);

        public IEnumerable<StatusEffectSnapshot> Buffs => 
            _statusEffectModule.StatusEffectsController.GetStatusEffectsForCategory(StatusEffectCategory.Buff).Select(s => s.Snapshot);

        public IEnumerable<StatusEffectSnapshot> Debuffs => 
            _statusEffectModule.StatusEffectsController.GetStatusEffectsForCategory(StatusEffectCategory.Debuff).Select(s => s.Snapshot);

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

            _statsModule.Stats.StatChanged -= OnStatChanged;
            _statusEffectModule.StatusEffectsController.Changed -= OnStatusEffectsControllerChanged;
        }
    }
}
