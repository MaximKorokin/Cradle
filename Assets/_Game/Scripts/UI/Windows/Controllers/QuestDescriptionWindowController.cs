using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestDescriptionWindowController : SingleViewWindowControllerBase<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments, QuestDescriptionView, IQuestDescriptionViewData, QuestDescriptionViewController>
    {
        public QuestDescriptionWindowController(
            QuestDescriptionViewController questDescriptionViewController,
            QuestDescriptionViewData questDescriptionViewData) : base(questDescriptionViewController, questDescriptionViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetQuestState(Arguments.Quest);
        }

        protected override QuestDescriptionView GetView() => Window.QuestDescriptionView;
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
