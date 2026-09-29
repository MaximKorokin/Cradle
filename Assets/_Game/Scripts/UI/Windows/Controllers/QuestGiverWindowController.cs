using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestGiverWindowController : SingleViewWindowControllerBase<QuestGiverWindow, QuestGiverWindowControllerArguments, QuestGiverView, QuestGiverViewData, QuestGiverViewController>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly QuestGiverViewData _questGiverViewData;
        private readonly EntityRepository _entityRepository;

        private string _targetEntityId;

        public QuestGiverWindowController(
            QuestGiverViewController questGiverViewController,
            QuestGiverViewData questGiverViewData,
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository) : base(questGiverViewController, questGiverViewData)
        {
            _globalEventBus = globalEventBus;
            _questGiverViewData = questGiverViewData;
            _entityRepository = entityRepository;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _targetEntityId = Arguments.TargetEntityId.Value;
            _questGiverViewData.SetEntityId(Arguments.GiverEntityId);
            _questGiverViewData.SetTargetEntity(Arguments.TargetEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.QuestGiverView.QuestInfoClicked += OnQuestInfoClicked;
            Window.QuestGiverView.QuestAcceptClicked += OnQuestAcceptClicked;
            Window.QuestGiverView.QuestCompleteClicked += OnQuestCompleteClicked;
        }

        protected override void OnUnbind()
        {
            Window.QuestGiverView.QuestInfoClicked -= OnQuestInfoClicked;
            Window.QuestGiverView.QuestAcceptClicked -= OnQuestAcceptClicked;
            Window.QuestGiverView.QuestCompleteClicked -= OnQuestCompleteClicked;

            base.OnUnbind();
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = _questGiverViewData.OfferedQuests.FindById(questId);
            if (quest == null) return;

            var questState = _questGiverViewData.IsQuestAccepted(questId) ? _questGiverViewData.GetQuestState(questId) : new(quest);

            _globalEventBus.Publish(new WindowOpenRequest(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(questState)));
        }

        private void OnQuestAcceptClicked(string questId)
        {
            if (_questGiverViewData.IsQuestAccepted(questId)) return;

            var quest = _questGiverViewData.OfferedQuests.FindById(questId);
            if (quest == null) return;

            var targetEntity = _entityRepository.Get(_targetEntityId);
            targetEntity.Publish(new QuestAddRequest(new QuestState(quest)));
        }

        private void OnQuestCompleteClicked(string questId)
        {
            if (!_questGiverViewData.IsQuestAccepted(questId)) return;
            if (!_questGiverViewData.CanCompleteQuest(questId)) return;

            var questState = _questGiverViewData.GetQuestState(questId);
            var targetEntity = _entityRepository.Get(_targetEntityId);
            targetEntity.Publish(new QuestCompleteRequest(questState));
        }

        protected override QuestGiverView GetView() => Window.QuestGiverView;
    }

    public readonly struct QuestGiverWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> GiverEntityId { get; }
        public IReadOnlyObservableData<string> TargetEntityId { get; }

        public QuestGiverWindowControllerArguments(IReadOnlyObservableData<string> giverEntityId, IReadOnlyObservableData<string> targetEntityId)
        {
            GiverEntityId = giverEntityId;
            TargetEntityId = targetEntityId;
        }
    }
}
