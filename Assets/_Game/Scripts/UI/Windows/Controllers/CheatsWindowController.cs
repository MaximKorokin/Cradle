using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Services;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : WindowControllerBase<CheatsWindow, CheatsWindowControllerArguments>
    {
        private CheatsWindow _window;

        private readonly IGlobalEventBus _globalEventBus;
        private readonly IPlayerProvider _playerProvider;
        private readonly EntityRepository _entityRepository;
        private readonly WindowManager _windowManager;
        private readonly CheatsHudData _cheatsHudData;
        private readonly EquipmentHudData _equipmentHudData;
        private readonly ItemStackFactory _itemStackAssembler;
        private readonly ItemPreviewService _itemPreviewService;

        public CheatsWindowController(
            IGlobalEventBus globalEventBus,
            IPlayerProvider playerProvider,
            EntityRepository entityRepository,
            WindowManager windowManager,
            CheatsHudData cheatsHudData,
            EquipmentHudData equipmentHudData,
            ItemStackFactory itemStackAssembler,
            ItemPreviewService itemPreviewService)
        {
            _globalEventBus = globalEventBus;
            _playerProvider = playerProvider;
            _entityRepository = entityRepository;
            _windowManager = windowManager;
            _cheatsHudData = cheatsHudData;
            _equipmentHudData = equipmentHudData;
            _itemStackAssembler = itemStackAssembler;
            _itemPreviewService = itemPreviewService;
        }

        public override void Initialize(CheatsWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _equipmentHudData.SetEquipmentEntity(arguments.EquipmentEntityId);
        }

        public override void Bind(CheatsWindow window)
        {
            _window = window;
            _window.ItemDefinitionInfoClicked += OnItemDefinitionInfoClicked;
            _window.ItemDefinitionActionClicked += OnItemDefinitionActionClicked;
            _window.StatusEffectDefinitionClicked += OnStatusEffectDefinitionClicked;

            _window.GameControlTabContent.ResetPlayerQuestsButtonClicked += OnResetPlayerQuestsButtonClicked;
            _window.GameControlTabContent.ResetPlayerLevelButtonClicked += OnResetPlayerLevelButtonClicked;

            _window.Render(_cheatsHudData);
        }

        public override void Unbind()
        {
            _window.ItemDefinitionInfoClicked -= OnItemDefinitionInfoClicked;
            _window.ItemDefinitionActionClicked -= OnItemDefinitionActionClicked;
            _window.StatusEffectDefinitionClicked -= OnStatusEffectDefinitionClicked;

            _window.GameControlTabContent.ResetPlayerQuestsButtonClicked -= OnResetPlayerQuestsButtonClicked;
            _window.GameControlTabContent.ResetPlayerLevelButtonClicked -= OnResetPlayerLevelButtonClicked;

        }

        private void OnStatusEffectDefinitionClicked(StatusEffectDefinition statusEffectDefinition)
        {
            if (_entityRepository.Get(Arguments.InventoryEntityId).TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                var statusEffect = new StatusEffect(statusEffectDefinition);
                statusEffectModule.StatusEffects.AddStatusEffect(statusEffect);
            }
        }

        private void OnItemDefinitionInfoClicked(ItemDefinition itemDefinition)
        {
            _itemPreviewService.ShowItemDefinitionPreview(
                itemDefinition,
                ItemContainerPath.Equipment(Arguments.EquipmentEntityId),
                _equipmentHudData.EquipmentModel.FindOccupiedSlotForItem(itemDefinition));
        }

        private void OnItemDefinitionActionClicked(ItemDefinition itemDefinition)
        {
            if (_entityRepository.Get(Arguments.InventoryEntityId).TryGetModule<InventoryModule>(out var inventoryModule))
            {
                _windowManager.ShowAmountPickerIfNeeded(itemDefinition.MaxAmount, itemDefinition.MaxAmount, amount =>
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
    }

    public readonly struct CheatsWindowControllerArguments : IWindowControllerArguments
    {
        public string InventoryEntityId { get; }
        public string EquipmentEntityId { get; }

        public CheatsWindowControllerArguments(string inventoryEntityId, string equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
