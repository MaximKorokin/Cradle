using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.QuestDescription)]
    public sealed class QuestDescriptionWindowController : SingleViewWindowControllerBase<QuestDescriptionWindow, QuestDescriptionWindowControllerArguments, QuestDescriptionView, QuestDescriptionViewData, QuestDescriptionViewController>
    {
        public QuestDescriptionWindowController(
            QuestDescriptionViewController questDescriptionViewController,
            QuestDescriptionViewData questDescriptionViewData) : base(questDescriptionViewController, questDescriptionViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.EntityId);
            ViewData.SetQuestId(Arguments.QuestDefinitionId);
        }

        protected override QuestDescriptionView GetView() => Window.QuestDescriptionView;
    }

    public readonly struct QuestDescriptionWindowControllerArguments : IWindowControllerArguments
    {
        public readonly IReadOnlyObservableData<EntryRef> EntityId;
        public readonly string QuestDefinitionId;

        public QuestDescriptionWindowControllerArguments(IReadOnlyObservableData<EntryRef> entityId, string questDefinitionId)
        {
            EntityId = entityId;
            QuestDefinitionId = questDefinitionId;
        }
    }
}
