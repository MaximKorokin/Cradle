using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.Systems;
using System;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class PlayerPromptBehaviourModule : EntityModuleBase
    {
        private readonly Action<string, string> _onOpen;

        public float Radius { get; }
        public string PromptText { get; }
        public string ButtonText { get; }

        public PlayerPromptBehaviourModule(float radius, string promptText, string buttonText, Action<string, string> onOpen)
        {
            Radius = radius;
            PromptText = promptText;
            ButtonText = buttonText;
            _onOpen = onOpen;
        }

        public void Open(string entityId, string targetId) => _onOpen(entityId, targetId);
    }

    public sealed class PlayerPromptBehaviourModuleFactory : IEntityModuleFactory
    {
        private readonly IGlobalEventBus _globalEventBus;

        public PlayerPromptBehaviourModuleFactory(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<ShopModuleDefinition>(out var shopDefinition) && shopDefinition.Radius > 0)
                return new PlayerPromptBehaviourModule(shopDefinition.Radius, shopDefinition.ShopDefinition.ShopName ?? "Shop", "Open",
                    (entityId, targetId) => _globalEventBus.Publish(
                        new ShopWindowOpenRequest(new ObservableData<string>(entityId), new ObservableData<string>(targetId))));

            if (entityDefinition.TryGetModuleDefinition<CraftingModuleDefinition>(out var craftDefinition) && craftDefinition.Radius > 0)
                return new PlayerPromptBehaviourModule(craftDefinition.Radius, craftDefinition.CrafterName, "Craft",
                    (entityId, targetId) => _globalEventBus.Publish(
                        new CraftingWindowOpenRequest(new ObservableData<string>(entityId), new ObservableData<string>(targetId))));

            if (entityDefinition.TryGetModuleDefinition<StorageModuleDefinition>(out var storageDefinition) && storageDefinition.Radius > 0)
                return new PlayerPromptBehaviourModule(storageDefinition.Radius, entityDefinition.DisplayName, "Open",
                    (entityId, targetId) => _globalEventBus.Publish(
                        new StorageWindowOpenRequest(new ObservableData<string>(entityId), new ObservableData<string>(targetId))));

            if (entityDefinition.TryGetModuleDefinition<QuestGiverModuleDefinition>(out var questGiverDefinition) && questGiverDefinition.Radius > 0)
                return new PlayerPromptBehaviourModule(questGiverDefinition.Radius, entityDefinition.DisplayName, "Talk",
                    (entityId, targetId) => _globalEventBus.Publish(
                        new QuestGiverWindowOpenRequest(new ObservableData<string>(entityId), new ObservableData<string>(targetId))));

            return null;
        }
    }
}
