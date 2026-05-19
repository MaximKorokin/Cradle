using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
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
            var shopEntity = _entityRepository.Get(request.ShopEntityId);
            if (shopEntity.TryGetModule<ShopModule>(out var shopModule))
            {
                _windowManager.InstantiateWindow<InventoryShopWindow, InventoryShopWindowControllerArguments>(new(
                    ItemContainerPath.Shop(request.ShopEntityId),
                    ItemContainerPath.Inventory(request.InventoryEntityId),
                    ItemContainerPath.Equipment(request.InventoryEntityId),
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
            SLog.Log("Quest giver window open request recieved");
        }
    }

    public readonly struct ShopWindowOpenRequest : IGlobalEvent
    {
        public string ShopEntityId { get; }
        public string InventoryEntityId { get; }

        public ShopWindowOpenRequest(string shopEntityId, string inventoryEntityId)
        {
            ShopEntityId = shopEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct CraftingWindowOpenRequest : IGlobalEvent
    {
        public string CrafterEntityId { get; }
        public string InventoryEntityId { get; }

        public CraftingWindowOpenRequest(string crafterEntityId, string inventoryEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct StorageWindowOpenRequest : IGlobalEvent
    {
        public string StorageEntityId { get; }
        public string InventoryEntityId { get; }

        public StorageWindowOpenRequest(string storageEntityId, string inventoryEntityId)
        {
            StorageEntityId = storageEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct QuestGiverWindowOpenRequest : IGlobalEvent
    {
        public string GiverEntityId { get; }
        public string TargetEntityId { get; }

        public QuestGiverWindowOpenRequest(string giverEntityId, string targetEntityId)
        {
            GiverEntityId = giverEntityId;
            TargetEntityId = targetEntityId;
        }
    }
}
