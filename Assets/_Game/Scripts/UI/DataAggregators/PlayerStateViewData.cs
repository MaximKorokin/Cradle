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
            _playerContext.PlayerChanging += SubscribeToPlayerModules;
            _playerContext.PlayerChanged += SubscribeToPlayerModules;

            SubscribeToPlayerModules();
        }

        private void SubscribeToPlayerModules()
        {
            _playerContext.StatModule.Stats.StatChanged += OnStatChanged;
            _playerContext.StatusEffectModule.StatusEffects.Changed += OnStatusEffectsControllerChanged;
            _playerContext.LevelingModule.Changed += OnLevelingModuleChanged;
        }

        private void UnsubscribeFromPlayerModules()
        {
            _playerContext.StatModule.Stats.StatChanged -= OnStatChanged;
            _playerContext.StatusEffectModule.StatusEffects.Changed -= OnStatusEffectsControllerChanged;
            _playerContext.LevelingModule.Changed -= OnLevelingModuleChanged;
        }

        public float CurrentHp => _playerContext.StatModule.Stats.Get(StatId.HpCurrent);
        public float MaxHp => _playerContext.StatModule.Stats.Get(StatId.HpMax);
        public float Level => _playerContext.LevelingModule.Level;
        public float NormalizedExperience => _playerContext.LevelingModule.GetNormalizedExperience();

        public IEnumerable<StatusEffectSnapshot> Buffs =>
            _playerContext.StatusEffectModule.StatusEffects.GetStatusEffectsForCategory(StatusEffectCategory.Buff).Select(s => s.Snapshot);

        public IEnumerable<StatusEffectSnapshot> Debuffs =>
            _playerContext.StatusEffectModule.StatusEffects.GetStatusEffectsForCategory(StatusEffectCategory.Debuff).Select(s => s.Snapshot);

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

        private void OnLevelingModuleChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();

            _playerContext.PlayerChanged -= SubscribeToPlayerModules;
            _playerContext.PlayerChanging -= UnsubscribeFromPlayerModules;

            UnsubscribeFromPlayerModules();
        }
    }
}
