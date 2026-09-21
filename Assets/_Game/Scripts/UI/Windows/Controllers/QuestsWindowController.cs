using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestsWindowController : WindowControllerBase<QuestsWindow, QuestsWindowControllerArguments>
    {
        private readonly QuestsHudData _questsHudData;
        private readonly WindowManager _windowManager;

        public QuestsWindowController(
            QuestsHudData questsHudData,
            WindowManager windowManager)
        {
            _questsHudData = questsHudData;
            _windowManager = windowManager;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _questsHudData.SetEntityId(Arguments.QuestModuleEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.QuestInfoClicked += OnQuestInfoClicked;

            Redraw();
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            if (Window != null)
            {
                Window.QuestInfoClicked -= OnQuestInfoClicked;
            }
        }

        private void Redraw()
        {
            Window.Render(_questsHudData);
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = _questsHudData.ActiveQuests.FirstOrDefault(q => q.Definition.Id == questId);
            if (quest == null) return;

            _windowManager.InstantiateWindow(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(quest));
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
