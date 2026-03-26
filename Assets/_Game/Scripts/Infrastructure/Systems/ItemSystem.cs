using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items.Commands;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ItemSystem : EntitySystemBase
    {
        private readonly ItemCommandHandler _itemCommandHandler;

        protected override EntityQuery EntityQuery { get; } = new (RestrictionState.Disabled);

        public ItemSystem(EntityRepository repository, ItemCommandHandler itemCommandHandler) : base(repository)
        {
            _itemCommandHandler = itemCommandHandler;

            TrackEntityEvent<ItemCommandRequest>(OnItemCommandRequested);
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
}
