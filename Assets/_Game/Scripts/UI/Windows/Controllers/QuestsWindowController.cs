using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Views.Controllers;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestsWindowController : WindowControllerBase<QuestsWindow, QuestsWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly QuestsViewData _questsViewData;
        private readonly QuestsViewController _questsViewController;

        public QuestsWindowController(
            IGlobalEventBus globalEventBus,
            QuestsViewData questsViewData,
            QuestsViewController questsViewController)
        {
            _globalEventBus = globalEventBus;
            _questsViewData = questsViewData;
            _questsViewController = questsViewController;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _questsViewData.SetEntityId(Arguments.QuestModuleEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _questsViewController.Initialize(Window.QuestsView);
            _questsViewController.Bind(_questsViewData);
            Window.QuestsView.QuestInfoClicked += OnQuestInfoClicked;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.QuestsView.QuestInfoClicked -= OnQuestInfoClicked;
            _questsViewController.Unbind();
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = _questsViewData.ActiveQuests.FirstOrDefault(q => q.Definition.Id == questId);
            if (quest == null) return;

            _globalEventBus.Publish(new WindowOpenRequest(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(quest)));
        }

        protected override void Redraw()
        {
            _questsViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();

            _questsViewData.Dispose();
            _questsViewController.Dispose();
        }
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
