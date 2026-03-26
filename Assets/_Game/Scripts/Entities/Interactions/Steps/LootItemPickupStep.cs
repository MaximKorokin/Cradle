using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class LootItemPickupStep : IInteractionStep
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityRepository _entityRepository;

        private bool _done;

        public LootItemPickupStep(IGlobalEventBus globalEventBus, EntityRepository entityRepository)
        {
            _globalEventBus = globalEventBus;
            _entityRepository = entityRepository;
        }

        public void Start(in InteractionContext context) => _done = false;

        public void Cancel(in InteractionContext context) => _done = true;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            var lootItemModule = context.Target.GetModule<LootItemModule>();
            var inventory = context.Source.GetModule<InventoryModule>().Inventory;
            var added = inventory.Add(new(lootItemModule.ItemDefinition, new EmptyInstanceData(), lootItemModule.Amount));

            if (added <= 0)
            {
                return StepStatus.Failed;
            }

            lootItemModule.Amount -= added;

            if (lootItemModule.Amount <= 0)
            {
                _globalEventBus.Publish<DespawnEntityViewRequestEvent>(new(context.Target));
                _entityRepository.Remove(((IEntry)context.Target).Id);
            }

            _done = true;
            return StepStatus.Completed;
        }
    }
}
