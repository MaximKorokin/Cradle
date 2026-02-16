using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class EquipmentStatsApplierModule : EntityModuleBase
    {
        private readonly StatModule _stats;

        public EquipmentStatsApplierModule(StatModule stats)
        {
            _stats = stats;
        }

        protected override void OnAttach()
        {
            Subscribe<EquipmentChangedEvent>(OnEquipmentChanged);
        }

        private void OnEquipmentChanged(EquipmentChangedEvent e)
        {
            var slotSource = e.Slot; // source-id for modifiers from this slot

            // Always remove previous modifiers from this slot (covers unequip/replace/update)
            _stats.RemoveModifiers(slotSource);

            // Apply new modifiers if something is equipped now
            if (e.Kind != EquipmentChangeKind.Unequipped && e.Item != null)
            {
                var modifiers = ExtractStatModifiers(e.Item.Value);
                if (modifiers.Count > 0)
                {
                    _stats.AddModifiers(slotSource, modifiers);
                }
            }

            // Change weight
            if (e.Kind != EquipmentChangeKind.Unequipped && e.Item != null)
            {
                _stats.AddModifiers(slotSource, new StatModifier(StatId.CarryWeight, StatStage.Add, StatOperation.Add, e.Item.Value.Definition.Weight).Yield());
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
    }
}
