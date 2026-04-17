using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Calculators;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class StatSystem : EntitySystemBase, ITickSystem
    {
        private readonly StatTickController _tickController;
        private readonly DerivedStatsCalculator _derivedStatsCalculator;
        private readonly LevelingConfig _levelingConfig;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(StatModule), typeof(HealthModule), typeof(RestrictionStateModule), }
            );

        public StatSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            DerivedStatsCalculator derivedStatsCalculator,
            StatsConfig statsConfig,
            LevelingConfig levelingConfig) : base(globalEventBus, repository)
        {
            _tickController = new(statsConfig);
            _derivedStatsCalculator = derivedStatsCalculator;
            _levelingConfig = levelingConfig;

            TrackEntityEvent<EquipmentChangedEvent>(OnEquipmentChanged);
            TrackEntityEvent<InventoryChangedEvent>(OnInventoryChanged);
            TrackEntityEvent<StatusEffectChangedEvent>(OnStatusEffectChanged);
            TrackEntityEvent<StatChangedEvent>(OnStatChanged);
            TrackEntityEvent<LevelChangedEvent>(OnLevelChanged);

            TrackGlobalEvent<DamageAppliedEvent>(OnDamageApplied);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(_tickController.Tick);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (!EntityQuery.Match(entity)) return;

            _derivedStatsCalculator.RecalculateDerivedStats(entity);

            // Apply stat modifiers from currently equipped items (if any)
            if (entity.TryGetModule<EquipmentModule>(out var equipmentModule))
            {
                foreach (var (slot, item) in equipmentModule.Equipment.Enumerate())
                {
                    if (item == null) continue;
                    var equipmentChangedEvent = new EquipmentChangedEvent(slot, item.Value, EquipmentChangeKind.Equipped);
                    OnEquipmentChanged(entity, equipmentChangedEvent);
                }
            }

            // Apply weight modifiers from items in inventory (if any)
            if (entity.TryGetModule<InventoryModule>(out var inventoryModule))
            {
                foreach (var (slot, item) in inventoryModule.Inventory.Enumerate())
                {
                    if (item == null) continue;
                    var inventoryChangedEvent = new InventoryChangedEvent(slot, item.Value, InventoryChangeKind.Added);
                    OnInventoryChanged(entity, inventoryChangedEvent);
                }
            }

            // Apply stat modifiers from active status effects (if any)
            if (entity.TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                foreach (var statusEffect in statusEffectModule.StatusEffects.GetStatusEffects())
                {
                    var statusEffectChangedEvent = new StatusEffectChangedEvent(new() { Kind = StatusEffectChangeKind.Added, StatusEffect = statusEffect });
                    OnStatusEffectChanged(entity, statusEffectChangedEvent);
                }
            }

            // Apply stat modifiers from current level
            if (entity.TryGetModule<LevelingModule>(out var levelingModule))
            {
                OnLevelChanged(entity, new LevelChangedEvent(levelingModule.Level, levelingModule.Level));
            }
        }

        private static void OnEquipmentChanged(Entity entity, EquipmentChangedEvent e)
        {
            var stats = entity.GetModule<StatModule>();

            var source = StatModifierSource.FromEquipmentSlot(e.Slot);

            // Always remove previous modifiers from this slot (covers unequip/replace/update)
            stats.RemoveModifiers(source);

            // Apply new modifiers if something is equipped now
            if (e.Kind != EquipmentChangeKind.Unequipped && e.Item != null)
            {
                var modifiers = ExtractStatModifiers(entity, e.Item.Value, ItemTrigger.OnEquipmentChange);
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

        private static List<StatModifier> ExtractStatModifiers(Entity entity, ItemStackSnapshot item, ItemTrigger trigger)
        {
            var result = new List<StatModifier>();

            var triggerContext = new ItemTriggerContext(entity, ItemTrigger.OnEquipmentChange, item);
            foreach (var trait in item.GetFunctionalTraits<StatModifiersTrait>(trigger))
            {
                if (trait.CanTrigger(triggerContext))
                    result.AddRange(trait.Modifiers);
            }

            return result;
        }

        private static void AddWeightModifierFromItem(ItemStackSnapshot item, StatModule stats, StatModifierSource source)
        {
            stats.AddModifiers(source, new StatModifier(StatId.CarryWeight, StatStage.Add, StatOperation.Add, item.Definition.Weight).Yield());
        }

        private void OnStatChanged(Entity entity, StatChangedEvent e)
        {
            _derivedStatsCalculator.RecalculateDerivedStats(entity, e);
        }

        private void OnDamageApplied(DamageAppliedEvent e)
        {
            var stateModule = e.Target.GetModule<RestrictionStateModule>();
            if (!stateModule.Has(RestrictionState.Dead) && e.Target.GetModule<HealthModule>().CurrentHealth <= 0)
            {
                stateModule.Add(RestrictionState.Dead);
                GlobalEventBus.Publish(new EntityDiedEvent(e.Target, e.Source));
            }
        }

        private static void OnInventoryChanged(Entity entity, InventoryChangedEvent e)
        {
            var stats = entity.GetModule<StatModule>();

            var source = StatModifierSource.FromInventorySlot(e.Slot.Index);

            stats.RemoveModifiers(source);

            if (e.Kind != InventoryChangeKind.Removed && e.Item != null)
            {
                AddWeightModifierFromItem(e.Item.Value, stats, source);
            }
        }

        private static void OnStatusEffectChanged(Entity entity, StatusEffectChangedEvent e)
        {
            var stats = entity.GetModule<StatModule>();

            var source = StatModifierSource.FromStatusEffect(e.StatusEffect.Definition.Id);

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

        private void OnLevelChanged(Entity entity, LevelChangedEvent levelChangedEvent)
        {
            var statModule = entity.GetModule<StatModule>();

            statModule.RemoveModifiers(StatModifierSource.Level);
            statModule.AddModifiers(
                StatModifierSource.Level,
                Enumerable.Repeat(0, levelChangedEvent.NewLevel).SelectMany(_ => _levelingConfig.StatModifiersOnLevelUp));
        }
    }

    public readonly struct EntityDiedEvent : IGlobalEvent
    {
        public readonly Entity Victim;
        public readonly Entity Killer;

        public EntityDiedEvent(Entity victim, Entity killer)
        {
            Victim = victim;
            Killer = killer;
        }
    }
}
