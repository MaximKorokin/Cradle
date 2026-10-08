using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.QuestGiver)]
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
        public IReadOnlyObservableData<EntryRef> GiverEntityId { get; }
        public IReadOnlyObservableData<EntryRef> TargetEntityId { get; }

        public QuestGiverWindowControllerArguments(IReadOnlyObservableData<EntryRef> giverEntityId, IReadOnlyObservableData<EntryRef> targetEntityId)
        {
            GiverEntityId = giverEntityId;
            TargetEntityId = targetEntityId;
        }
    }
}
