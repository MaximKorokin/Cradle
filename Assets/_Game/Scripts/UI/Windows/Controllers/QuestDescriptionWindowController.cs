using Assets._Game.Scripts.Quests;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestDescriptionWindowController : WindowControllerBase<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments>
    {
        private QuestDescriptionWindow _window;
        private QuestDefinition _quest;

        public override void Initialize(QuestDescriptionWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _quest = arguments.Quest;
        }

        public override void Bind(QuestDescriptionWindow window)
        {
            _window = window;

            Redraw();
        }

        public override void Unbind()
        {
            if (_window != null)
            {
                _window = null;
            }
        }

        private void Redraw()
        {
            if (_window == null) return;
            _window.Render(_quest);
        }
    }

    public readonly struct QuestDescriptionWindowControllerArguments : IWindowControllerArguments
    {
        public QuestDefinition Quest { get; }

        public QuestDescriptionWindowControllerArguments(QuestDefinition quest)
        {
            Quest = quest;
        }
    }
}
