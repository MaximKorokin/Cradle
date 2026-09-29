using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : SingleViewWindowControllerBase<CheatsWindow, CheatsWindowControllerArguments, CheatsView, CheatsViewData, CheatsViewController>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly IPlayerProvider _playerProvider;
        private readonly EntityRepository _entityRepository;
        private readonly EquipmentViewData _equipmentViewData;
        private readonly ItemStackFactory _itemStackAssembler;

        public CheatsWindowController(
            CheatsViewController cheatsViewController,
            CheatsViewData cheatsViewData,
            IGlobalEventBus globalEventBus,
            IPlayerProvider playerProvider,
            EntityRepository entityRepository,
            EquipmentViewData equipmentViewData,
            ItemStackFactory itemStackAssembler) : base(cheatsViewController, cheatsViewData)
        {
            _globalEventBus = globalEventBus;
            _playerProvider = playerProvider;
            _entityRepository = entityRepository;
            _equipmentViewData = equipmentViewData;
            _itemStackAssembler = itemStackAssembler;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.CheatsView.ItemDefinitionActionClicked += OnItemDefinitionActionClicked;
            Window.CheatsView.StatusEffectDefinitionClicked += OnStatusEffectDefinitionClicked;

            Window.GameControlTabContent.ResetPlayerQuestsButtonClicked += OnResetPlayerQuestsButtonClicked;
            Window.GameControlTabContent.ResetPlayerLevelButtonClicked += OnResetPlayerLevelButtonClicked;
        }

        protected override void OnUnbind()
        {
            Window.CheatsView.ItemDefinitionActionClicked -= OnItemDefinitionActionClicked;
            Window.CheatsView.StatusEffectDefinitionClicked -= OnStatusEffectDefinitionClicked;

            Window.GameControlTabContent.ResetPlayerQuestsButtonClicked -= OnResetPlayerQuestsButtonClicked;
            Window.GameControlTabContent.ResetPlayerLevelButtonClicked -= OnResetPlayerLevelButtonClicked;

            base.OnUnbind();
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

        protected override CheatsView GetView() => Window.CheatsView;

        public override void Dispose()
        {
            base.Dispose();

            _equipmentViewData.Dispose();
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
