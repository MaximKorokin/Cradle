using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Loot;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LootSystem : EntitySystemBase
    {
        private readonly LootConfig _lootConfig;
        private readonly DefaultEntityDefinitionReferences _defaultEntityDefinitionReferences;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(LootItemModule) }
            );

        public LootSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            LootConfig lootConfig,
            DefaultEntityDefinitionReferences defaultEntityDefinitionReferences) : base(globalEventBus, entityRepository)
        {
            _lootConfig = lootConfig;
            _defaultEntityDefinitionReferences = defaultEntityDefinitionReferences;

            TrackGlobalEvent<LootDropRequestedEvent>(OnLootDropRequested);
            TrackGlobalEvent<LootItemDropRequestedEvent>(OnLootItemDropRequested);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (!EntityQuery.Match(entity)) return;

            entity.SubscribeOnce<EntityBoundEvent>(e =>
            {
                // Set the loot item's sprite based on its item definition
                if (entity.TryGetModule<AppearanceModule>(out var appearanceModule))
                {
                    var lootItemModule = entity.GetModule<LootItemModule>();
                    appearanceModule.RequestSetUnitSprite(lootItemModule.ItemDefinition.Sprite);
                }
            });
        }

        private void OnLootDropRequested(LootDropRequestedEvent e)
        {
            var entries = e.LootTable.LootEntries;
            if (entries == null) return;

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry.ItemDefinition == null)
                    continue;

                if (Random.value > entry.Chance)
                    continue;

                int amount = Random.Range(entry.MinAmount, entry.MaxAmount + 1);
                if (amount <= 0)
                    continue;

                var spawnPosition = e.Position + Random.insideUnitCircle * _lootConfig.SpawnRadius;
                CreateLoot(spawnPosition, entry.ItemDefinition, amount);
            }
        }

        private void OnLootItemDropRequested(LootItemDropRequestedEvent e)
        {
            if (e.ItemDefinition != null && e.Amount > 0)
            {
                CreateLoot(e.Position, e.ItemDefinition, e.Amount);
            }
        }

        private void CreateLoot(Vector2 position, ItemDefinition itemDefinition, int amount)
        {
            GlobalEventBus.Publish(new SpawnEntityRequest(
                _defaultEntityDefinitionReferences.LootItem,
                position,
                new[] {
                    new LootItemEntitySpawnInitializer(itemDefinition, amount)
                }));
        }
    }

    public readonly struct LootDropRequestedEvent : IGlobalEvent
    {
        public Vector2 Position { get; }
        public LootTable LootTable { get; }

        public LootDropRequestedEvent(Vector2 position, LootTable lootTable)
        {
            Position = position;
            LootTable = lootTable;
        }
    }

    public readonly struct LootItemDropRequestedEvent : IGlobalEvent
    {
        public Vector2 Position { get; }
        public ItemDefinition ItemDefinition { get; }
        public int Amount { get; }

        public LootItemDropRequestedEvent(Vector2 position, ItemDefinition itemDefinition, int amount)
        {
            Position = position;
            ItemDefinition = itemDefinition;
            Amount = amount;
        }
    }
}
