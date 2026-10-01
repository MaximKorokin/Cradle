using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestGiverWindowController : SingleViewWindowControllerBase<QuestGiverWindow, QuestGiverWindowControllerArguments, QuestGiverView, QuestGiverViewData, QuestGiverViewController>
    {
        private readonly QuestGiverViewData _questGiverViewData;

        public QuestGiverWindowController(
            QuestGiverViewController questGiverViewController,
            QuestGiverViewData questGiverViewData) : base(questGiverViewController, questGiverViewData)
        {
            _questGiverViewData = questGiverViewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _questGiverViewData.SetEntityId(Arguments.GiverEntityId);
            _questGiverViewData.SetTargetEntity(Arguments.TargetEntityId);
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
