using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestDescriptionWindowController : WindowControllerBase<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments>
    {
        private readonly QuestDescriptionViewData _questDescriptionViewData;
        private readonly QuestDescriptionViewController _questDescriptionViewController;

        public QuestDescriptionWindowController(
            QuestDescriptionViewData questDescriptionViewData,
            QuestDescriptionViewController questDescriptionViewController)
        {
            _questDescriptionViewData = questDescriptionViewData;
            _questDescriptionViewController = questDescriptionViewController;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _questDescriptionViewData.SetQuestState(Arguments.Quest);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _questDescriptionViewController.Initialize(Window.QuestDescriptionView);
            _questDescriptionViewController.Bind(_questDescriptionViewData);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _questDescriptionViewController.Unbind();
        }

        protected override void Redraw()
        {
            _questDescriptionViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();

            _questDescriptionViewData.Dispose();
            _questDescriptionViewController.Dispose();
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
