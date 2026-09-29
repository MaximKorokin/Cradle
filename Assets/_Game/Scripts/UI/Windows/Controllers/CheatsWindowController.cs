using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : WindowControllerBase<CheatsWindow, CheatsWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly IPlayerProvider _playerProvider;
        private readonly EntityRepository _entityRepository;
        private readonly CheatsViewData _cheatsViewData;
        private readonly EquipmentViewData _equipmentViewData;
        private readonly ItemStackFactory _itemStackAssembler;
        private readonly CheatsViewController _cheatsViewController;

        public CheatsWindowController(
            IGlobalEventBus globalEventBus,
            IPlayerProvider playerProvider,
            EntityRepository entityRepository,
            CheatsViewData cheatsViewData,
            EquipmentViewData equipmentViewData,
            ItemStackFactory itemStackAssembler,
            CheatsViewController cheatsViewController)
        {
            _globalEventBus = globalEventBus;
            _playerProvider = playerProvider;
            _entityRepository = entityRepository;
            _cheatsViewData = cheatsViewData;
            _equipmentViewData = equipmentViewData;
            _itemStackAssembler = itemStackAssembler;
            _cheatsViewController = cheatsViewController;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _cheatsViewController.Initialize(Window.CheatsView);
            _cheatsViewController.Bind(_cheatsViewData);
            
            Window.CheatsView.ItemDefinitionActionClicked += OnItemDefinitionActionClicked;
            Window.CheatsView.StatusEffectDefinitionClicked += OnStatusEffectDefinitionClicked;

            Window.GameControlTabContent.ResetPlayerQuestsButtonClicked += OnResetPlayerQuestsButtonClicked;
            Window.GameControlTabContent.ResetPlayerLevelButtonClicked += OnResetPlayerLevelButtonClicked;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.CheatsView.ItemDefinitionActionClicked -= OnItemDefinitionActionClicked;
            Window.CheatsView.StatusEffectDefinitionClicked -= OnStatusEffectDefinitionClicked;

            Window.GameControlTabContent.ResetPlayerQuestsButtonClicked -= OnResetPlayerQuestsButtonClicked;
            Window.GameControlTabContent.ResetPlayerLevelButtonClicked -= OnResetPlayerLevelButtonClicked;

            _cheatsViewController.Unbind();
        }

        private void OnStatusEffectDefinitionClicked(StatusEffectDefinition statusEffectDefinition)
        {
            if (_entityRepository.Get(Arguments.InventoryEntityId.Value).TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                var statusEffect = new StatusEffect(statusEffectDefinition);
                statusEffectModule.StatusEffects.AddStatusEffect(statusEffect);
            }
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
            _cheatsViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();

            _cheatsViewData.Dispose();
            _equipmentViewData.Dispose();
            _cheatsViewController.Dispose();
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
