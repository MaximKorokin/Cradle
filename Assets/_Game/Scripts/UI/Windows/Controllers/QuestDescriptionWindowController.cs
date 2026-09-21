using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.DataFormatters;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestDescriptionWindowController : WindowControllerBase<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments>
    {
        private QuestState _quest;

        private readonly QuestStateFormatter _questStateFormatter;

        public QuestDescriptionWindowController(QuestStateFormatter questStateFormatter)
        {
            _questStateFormatter = questStateFormatter;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _quest = Arguments.Quest;
            _quest.Updated += OnQuestUpdated;
        }

        private void OnQuestUpdated(QuestState quest)
        {
            Redraw();
        }

        protected override void Redraw()
        {
            Window.Render(_questStateFormatter.FormatData(_quest));
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
