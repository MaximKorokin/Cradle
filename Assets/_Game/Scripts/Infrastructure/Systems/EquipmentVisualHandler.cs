using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    /// <summary>
    /// Handles equipment visual rules (Default, Mirror, Hide) for entity appearance.
    /// Supports multiple EquippableVisualTrait traits per item.
    /// </summary>
    public sealed class EquipmentVisualHandler
    {
        /// <summary>
        /// Refreshes equipment visuals for all slots affected by an equipment change.
        /// </summary>
        public void RefreshEquipmentVisuals(Entity entity)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule) ||
                !entity.TryGetModule<AppearanceModule>(out var appearance))
            {
                SLog.Error($"Entity {entity.Definition} is missing required modules for equipment visuals.");
                return;
            }

            var slotDisplays = CollectAffectedSlotsWithDisplay(equipmentModule);

            foreach (var (slotType, displayInfo) in slotDisplays)
            {
                ApplySlotVisuals(entity, slotType, displayInfo, appearance);
            }
        }

        /// <summary>
        /// Collects all slots that need to be refreshed along with their display information.
        /// This is done in a single pass through all equipment, using priority-based updates.
        /// </summary>
        private Dictionary<EquipmentSlotType, SlotDisplayInfo> CollectAffectedSlotsWithDisplay(
            EquipmentModule equipmentModule)
        {
            var slotDisplays = new Dictionary<EquipmentSlotType, SlotDisplayInfo>();

            // Process the changed item and all currently equipped items in a single pass
            foreach (var (slot, item) in equipmentModule.Equipment.Enumerate())
            {
                if (item == null)
                {
                    TrySetSlotDisplay(slotDisplays, slot.SlotType, new SlotDisplayInfo(default, slot), VisualRulePriority.Default);
                    continue;
                }

                // Default rule: item shows in its own slot
                TrySetSlotDisplay(slotDisplays, slot.SlotType, new SlotDisplayInfo(item.Value, slot), VisualRulePriority.Default);

                // Process all visual traits of this item
                var visualTraits = item.Value.Definition.GetTraits<EquippableVisualTrait>();
                foreach (var visualTrait in visualTraits)
                {
                    if (visualTrait?.Slots == null)
                        continue;

                    var displayInfo = new SlotDisplayInfo(item.Value, slot);
                    var priority = GetPriority(visualTrait.Rule);

                    foreach (var affectedSlot in visualTrait.Slots)
                    {
                        TrySetSlotDisplay(slotDisplays, affectedSlot, displayInfo, priority);
                    }
                }
            }

            return slotDisplays;
        }

        /// <summary>
        /// Attempts to set display info for a slot, respecting priority rules.
        /// Higher priority rules (Hide > Mirror > Default) override lower priority ones.
        /// </summary>
        private void TrySetSlotDisplay(
            Dictionary<EquipmentSlotType, SlotDisplayInfo> slotDisplays,
            EquipmentSlotType slotType,
            SlotDisplayInfo newDisplayInfo,
            VisualRulePriority newPriority)
        {
            // If slot doesn't exist, add it
            if (!slotDisplays.TryGetValue(slotType, out var existingDisplayInfo))
            {
                slotDisplays[slotType] = newDisplayInfo.WithPriority(newPriority);
                return;
            }

            // If new priority is higher, replace
            if (newPriority > existingDisplayInfo.Priority)
            {
                slotDisplays[slotType] = newDisplayInfo.WithPriority(newPriority);
            }
        }

        /// <summary>
        /// Gets the priority level for a visual rule.
        /// </summary>
        private VisualRulePriority GetPriority(EquipVisualRule rule)
        {
            return rule switch
            {
                EquipVisualRule.Hide => VisualRulePriority.Hide,
                EquipVisualRule.Mirror => VisualRulePriority.Mirror,
                EquipVisualRule.Default => VisualRulePriority.Default,
                _ => VisualRulePriority.Default
            };
        }

        /// <summary>
        /// Applies the determined display information to the slot's visual units.
        /// </summary>
        private void ApplySlotVisuals(
            Entity entity,
            EquipmentSlotType slotType,
            SlotDisplayInfo displayInfo,
            AppearanceModule appearance)
        {
            foreach (var entityUnitVisualModel in entity.Definition.VisualModel.Units.Where(u => u.EquipmentSlots.Contains(slotType)))
            {
                var path = $"{entityUnitVisualModel.Path}/{slotType}";

                if (displayInfo.HasDisplay)
                {
                    var relativeOrderInLayer = GetItemOrderInLayer(displayInfo.Item, displayInfo.Slot, slotType);
                    appearance.RequestEnsureUnit(path, relativeOrderInLayer);
                    appearance.RequestSetUnitSprite(path, displayInfo.Item.Definition.Sprite);
                }
                else
                {
                    appearance.RequestRemoveUnit(path);
                }
            }
        }

        /// <summary>
        /// Gets the order in layer for an item based on its traits.
        /// </summary>
        private int GetItemOrderInLayer(ItemStackSnapshot item, EquipmentSlotKey slot, EquipmentSlotType slotType)
        {
            OrderInLayerOverrideKind kind;
            if (item.Definition.TryGetTrait<OrderInLayerOverrideTrait>(out var trait))
            {
                kind = trait.Kind;
            }
            else
            {
                kind = slotType == EquipmentSlotType.Weapon ? OrderInLayerOverrideKind.Weapon : OrderInLayerOverrideKind.Clothing;
            }

            return kind switch
            {
                OrderInLayerOverrideKind.Weapon => -1,
                OrderInLayerOverrideKind.Clothing => 1,
                OrderInLayerOverrideKind.Overlay => 2,
                _ => 0
            };
        }

        #region Helper Types
        /// <summary>
        /// Priority levels for visual rules. Higher values override lower values.
        /// </summary>
        private enum VisualRulePriority
        {
            Default = 0,
            Mirror = 10,
            Hide = 20
        }

        /// <summary>
        /// Contains information about what should be displayed in a slot.
        /// </summary>
        private readonly struct SlotDisplayInfo
        {
            public readonly ItemStackSnapshot Item;
            public readonly EquipmentSlotKey Slot;
            public readonly VisualRulePriority Priority;

            public SlotDisplayInfo(ItemStackSnapshot item, EquipmentSlotKey slot, VisualRulePriority priority = VisualRulePriority.Default)
            {
                Item = item;
                Slot = slot;
                Priority = priority;
            }

            public bool HasDisplay => Priority != VisualRulePriority.Hide && Item.Definition != null;

            public SlotDisplayInfo WithPriority(VisualRulePriority priority) =>
                new SlotDisplayInfo(Item, Slot, priority);
        }
        #endregion
    }
}
