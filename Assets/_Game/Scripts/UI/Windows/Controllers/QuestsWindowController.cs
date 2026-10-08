using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.Quests)]
    public sealed class QuestsWindowController : SingleViewWindowControllerBase<QuestsWindow, QuestsWindowControllerArguments, QuestsView, QuestsViewData, QuestsViewController>
    {
        public QuestsWindowController(
            QuestsViewController questsViewController,
            QuestsViewData questsViewData) : base(questsViewController, questsViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.QuestModuleEntityId);
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
