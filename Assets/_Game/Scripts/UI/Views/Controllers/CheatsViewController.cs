using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class CheatsViewController : ViewControllerBase<CheatsView, CheatsViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityRepository _entityRepository;
        private readonly ItemStackFactory _itemStackAssembler;

        private IReadOnlyObservableData<EntryRef> _entityId;

        public CheatsViewController(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            ItemStackFactory itemStackAssembler)
        {
            _globalEventBus = globalEventBus;
            _entityRepository = entityRepository;
            _itemStackAssembler = itemStackAssembler;
        }

        public void SetEntityId(IReadOnlyObservableData<EntryRef> entityId)
        {
            _entityId = entityId;
        }

        public override void Initialize(CheatsView view)
        {
            base.Initialize(view);

            View.ItemDefinitionActionClicked += OnItemDefinitionActionClicked;
            View.StatusEffectDefinitionClicked += OnStatusEffectDefinitionClicked;
            View.QuestDefinitionClicked += OnQuestDefinitionClicked;
            View.GameControlTabContent.ResetPlayerQuestsButtonClicked += OnResetPlayerQuestsButtonClicked;
            View.GameControlTabContent.ResetPlayerLevelButtonClicked += OnResetPlayerLevelButtonClicked;
            View.GameControlTabContent.ResetWindowPositionsButtonClicked += OnResetWindowPositionsButtonClicked;
        }

        public override void Dispose()
        {
            View.ItemDefinitionActionClicked -= OnItemDefinitionActionClicked;
            View.StatusEffectDefinitionClicked -= OnStatusEffectDefinitionClicked;
            View.QuestDefinitionClicked -= OnQuestDefinitionClicked;
            View.GameControlTabContent.ResetPlayerQuestsButtonClicked -= OnResetPlayerQuestsButtonClicked;
            View.GameControlTabContent.ResetPlayerLevelButtonClicked -= OnResetPlayerLevelButtonClicked;
            View.GameControlTabContent.ResetWindowPositionsButtonClicked -= OnResetWindowPositionsButtonClicked;

            base.Dispose();
        }

        private void OnStatusEffectDefinitionClicked(StatusEffectDefinition statusEffectDefinition)
        {
            if (_entityRepository.TryGet(_entityId.Value, out var statusEntity))
            {
                _globalEventBus.Publish(new StatusEffectApplyRequest(statusEntity, new StatusEffect(statusEffectDefinition)));
            }
        }

        private void OnItemDefinitionActionClicked(ItemDefinition itemDefinition)
        {
            if (_entityRepository.TryGet(_entityId.Value, out var inventoryEntity) && inventoryEntity.HasModule<InventoryModule>())
            {
                var path = ItemContainerPath.Inventory(inventoryEntity.Id);
                WindowUtils.ShowAmountPickerIfNeeded(_globalEventBus, itemDefinition.MaxAmount, itemDefinition.MaxAmount, amount =>
                {
                    var item = _itemStackAssembler.Create(itemDefinition.Id, amount).Snapshot;
                    _globalEventBus.Publish(new ItemCommandRequest(new AddItemCommand(path, item)));
                });
            }
        }

        private void OnQuestDefinitionClicked(QuestDefinition questDefinition)
        {
            if (!_entityRepository.TryGet(_entityId.Value, out var entity)) return;

            entity.Publish(new QuestAddRequest(questDefinition.Id));
        }

        private void OnResetPlayerQuestsButtonClicked()
        {
            if (_entityRepository.TryGet(_entityId.Value, out var entity))
                _globalEventBus.Publish(new ResetEntityModuleRequest(entity, typeof(QuestModule)));
        }

        private void OnResetPlayerLevelButtonClicked()
        {
            if (_entityRepository.TryGet(_entityId.Value, out var entity))
                _globalEventBus.Publish(new ResetEntityModuleRequest(entity, typeof(LevelingModule)));
        }

        private void OnResetWindowPositionsButtonClicked()
        {
            _globalEventBus.Publish(new WindowPositionsResetRequest());
        }
    }
}
