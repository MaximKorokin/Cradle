using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestsWindowController : WindowControllerBase<QuestsWindow, QuestsWindowControllerArguments>
    {
        private QuestsWindow _window;

        private readonly QuestsHudData _questsHudData;
        private readonly WindowManager _windowManager;

        public QuestsWindowController(
            QuestsHudData questsHudData,
            WindowManager windowManager)
        {
            _questsHudData = questsHudData;
            _windowManager = windowManager;
        }

        public override void Initialize(QuestsWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _questsHudData.SetEntityId(arguments.QuestModuleEntityId);
        }

        public override void Bind(QuestsWindow window)
        {
            _window = window;
            _window.QuestInfoClicked += OnQuestInfoClicked;

            Redraw();
        }

        public override void Unbind()
        {
            if (_window != null)
            {
                _window.QuestInfoClicked -= OnQuestInfoClicked;
                _window = null;
            }
        }

        private void Redraw()
        {
            _window.Render(_questsHudData);
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = _questsHudData.ActiveQuests.FirstOrDefault(q => q.Definition.Id == questId);
            if (quest == null) return;

            _windowManager.InstantiateWindow<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments>(new(quest));
        }
    }

    public readonly struct QuestsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> QuestModuleEntityId { get; }

        public QuestsWindowControllerArguments(IReadOnlyObservableData<string> questModuleEntityId)
        {
            QuestModuleEntityId = questModuleEntityId;
        }
    }
}
