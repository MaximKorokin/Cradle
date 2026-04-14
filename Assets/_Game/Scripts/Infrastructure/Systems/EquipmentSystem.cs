using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items.Commands;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class EquipmentSystem : EntitySystemBase, ITickSystem
    {
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

            for (int i = 0; i < equipmentModule.Equipment.Slots.Count; i++)
            {
                var command = new UseItemCommand(ItemContainerId.Equipment, equipmentModule.Equipment.Slots[i].ToInt64(), false);
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
