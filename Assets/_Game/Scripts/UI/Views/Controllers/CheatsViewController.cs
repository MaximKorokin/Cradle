using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CheatsViewController : ViewControllerBase<CheatsView, CheatsViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly IPlayerProvider _playerProvider;
        private readonly EntityRepository _entityRepository;
        private readonly ItemStackFactory _itemStackAssembler;

        private IReadOnlyObservableData<string> _inventoryEntityId;

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

        public void SetInventoryEntityId(IReadOnlyObservableData<string> inventoryEntityId)
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
        }

        public override void Dispose()
        {
            View.ItemDefinitionActionClicked -= OnItemDefinitionActionClicked;
            View.StatusEffectDefinitionClicked -= OnStatusEffectDefinitionClicked;
            View.GameControlTabContent.ResetPlayerQuestsButtonClicked -= OnResetPlayerQuestsButtonClicked;
            View.GameControlTabContent.ResetPlayerLevelButtonClicked -= OnResetPlayerLevelButtonClicked;

            base.Dispose();
        }

        private void OnStatusEffectDefinitionClicked(StatusEffectDefinition statusEffectDefinition)
        {
            if (_entityRepository.Get(_inventoryEntityId.Value).TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                statusEffectModule.StatusEffects.AddStatusEffect(new StatusEffect(statusEffectDefinition));
            }
        }

        private void OnItemDefinitionActionClicked(ItemDefinition itemDefinition)
        {
            if (_entityRepository.Get(_inventoryEntityId.Value).TryGetModule<InventoryModule>(out var inventoryModule))
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
    }
}
