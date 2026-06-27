using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.DataFormatters;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestDescriptionWindowController : WindowControllerBase<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments>
    {
        private QuestDescriptionWindow _window;
        private QuestState _quest;

        private readonly QuestStateFormatter _questStateFormatter;

        public QuestDescriptionWindowController(QuestStateFormatter questStateFormatter)
        {
            _questStateFormatter = questStateFormatter;
        }

        public override void Initialize(QuestDescriptionWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _quest = arguments.Quest;
            _quest.Updated += OnQuestUpdated;
        }

        private void OnQuestUpdated(QuestState quest)
        {
            Redraw();
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
            _window.Render(_questStateFormatter.FormatData(_quest));
        }

        public override void Dispose()
        {
            base.Dispose();
            _quest.Updated -= OnQuestUpdated;
        }
    }

    public readonly struct QuestDescriptionWindowControllerArguments : IWindowControllerArguments
    {
        public QuestState Quest { get; }

        public QuestDescriptionWindowControllerArguments(QuestState quest)
        {
            Quest = quest;
        }
    }
}
