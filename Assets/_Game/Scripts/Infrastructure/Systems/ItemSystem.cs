using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ItemSystem : EntitySystemBase, ITickSystem
    {
        private readonly ItemCommandHandler _itemCommandHandler;

        protected override EntityQuery EntityQuery { get; } = new (RestrictionState.Disabled);

        public ItemSystem(EntityRepository repository, ItemCommandHandler itemCommandHandler) : base(repository)
        {
            _itemCommandHandler = itemCommandHandler;

            TrackEntityEvent<ItemCommandRequest>(OnItemCommandRequested);
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(TickEntity);
        }

        private void TickEntity(Entity entity)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule)) return;

            for (int i = 0; i < equipmentModule.Equipment.Slots.Count; i++)
            {
                var command = new UseItemCommand(ItemContainerId.Equipment, equipmentModule.Equipment.Slots[i].ToInt64(), false);
                _itemCommandHandler.Handle(entity, command);
            }
        }

        private void OnItemCommandRequested(ItemCommandRequest e)
        {
            _itemCommandHandler.Handle(e.Entity, e.Command);
        }
    }

    public readonly struct ItemCommandRequest : IEntityEvent
    {
        public readonly IItemCommand Command;

        public Entity Entity { get; }

        public ItemCommandRequest(Entity entity, IItemCommand command)
        {
            Entity = entity;
            Command = command;
        }
    }

    public readonly struct ItemUseStartedEvent : IEntityEvent
    {
        public Entity Entity { get; }
        public readonly ItemStackSnapshot Item;
        public readonly bool IsManual;

        public ItemUseStartedEvent(Entity entity, ItemStackSnapshot item, bool isManual)
        {
            Entity = entity;
            Item = item;
            IsManual = isManual;
        }
    }
}
