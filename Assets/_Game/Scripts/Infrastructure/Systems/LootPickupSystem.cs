using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LootPickupSystem : EntitySystemBase, ITickSystem
    {
        private readonly GlobalEventBus _globalEventBus;

        protected override EntityQuery EntityQuery => new(
            RestrictionState.Disabled | RestrictionState.Dead | RestrictionState.Feared | RestrictionState.Stunned,
            new[] { typeof(LootPickupModule), typeof(InventoryEquipmentModule) });

        public LootPickupSystem(EntityRepository repository, GlobalEventBus globalEventBus) : base(repository)
        {
            _globalEventBus = globalEventBus;
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(IterateEntity);
        }

        private void IterateEntity(Entity entity)
        {
            var intentModule = entity.GetModule<IntentModule>();

            if (!intentModule.TryConsumePickupItem(out var pickupItemIntent)) return;

            var lootItemModule = pickupItemIntent.LootItem.GetModule<LootItemModule>();
            var inventory = entity.GetModule<InventoryEquipmentModule>().Inventory;
            inventory.Add(new(lootItemModule.ItemDefinition, new EmptyInstanceData(), lootItemModule.Amount));

            _globalEventBus.Publish<DespawnEntityViewRequestEvent>(new(pickupItemIntent.LootItem));
            EntityRepository.Remove(((IEntry)pickupItemIntent.LootItem).Id);
        }
    }
}
