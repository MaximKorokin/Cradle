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
            if (!_entityRepository.TryGet(request.ShopEntityId.Value, out var shopEntity))
                return;

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
        public IReadOnlyObservableData<EntryRef> ShopEntityId { get; }
        public IReadOnlyObservableData<EntryRef> InventoryEntityId { get; }

        public ShopWindowOpenRequest(IReadOnlyObservableData<EntryRef> shopEntityId, IReadOnlyObservableData<EntryRef> inventoryEntityId)
        {
            ShopEntityId = shopEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct CraftingWindowOpenRequest : IGlobalEvent
    {
        public IReadOnlyObservableData<EntryRef> CrafterEntityId { get; }
        public IReadOnlyObservableData<EntryRef> InventoryEntityId { get; }

        public CraftingWindowOpenRequest(IReadOnlyObservableData<EntryRef> crafterEntityId, IReadOnlyObservableData<EntryRef> inventoryEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }

    public readonly struct StorageWindowOpenRequest : IGlobalEvent
    {
        public IReadOnlyObservableData<EntryRef> StorageEntityId { get; }

        public StorageWindowOpenRequest(IReadOnlyObservableData<EntryRef> storageEntityId)
        {
            StorageEntityId = storageEntityId;
        }
    }

    public readonly struct QuestGiverWindowOpenRequest : IGlobalEvent
    {
        public IReadOnlyObservableData<EntryRef> GiverEntityId { get; }
        public IReadOnlyObservableData<EntryRef> TargetEntityId { get; }

        public QuestGiverWindowOpenRequest(IReadOnlyObservableData<EntryRef> giverEntityId, IReadOnlyObservableData<EntryRef> targetEntityId)
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
