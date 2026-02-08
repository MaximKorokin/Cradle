using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class EquipmentStatsApplierModule : EntityModuleBase
    {
        private readonly StatsModule _stats;

        public EquipmentStatsApplierModule(StatsModule stats)
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
            if (e.Item != null)
            {
                var modifiers = ExtractModifiers(e.Item.Value);
                if (modifiers.Count > 0)
                {
                    _stats.AddModifiers(slotSource, modifiers);
                }
            }
        }

        private static List<StatModifier> ExtractModifiers(ItemStackSnapshot item)
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
