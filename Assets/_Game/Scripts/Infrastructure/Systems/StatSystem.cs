using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    internal class StatSystem : ReactiveEntitySystemBase
    {
        public StatSystem(EntityRepository repository) : base(repository)
        {
        }

        protected override bool Filter(Entity entity)
        {
            return entity.HasModule<StatModule>();
        }

        protected override void OnTrack(Entity entity)
        {
            entity.Subscribe<EquipmentChangedEvent>(OnEquipmentChanged);
            entity.Subscribe<InventoryChangedEvent>(OnInventoryChanged);
            entity.Subscribe<StatusEffectChangedEvent>(OnStatusEffectChanged);
            entity.Subscribe<StatChangedEvent>(OnStatChanged);
            RecalculateDerivedStats(entity);
        }

        protected override void OnUntrack(Entity entity)
        {
            entity.Unsubscribe<EquipmentChangedEvent>(OnEquipmentChanged);
            entity.Unsubscribe<InventoryChangedEvent>(OnInventoryChanged);
            entity.Unsubscribe<StatusEffectChangedEvent>(OnStatusEffectChanged);
            entity.Unsubscribe<StatChangedEvent>(OnStatChanged);
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

        private static void OnStatChanged(StatChangedEvent e)
        {
            // todo: list primary stats at least in an array
            if (e.StatId == StatId.Strength || e.StatId == StatId.Agility)
                RecalculateDerivedStats(e.Entity);
        }

        private static void RecalculateDerivedStats(Entity entity)
        {
            var stats = entity.GetModule<StatModule>();

            var source = StatModifierSource.Derived;

            stats.RemoveModifiers(source);

            var strength = stats.Stats.GetBase(StatId.Strength);
            var agility = stats.Stats.GetBase(StatId.Agility);

            // todo: list formulas at least in an array
            stats.AddModifiers(source, new[]
            {
                new StatModifier(StatId.PhysicalAttack, StatStage.PreAdd, StatOperation.Add, strength * 2f),
                new StatModifier(StatId.PhysicalAttackSpeed, StatStage.PreAdd, StatOperation.Add, agility * 0.01f),
            });
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
