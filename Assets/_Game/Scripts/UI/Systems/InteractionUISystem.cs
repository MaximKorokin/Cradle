using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class InteractionUISystem : UISystemBase
    {
        private EntityRepository _entityRepository;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository)
        {
            BaseConstruct(globalEventBus);

            _entityRepository = entityRepository;

            TrackGlobalEvent<ShopWindowOpenRequest>(OnShopWindowOpenRequest);
            TrackGlobalEvent<CraftingWindowOpenRequest>(OnCraftingWindowOpenRequest);
            TrackGlobalEvent<StorageWindowOpenRequest>(OnStorageWindowOpenRequest);
            TrackGlobalEvent<QuestGiverWindowOpenRequest>(OnQuestGiverWindowOpenRequest);
        }

        private void OnShopWindowOpenRequest(ShopWindowOpenRequest request)
        {
            var shopEntity = _entityRepository.Get(request.ShopEntityId.Value);
            if (shopEntity.TryGetModule<ShopModule>(out var shopModule))
            {
                GlobalEventBus.Publish(new WindowToggleRequest(
                    WindowId.Shop,
                    new InventoryShopWindowControllerArguments(
                        request.ShopEntityId,
                        request.InventoryEntityId,
                        shopModule.Definition.ShopName,
                        shopModule.Definition.BuyCoefficient,
                        shopModule.Definition.SellCoefficient)));
            }
        }

        private void OnCraftingWindowOpenRequest(CraftingWindowOpenRequest request)
        {
            GlobalEventBus.Publish(new WindowToggleRequest(
                WindowId.Crafting,
                new CraftingWindowControllerArguments(request.CrafterEntityId, request.InventoryEntityId, request.InventoryEntityId)));
        }

        private void OnStorageWindowOpenRequest(StorageWindowOpenRequest request)
        {
            GlobalEventBus.Publish(new WindowToggleRequest(
                WindowId.Storage,
                new InventoryStorageWindowControllerArguments(request.StorageEntityId, request.InventoryEntityId, request.InventoryEntityId)));
        }

        private void OnQuestGiverWindowOpenRequest(QuestGiverWindowOpenRequest request)
        {
            GlobalEventBus.Publish(new WindowToggleRequest(
                WindowId.QuestGiver,
                new QuestGiverWindowControllerArguments(request.GiverEntityId, request.TargetEntityId)));
        }
    }

    public readonly struct ShopWindowOpenRequest : IGlobalEvent
    {
        public IReadOnlyObservableData<string> ShopEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }

        public ShopWindowOpenRequest(IReadOnlyObservableData<string> shopEntityId, IReadOnlyObservableData<string> inventoryEntityId)
        {
            ShopEntityId = shopEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct CraftingWindowOpenRequest : IGlobalEvent
    {
        public IReadOnlyObservableData<string> CrafterEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }

        public CraftingWindowOpenRequest(IReadOnlyObservableData<string> crafterEntityId, IReadOnlyObservableData<string> inventoryEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct StorageWindowOpenRequest : IGlobalEvent
    {
        public IReadOnlyObservableData<string> StorageEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }

        public StorageWindowOpenRequest(IReadOnlyObservableData<string> storageEntityId, IReadOnlyObservableData<string> inventoryEntityId)
        {
            StorageEntityId = storageEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct QuestGiverWindowOpenRequest : IGlobalEvent
    {
        public IReadOnlyObservableData<string> GiverEntityId { get; }
        public IReadOnlyObservableData<string> TargetEntityId { get; }

        public QuestGiverWindowOpenRequest(IReadOnlyObservableData<string> giverEntityId, IReadOnlyObservableData<string> targetEntityId)
        {
            GiverEntityId = giverEntityId;
            TargetEntityId = targetEntityId;
        }
    }
}
