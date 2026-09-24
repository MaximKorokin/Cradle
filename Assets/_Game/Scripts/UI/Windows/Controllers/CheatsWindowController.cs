using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : WindowControllerBase<CheatsWindow, CheatsWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly IPlayerProvider _playerProvider;
        private readonly EntityRepository _entityRepository;
        private readonly CheatsHudData _cheatsHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly ItemStackFactory _itemStackAssembler;
        private readonly ItemPreviewService _itemPreviewService;

        public CheatsWindowController(
            IGlobalEventBus globalEventBus,
            IPlayerProvider playerProvider,
            EntityRepository entityRepository,
            CheatsHudData cheatsHudData,
            EquipmentHudData equipmentHudData,
            ItemStackFactory itemStackAssembler,
            ItemPreviewService itemPreviewService)
        {
            _globalEventBus = globalEventBus;
            _playerProvider = playerProvider;
            _entityRepository = entityRepository;
            _cheatsHudData = cheatsHudData;
            _equipmentHudData = equipmentHudData;
            _itemStackAssembler = itemStackAssembler;
            _itemPreviewService = itemPreviewService;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentHudData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.ItemDefinitionInfoClicked += OnItemDefinitionInfoClicked;
            Window.ItemDefinitionActionClicked += OnItemDefinitionActionClicked;
            Window.StatusEffectDefinitionClicked += OnStatusEffectDefinitionClicked;

            Window.GameControlTabContent.ResetPlayerQuestsButtonClicked += OnResetPlayerQuestsButtonClicked;
            Window.GameControlTabContent.ResetPlayerLevelButtonClicked += OnResetPlayerLevelButtonClicked;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.ItemDefinitionInfoClicked -= OnItemDefinitionInfoClicked;
            Window.ItemDefinitionActionClicked -= OnItemDefinitionActionClicked;
            Window.StatusEffectDefinitionClicked -= OnStatusEffectDefinitionClicked;

            Window.GameControlTabContent.ResetPlayerQuestsButtonClicked -= OnResetPlayerQuestsButtonClicked;
            Window.GameControlTabContent.ResetPlayerLevelButtonClicked -= OnResetPlayerLevelButtonClicked;
        }

        private void OnStatusEffectDefinitionClicked(StatusEffectDefinition statusEffectDefinition)
        {
            if (_entityRepository.Get(Arguments.InventoryEntityId.Value).TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                var statusEffect = new StatusEffect(statusEffectDefinition);
                statusEffectModule.StatusEffects.AddStatusEffect(statusEffect);
            }
        }

        private void OnItemDefinitionInfoClicked(ItemDefinition itemDefinition)
        {
            _itemPreviewService.ShowItemDefinitionPreview(
                itemDefinition,
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId.Value),
                _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(itemDefinition));
        }

        private void OnItemDefinitionActionClicked(ItemDefinition itemDefinition)
        {
            if (_entityRepository.Get(Arguments.InventoryEntityId.Value).TryGetModule<InventoryModule>(out var inventoryModule))
            {
                WindowUtils.ShowAmountPickerIfNeeded(_globalEventBus, itemDefinition.MaxAmount, itemDefinition.MaxAmount, amount =>
                {
                    inventoryModule.Inventory.Add(_itemStackAssembler.Create(itemDefinition.Id, amount).Snapshot);
                });
            }
        }

        private void OnResetPlayerQuestsButtonClicked()
        {
            _globalEventBus.Publish(new ResetEntityModuleRequest(_playerProvider.Player, typeof(QuestModule)));
        }

        private void OnResetPlayerLevelButtonClicked()
        {
            _globalEventBus.Publish(new ResetEntityModuleRequest(_playerProvider.Player, typeof(LevelingModule)));
        }

        protected override void Redraw()
        {
            Window.Render(_cheatsHudData);
        }
    }

    public readonly struct CheatsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public CheatsWindowControllerArguments(IReadOnlyObservableData<string> inventoryEntityId, IReadOnlyObservableData<string> equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
