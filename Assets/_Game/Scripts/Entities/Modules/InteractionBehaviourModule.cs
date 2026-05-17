using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using System;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class InteractionBehaviourModule : EntityModuleBase
    {
        private readonly Action<string, string> _onOpen;

        public float Radius { get; }
        public string PromptText { get; }
        public string ButtonText { get; }

        public InteractionBehaviourModule(float radius, string promptText, string buttonText, Action<string, string> onOpen)
        {
            Radius = radius;
            PromptText = promptText;
            ButtonText = buttonText;
            _onOpen = onOpen;
        }

        public void Open(string entityId, string targetId) => _onOpen(entityId, targetId);
    }

    public sealed class InteractionBehaviourModuleFactory : IEntityModuleFactory
    {
        private readonly IGlobalEventBus _globalEventBus;

        public InteractionBehaviourModuleFactory(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<ShopModuleDefinition>(out var shopDefinition))
                return new InteractionBehaviourModule(shopDefinition.Radius, shopDefinition.ShopDefinition.ShopName ?? "Shop", "Open",
                    (entityId, targetId) => _globalEventBus.Publish(new ShopWindowOpenRequest(entityId, targetId)));

            if (entityDefinition.TryGetModuleDefinition<CraftingModuleDefinition>(out var craftDefinition))
                return new InteractionBehaviourModule(craftDefinition.Radius, craftDefinition.CrafterName, "Craft",
                    (entityId, targetId) => _globalEventBus.Publish(new CraftingWindowOpenRequest(entityId, targetId)));

            if (entityDefinition.TryGetModuleDefinition<StorageModuleDefinition>(out var storageDefinition) && storageDefinition.Radius > 0)
                return new InteractionBehaviourModule(storageDefinition.Radius, "Storage", "Open",
                    (entityId, targetId) => _globalEventBus.Publish(new StorageWindowOpenRequest(entityId, targetId)));

            if (entityDefinition.TryGetModuleDefinition<QuestGiverModuleDefinition>(out var questGiverDefinition))
                return new InteractionBehaviourModule(questGiverDefinition.Radius, "Quests", "Talk",
                    (entityId, targetId) => _globalEventBus.Publish(new QuestGiverWindowOpenRequest(entityId, targetId)));

            return null;
        }
    }
}
