using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Calculators;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    internal class StatSystem : EntitySystemBase, ITickSystem
    {
        private readonly StatTickController _tickController;
        private readonly DerivedStatsCalculator _derivedStatsCalculator;
        private readonly IGlobalEventBus _globalEventBus;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(StatModule), typeof(RestrictionStateModule), }
            );

        public StatSystem(
            EntityRepository repository,
            DerivedStatsCalculator derivedStatsCalculator,
            StatsConfig statsConfig,
            IGlobalEventBus globalEventBus) : base(repository)
        {
            _tickController = new(statsConfig);
            _derivedStatsCalculator = derivedStatsCalculator;
            _globalEventBus = globalEventBus;

            TrackEntityEvent<EquipmentChangedEvent>(OnEquipmentChanged);
            TrackEntityEvent<InventoryChangedEvent>(OnInventoryChanged);
            TrackEntityEvent<StatusEffectChangedEvent>(OnStatusEffectChanged);
            TrackEntityEvent<StatChangedEvent>(OnStatChanged);
            TrackEntityEvent<DamageAppliedEvent>(OnDamageApplied);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(_tickController.Tick);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (EntityQuery.Match(entity))
            {
                _derivedStatsCalculator.RecalculateDerivedStats(entity);
            }
        }

        private static void OnEquipmentChanged(EquipmentChangedEvent e)
        {
            var stats = e.Entity.GetModule<StatModule>();

            var source = StatModifierSource.FromEquipmentSlot(e.Slot);

            // Always remove previous modifiers from this slot (covers unequip/replace/update)
            stats.RemoveModifiers(source);

            // Apply new modifiers if something is equipped now
            if (e.Kind != EquipmentChangeKind.Unequipped && e.Item != null)
            {
                var modifiers = ExtractStatModifiers(e.Item.Value);
                if (modifiers.Count > 0)
                {
                    stats.AddModifiers(source, modifiers);
                }
            }

            // Change weight
            if (e.Kind != EquipmentChangeKind.Unequipped && e.Item != null)
            {
                AddWeightModifierFromItem(e.Item.Value, stats, source);
            }
        }

        private static List<StatModifier> ExtractStatModifiers(ItemStackSnapshot item)
        {
            var result = new List<StatModifier>();

            foreach (var trait in item.GetTraits<StatModifiersTrait>())
            {
                result.AddRange(trait.Modifiers);
            }

            return result;
        }

        private static void AddWeightModifierFromItem(ItemStackSnapshot item, StatModule stats, StatModifierSource source)
        {
            stats.AddModifiers(source, new StatModifier(StatId.CarryWeight, StatStage.Add, StatOperation.Add, item.Definition.Weight).Yield());
        }

        private void OnStatChanged(StatChangedEvent e)
        {
            _derivedStatsCalculator.RecalculateDerivedStats(e);

            // sync the walk speed stat to the animator
            if (e.StatId == StatId.MoveSpeed && e.Entity.TryGetModule<AppearanceModule>(out var appearanceModule))
            {
                var stats = e.Entity.GetModule<StatModule>();
                appearanceModule.RequestSetAnimatorValue(EntityAnimatorParameterName.WalkSpeedMultiplier, stats.Stats.Get(StatId.MoveSpeed));
            }
        }

        private void OnDamageApplied(DamageAppliedEvent e)
        {
            var stateModule = e.Entity.GetModule<RestrictionStateModule>();
            if (!stateModule.Has(RestrictionState.Dead) && e.Entity.GetModule<StatModule>().Stats.Get(StatId.HpCurrent) <= 0)
            {
                stateModule.Add(RestrictionState.Dead);
                _globalEventBus.Publish<EntityDiedEvent>(new(e.Entity, e.Source));
            }
        }

        private static void OnInventoryChanged(InventoryChangedEvent e)
        {
            var stats = e.Entity.GetModule<StatModule>();

            var source = StatModifierSource.FromInventorySlot(e.Slot);

            stats.RemoveModifiers(source);

            if (e.Kind != InventoryChangeKind.Removed && e.Item != null)
            {
                AddWeightModifierFromItem(e.Item.Value, stats, source);
            }
        }

        private static void OnStatusEffectChanged(StatusEffectChangedEvent e)
        {
            var stats = e.Entity.GetModule<StatModule>();

            var source = StatModifierSource.FromStatusEffect(e.StatusEffect.Definition.Id);

            if (stats == null) return;
            foreach (var modifier in e.StatusEffect.Definition.StatModifiers)
            {
                if (e.Kind == StatusEffectChangeKind.Added)
                {
                    stats.AddModifiers(source, new[] { modifier });
                }
                else if (e.Kind == StatusEffectChangeKind.Removed)
                {
                    stats.RemoveModifiers(source);
                }
            }
        }
    }
}
