using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestsWindowController : SingleViewWindowControllerBase<QuestsWindow, QuestsWindowControllerArguments, QuestsView, QuestsViewData, QuestsViewController>
    {
        private readonly IGlobalEventBus _globalEventBus;

        public QuestsWindowController(
            QuestsViewController questsViewController,
            QuestsViewData questsViewData,
            IGlobalEventBus globalEventBus) : base(questsViewController, questsViewData)
        {
            _globalEventBus = globalEventBus;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.QuestModuleEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.QuestsView.QuestInfoClicked += OnQuestInfoClicked;
        }

        protected override void OnUnbind()
        {
            Window.QuestsView.QuestInfoClicked -= OnQuestInfoClicked;

            base.OnUnbind();
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = ViewData.ActiveQuests.FirstOrDefault(q => q.Definition.Id == questId);
            if (quest == null) return;

            _globalEventBus.Publish(new WindowOpenRequest(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(quest)));
        }

        protected override QuestsView GetView() => Window.QuestsView;
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
