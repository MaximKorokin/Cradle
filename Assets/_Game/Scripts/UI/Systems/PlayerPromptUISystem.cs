using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class PlayerPromptUISystem : UISystemBase
    {
        private EntityRepository _entityRepository;
        private PlayerPromptWidget _playerPromptWidget;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            PlayerPromptWidget playerPromptWidget)
        {
            BaseConstruct(globalEventBus);

            _entityRepository = entityRepository;
            _playerPromptWidget = playerPromptWidget;

            TrackGlobalEvent<ShopWindowOpenRequest>(OnShopWindowOpenRequest);
            TrackGlobalEvent<CraftingWindowOpenRequest>(OnCraftingWindowOpenRequest);
            TrackGlobalEvent<StorageWindowOpenRequest>(OnStorageWindowOpenRequest);
            TrackGlobalEvent<QuestGiverWindowOpenRequest>(OnQuestGiverWindowOpenRequest);

            TrackGlobalEvent<PlayerPromptRequest>(OnPlayerPromptRequested);
        }

        private void OnShopWindowOpenRequest(ShopWindowOpenRequest request)
        {
            var shopEntity = _entityRepository.Get(request.ShopEntityId.Value);
            if (shopEntity.TryGetModule<ShopModule>(out var shopModule))
            {
                GlobalEventBus.Publish(new WindowToggleRequest(
                    WindowId.Shop,
                    new ShopWindowControllerArguments(
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
                new CraftingWindowControllerArguments(request.CrafterEntityId, request.InventoryEntityId)));
        }

        private void OnStorageWindowOpenRequest(StorageWindowOpenRequest request)
        {
            GlobalEventBus.Publish(new WindowToggleRequest(
                WindowId.Storage,
                new StorageWindowControllerArguments(request.StorageEntityId)));
        }

        private void OnQuestGiverWindowOpenRequest(QuestGiverWindowOpenRequest request)
        {
            GlobalEventBus.Publish(new WindowToggleRequest(
                WindowId.QuestGiver,
                new QuestGiverWindowControllerArguments(request.GiverEntityId, request.TargetEntityId)));
        }

        private void OnPlayerPromptRequested(PlayerPromptRequest request)
        {
            if (request.Show)
            {
                _playerPromptWidget.Show(request.PromptText, request.ButtonText, request.Callback);
            }
            else
            {
                _playerPromptWidget.Hide();
            }
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

        public StorageWindowOpenRequest(IReadOnlyObservableData<string> storageEntityId)
        {
            StorageEntityId = storageEntityId;
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

    public readonly struct PlayerPromptRequest : IGlobalEvent
    {
        public string PromptText { get; }
        public string ButtonText { get; }
        public bool Show { get; }
        public Action Callback { get; }

        private PlayerPromptRequest(string promptText, string buttonText, bool show, Action callback = null)
        {
            PromptText = promptText;
            ButtonText = buttonText;
            Show = show;
            Callback = callback;
        }

        public static PlayerPromptRequest GetShowRequest(string promptText, string buttonText, Action callback = null)
        {
            return new PlayerPromptRequest(promptText, buttonText, true, callback);
        }

        public static PlayerPromptRequest GetHideRequest()
        {
            return new PlayerPromptRequest(string.Empty, string.Empty, false, null);
        }
    }
}
