using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using UnityEngine;

namespace Assets._Game.Scripts.Quests.Objectives
{
    public sealed class ItemsInInventoryObjectiveDefinition : ObjectiveDefinition
    {
        [field: SerializeField]
        public ItemDefinition Item { get; private set; }

        public override ObjectiveProgress CreateProgress()
        {
            return new ItemsInInventoryObjectiveProgress(this);
        }
    }

    public sealed class ItemsInInventoryObjectiveProgress : ObjectiveProgress<ItemsInInventoryObjectiveDefinition>
    {
        public ItemsInInventoryObjectiveProgress(ItemsInInventoryObjectiveDefinition definition) : base(definition)
        {
        }

        public override void HandleEvent(IEvent e)
        {
            // Only update progress if the event is an InventoryChangedEvent that involves the relevant item definition.
            if (e is InventoryChangedEvent inventoryChangedEvent && inventoryChangedEvent.Item.HasValue && inventoryChangedEvent.Item.Value.Definition == Definition.Item)
            {
                var key = ItemKey.From(Definition.Item, null);
                int itemCount = inventoryChangedEvent.Inventory.Count(key);
                SetProgress(itemCount);
            }
        }
    }
}
