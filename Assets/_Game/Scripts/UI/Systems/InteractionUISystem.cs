using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class InteractionUISystem : UISystemBase
    {
        private WindowManager _windowManager;
        private EntityRepository _entityRepository;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            WindowManager windowManager,
            EntityRepository entityRepository)
        {
            BaseConstruct(globalEventBus);

            _windowManager = windowManager;
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
                _windowManager.InstantiateWindow<InventoryShopWindow, InventoryShopWindowControllerArguments>(new(
                    request.ShopEntityId,
                    request.InventoryEntityId,
                    shopModule.Definition.ShopName,
                    shopModule.Definition.BuyCoefficient,
                    shopModule.Definition.SellCoefficient));
            }
        }

        private void OnCraftingWindowOpenRequest(CraftingWindowOpenRequest request)
        {
            _windowManager.InstantiateWindow<CraftingWindow, CraftingWindowControllerArguments>(
                new(request.CrafterEntityId, request.InventoryEntityId, request.InventoryEntityId));
        }

        private void OnStorageWindowOpenRequest(StorageWindowOpenRequest request)
        {
            _windowManager.InstantiateWindow<InventoryStorageWindow, InventoryStorageWindowControllerArguments>(
                new(request.StorageEntityId, request.InventoryEntityId, request.InventoryEntityId));
        }

        private void OnQuestGiverWindowOpenRequest(QuestGiverWindowOpenRequest request)
        {
            _windowManager.InstantiateWindow<QuestGiverWindow, QuestGiverWindowControllerArguments>(
                new(request.GiverEntityId, request.TargetEntityId));
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
