using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestGiverWindowController : WindowControllerBase<QuestGiverWindow, QuestGiverWindowControllerArguments>
    {
        private QuestGiverWindow _window;

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

        public override void Initialize(QuestGiverWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _targetEntityId = arguments.TargetEntityId;
            _questGiverHudData.SetEntities(arguments.GiverEntityId, arguments.TargetEntityId);
            _questGiverHudData.Changed += Redraw;
        }

        public override void Bind(QuestGiverWindow window)
        {
            _window = window;
            _window.QuestInfoClicked += OnQuestInfoClicked;
            _window.QuestAcceptClicked += OnQuestAcceptClicked;
            _window.QuestCompleteClicked += OnQuestCompleteClicked;

            Redraw();
        }

        public override void Unbind()
        {
            _questGiverHudData.Changed -= Redraw;

            if (_window != null)
            {
                _window.QuestInfoClicked -= OnQuestInfoClicked;
                _window.QuestAcceptClicked -= OnQuestAcceptClicked;
                _window.QuestCompleteClicked -= OnQuestCompleteClicked;
                _window = null;
            }
        }

        public override void Dispose()
        {
            Unbind();
            _questGiverHudData.Dispose();
        }

        private void Redraw()
        {
            if (_window == null) return;
            _window.Render(_questGiverHudData);
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = _questGiverHudData.OfferedQuests.FindById(questId);
            if (quest == null) return;

            var questState = _questGiverHudData.IsQuestAccepted(questId) ? _questGiverHudData.GetQuestState(questId) : new(quest);

            _windowManager.InstantiateWindow<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments>(new(questState));
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
    }

    public readonly struct QuestGiverWindowControllerArguments : IWindowControllerArguments
    {
        public string GiverEntityId { get; }
        public string TargetEntityId { get; }

        public QuestGiverWindowControllerArguments(string giverEntityId, string targetEntityId)
        {
            GiverEntityId = giverEntityId;
            TargetEntityId = targetEntityId;
        }
    }
}
