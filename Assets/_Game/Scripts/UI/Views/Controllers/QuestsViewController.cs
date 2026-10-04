using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System.Linq;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class QuestsViewController : ViewControllerBase<QuestsView, QuestsViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;

        public QuestsViewController(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public override void Initialize(QuestsView view)
        {
            base.Initialize(view);
            View.QuestInfoClicked += OnQuestInfoClicked;
        }

        public override void Dispose()
        {
            View.QuestInfoClicked -= OnQuestInfoClicked;
            base.Dispose();
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = Data.ActiveQuests.FirstOrDefault(q => q.Definition.Id == questId);
            if (quest == null) return;

            _globalEventBus.Publish(new WindowToggleRequest(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(quest)));
        }
    }
}
