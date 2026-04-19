using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class EquipmentSystem : EntitySystemBase, ITickSystem
    {
        private readonly Dictionary<ItemDefinition, long> _autoUsedItems = new();

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled, new[] { typeof(EquipmentModule) });

        public EquipmentSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository) : base(globalEventBus, repository)
        {
            TrackEntityEvent<ItemUseSettingsUpdateRequest>(OnItemUseSettingsUpdateRequested);
        }

        private void OnItemUseSettingsUpdateRequested(Entity entity, ItemUseSettingsUpdateRequest request)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule)) return;

            equipmentModule.SetAutoItemUseSettings(request.ItemUseSettings);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(TickEntity);
        }

        private void TickEntity(Entity entity)
        {
            var equipmentModule = entity.GetModule<EquipmentModule>();

            _autoUsedItems.Clear();

            // Auto use items in equipment slots
            // Prevent using the same item multiple times if it appears in multiple slots
            foreach (var (slot, item) in equipmentModule.Equipment.Enumerate())
            {
                if (item == null || !_autoUsedItems.TryAdd(item.Value.Definition, slot.ToInt64())) continue;
            }

            // Publish use item commands for auto used items
            foreach (var slot in _autoUsedItems.Values)
            {
                var command = new UseItemCommand(ItemContainerId.Equipment, slot, false);
                var request = new ItemCommandRequest(command);
                entity.Publish(request);
            }
        }
    }

    public readonly struct ItemUseSettingsUpdateRequest : IEntityEvent
    {
        public readonly ItemUseSettings ItemUseSettings;

        public ItemUseSettingsUpdateRequest(ItemUseSettings itemUseSettings)
        {
            ItemUseSettings = itemUseSettings;
        }
    }
}
