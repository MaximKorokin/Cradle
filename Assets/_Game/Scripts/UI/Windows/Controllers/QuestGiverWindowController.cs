using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestGiverWindowController : SingleViewWindowControllerBase<QuestGiverWindow, QuestGiverWindowControllerArguments, QuestGiverView, QuestGiverViewData, QuestGiverViewController>
    {
        public QuestGiverWindowController(
            QuestGiverViewController questGiverViewController,
            QuestGiverViewData questGiverViewData) : base(questGiverViewController, questGiverViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.GiverEntityId);
            ViewData.SetTargetEntity(Arguments.TargetEntityId);
        }

        protected override QuestGiverView GetView() => Window.QuestGiverView;
    }

    public readonly struct QuestGiverWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> GiverEntityId { get; }
        public IReadOnlyObservableData<string> TargetEntityId { get; }

        public QuestGiverWindowControllerArguments(IReadOnlyObservableData<string> giverEntityId, IReadOnlyObservableData<string> targetEntityId)
        {
            GiverEntityId = giverEntityId;
            TargetEntityId = targetEntityId;
        }
    }
}
