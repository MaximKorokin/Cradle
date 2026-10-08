using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CheatsViewController : ViewControllerBase<CheatsView, CheatsViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly IPlayerProvider _playerProvider;
        private readonly EntityRepository _entityRepository;
        private readonly ItemStackFactory _itemStackAssembler;

        private IReadOnlyObservableData<EntryRef> _inventoryEntityId;

        public CheatsViewController(
            IGlobalEventBus globalEventBus,
            IPlayerProvider playerProvider,
            EntityRepository entityRepository,
            ItemStackFactory itemStackAssembler)
        {
            _globalEventBus = globalEventBus;
            _playerProvider = playerProvider;
            _entityRepository = entityRepository;
            _itemStackAssembler = itemStackAssembler;
        }

        public void SetInventoryEntityId(IReadOnlyObservableData<EntryRef> inventoryEntityId)
        {
            _inventoryEntityId = inventoryEntityId;
        }

        public override void Initialize(CheatsView view)
        {
            base.Initialize(view);

            View.ItemDefinitionActionClicked += OnItemDefinitionActionClicked;
            View.StatusEffectDefinitionClicked += OnStatusEffectDefinitionClicked;
            View.GameControlTabContent.ResetPlayerQuestsButtonClicked += OnResetPlayerQuestsButtonClicked;
            View.GameControlTabContent.ResetPlayerLevelButtonClicked += OnResetPlayerLevelButtonClicked;
            View.GameControlTabContent.ResetWindowPositionsButtonClicked += OnResetWindowPositionsButtonClicked;
        }

        public override void Dispose()
        {
            View.ItemDefinitionActionClicked -= OnItemDefinitionActionClicked;
            View.StatusEffectDefinitionClicked -= OnStatusEffectDefinitionClicked;
            View.GameControlTabContent.ResetPlayerQuestsButtonClicked -= OnResetPlayerQuestsButtonClicked;
            View.GameControlTabContent.ResetPlayerLevelButtonClicked -= OnResetPlayerLevelButtonClicked;
            View.GameControlTabContent.ResetWindowPositionsButtonClicked -= OnResetWindowPositionsButtonClicked;

            base.Dispose();
        }

        private void OnStatusEffectDefinitionClicked(StatusEffectDefinition statusEffectDefinition)
        {
            if (_entityRepository.TryGet(_inventoryEntityId.Value, out var statusEntity) && statusEntity.TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                statusEffectModule.StatusEffects.AddStatusEffect(new StatusEffect(statusEffectDefinition));
            }
        }

        private void OnItemDefinitionActionClicked(ItemDefinition itemDefinition)
        {
            if (_entityRepository.TryGet(_inventoryEntityId.Value, out var inventoryEntity) && inventoryEntity.TryGetModule<InventoryModule>(out var inventoryModule))
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

        private void OnResetWindowPositionsButtonClicked()
        {
            _globalEventBus.Publish(new WindowPositionsResetRequest());
        }
    }
}
