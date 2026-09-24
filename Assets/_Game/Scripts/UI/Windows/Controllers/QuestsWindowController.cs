using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestsWindowController : WindowControllerBase<QuestsWindow, QuestsWindowControllerArguments>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly QuestsHudData _questsHudData;

        public QuestsWindowController(
            IGlobalEventBus globalEventBus,
            QuestsHudData questsHudData)
        {
            _globalEventBus = globalEventBus;
            _questsHudData = questsHudData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _questsHudData.SetEntityId(Arguments.QuestModuleEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.QuestInfoClicked += OnQuestInfoClicked;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.QuestInfoClicked -= OnQuestInfoClicked;
        }

        private void OnQuestInfoClicked(string questId)
        {
            var quest = _questsHudData.ActiveQuests.FirstOrDefault(q => q.Definition.Id == questId);
            if (quest == null) return;

            _globalEventBus.Publish(new WindowOpenRequest(WindowId.QuestDescription, new QuestDescriptionWindowControllerArguments(quest)));
        }

        protected override void Redraw()
        {
            Window.Render(_questsHudData);
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
