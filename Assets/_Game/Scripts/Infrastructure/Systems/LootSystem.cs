using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Loot;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LootSystem : SystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityFactory _entityAssembler;
        private readonly DefaultEntityDefinitionReferences _defaultEntityDefinitionReferences;

        public LootSystem(
            IGlobalEventBus globalEventBus,
            EntityFactory entityAssembler,
            DefaultEntityDefinitionReferences defaultEntityDefinitionReferences)
        {
            _globalEventBus = globalEventBus;
            _entityAssembler = entityAssembler;
            _defaultEntityDefinitionReferences = defaultEntityDefinitionReferences;

            _globalEventBus.Subscribe<LootDropRequestedEvent>(OnLootDropRequested);
            _globalEventBus.Subscribe<LootItemDropRequestedEvent>(OnLootItemDropRequested);
        }

        public override void Dispose()
        {
            base.Dispose();
            _globalEventBus.Unsubscribe<LootDropRequestedEvent>(OnLootDropRequested);
            _globalEventBus.Unsubscribe<LootItemDropRequestedEvent>(OnLootItemDropRequested);
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

                CreateLoot(e.Position, entry.ItemDefinition, amount);
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
            var entity = _entityAssembler.Create(_defaultEntityDefinitionReferences.LootItem);
            _globalEventBus.Publish<SpawnEntityViewRequest>(new(entity, position));

            entity.GetModule<AppearanceModule>().RequestSetUnitSprite(itemDefinition.Sprite);

            entity.AddModule(new LootItemModule(itemDefinition, amount));
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
