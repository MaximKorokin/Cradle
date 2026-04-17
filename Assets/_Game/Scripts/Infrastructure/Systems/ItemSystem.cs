using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ItemSystem : EntitySystemBase
    {
        private readonly ItemCommandHandler _itemCommandHandler;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled);

        public ItemSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            ItemCommandHandler itemCommandHandler) : base(globalEventBus, repository)
        {
            _itemCommandHandler = itemCommandHandler;

            TrackEntityEvent<ItemCommandRequest>(OnItemCommandRequested);
        }

        private void OnItemCommandRequested(Entity entity, ItemCommandRequest e)
        {
            _itemCommandHandler.Handle(entity, e.Command);
        }
    }

    public readonly struct ItemCommandRequest : IEntityEvent
    {
        public readonly IItemCommand Command;

        public ItemCommandRequest(IItemCommand command)
        {
            Command = command;
        }
    }

    public readonly struct ItemUseStartedEvent : IEntityEvent
    {
        public readonly ItemStackSnapshot Item;
        public readonly ItemUseSettings ItemUseSettings;

        public ItemUseStartedEvent(ItemStackSnapshot item, ItemUseSettings itemUseSettings)
        {
            Item = item;
            ItemUseSettings = itemUseSettings;
        }
    }
}
