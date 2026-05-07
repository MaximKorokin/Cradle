using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class ShopUISystem : UISystemBase
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
        }

        private void OnShopWindowOpenRequest(ShopWindowOpenRequest request)
        {
            var shopEntity = _entityRepository.Get(request.ShopEntityId);
            if (shopEntity.TryGetModule<ShopModule>(out var shopModule))
            {
                _windowManager.InstantiateWindow<InventoryShopWindow, InventoryShopWindowControllerArguments>(
                    new(shopModule.Shop, shopModule.Definition.ShopName, shopModule.Definition.BuyCoefficient, shopModule.Definition.SellCoefficient));
            }
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
}
