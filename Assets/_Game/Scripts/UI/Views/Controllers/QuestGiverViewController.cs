using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class QuestGiverViewController : ViewControllerBase<QuestGiverView, QuestGiverViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly EntityRepository _entityRepository;

        public QuestGiverViewController(IGlobalEventBus globalEventBus, EntityRepository entityRepository)
        {
            _globalEventBus = globalEventBus;
            _entityRepository = entityRepository;
        }

        public override void Initialize(QuestGiverView view)
        {
            base.Initialize(view);
            View.QuestInfoClicked += OnQuestInfoClicked;
            View.QuestAcceptClicked += OnQuestAcceptClicked;
            View.QuestCompleteClicked += OnQuestCompleteClicked;
        }

        public override void Dispose()
        {
            View.QuestInfoClicked -= OnQuestInfoClicked;
            View.QuestAcceptClicked -= OnQuestAcceptClicked;
            View.QuestCompleteClicked -= OnQuestCompleteClicked;
            base.Dispose();
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = Data.OfferedQuests.FindById(questId);
            if (quest == null) return;

            _globalEventBus.Publish(new WindowToggleRequest(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(null, questId)));
        }

        private void OnQuestAcceptClicked(string questId)
        {
            if (Data.IsQuestAccepted(questId)) return;

            var quest = Data.OfferedQuests.FindById(questId);
            if (quest == null) return;

            _entityRepository.Get(Data.TargetEntityId.Value).Publish(new QuestAddRequest(quest.Id));
        }

        private void OnQuestCompleteClicked(string questId)
        {
            if (!Data.IsQuestAccepted(questId)) return;
            if (!Data.CanCompleteQuest(questId)) return;

            var quest = Data.OfferedQuests.FindById(questId);
            _entityRepository.Get(Data.TargetEntityId.Value).Publish(new QuestCompleteRequest(quest.Id));
        }
    }
}
