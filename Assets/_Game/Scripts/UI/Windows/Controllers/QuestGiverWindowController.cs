using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestGiverWindowController : WindowControllerBase<QuestGiverWindow, QuestGiverWindowControllerArguments>
    {
        private readonly QuestGiverHudData _questGiverHudData;
        private readonly EntityRepository _entityRepository;
        private readonly WindowManager _windowManager;

        private string _targetEntityId;

        public QuestGiverWindowController(
            QuestGiverHudData questGiverHudData,
            EntityRepository entityRepository,
            WindowManager windowManager)
        {
            _questGiverHudData = questGiverHudData;
            _entityRepository = entityRepository;
            _windowManager = windowManager;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _targetEntityId = Arguments.TargetEntityId.Value;
            _questGiverHudData.SetEntityId(Arguments.GiverEntityId);
            _questGiverHudData.SetTargetEntity(Arguments.TargetEntityId);
            _questGiverHudData.Changed += Redraw;
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.QuestInfoClicked += OnQuestInfoClicked;
            Window.QuestAcceptClicked += OnQuestAcceptClicked;
            Window.QuestCompleteClicked += OnQuestCompleteClicked;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _questGiverHudData.Changed -= Redraw;

            Window.QuestInfoClicked -= OnQuestInfoClicked;
            Window.QuestAcceptClicked -= OnQuestAcceptClicked;
            Window.QuestCompleteClicked -= OnQuestCompleteClicked;
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = _questGiverHudData.OfferedQuests.FindById(questId);
            if (quest == null) return;

            var questState = _questGiverHudData.IsQuestAccepted(questId) ? _questGiverHudData.GetQuestState(questId) : new(quest);

            _windowManager.InstantiateWindow(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(questState));
        }

        private void OnQuestAcceptClicked(string questId)
        {
            if (_questGiverHudData.IsQuestAccepted(questId)) return;

            var quest = _questGiverHudData.OfferedQuests.FindById(questId);
            if (quest == null) return;

            var targetEntity = _entityRepository.Get(_targetEntityId);
            targetEntity.Publish(new QuestAddRequest(new QuestState(quest)));
        }

        private void OnQuestCompleteClicked(string questId)
        {
            if (!_questGiverHudData.IsQuestAccepted(questId) || !_questGiverHudData.CanCompleteQuest(questId)) return;

            var targetEntity = _entityRepository.Get(_targetEntityId);
            targetEntity.Publish(new QuestCompleteRequest(_questGiverHudData.GetQuestState(questId)));
        }

        public override void Dispose()
        {
            base.Dispose();

            _questGiverHudData.Dispose();
        }

        protected override void Redraw()
        {
            Window.Render(_questGiverHudData);
        }
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
