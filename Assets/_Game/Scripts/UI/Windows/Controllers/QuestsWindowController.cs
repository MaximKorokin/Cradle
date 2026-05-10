using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestsWindowController : WindowControllerBase<QuestsWindow, EmptyWindowControllerArguments>
    {
        private QuestsWindow _window;

        private readonly QuestsHudData _questsHudData;

        public QuestsWindowController(QuestsHudData questsHudData)
        {
            _questsHudData = questsHudData;
        }

        public override void Bind(QuestsWindow window)
        {
            _window = window;

            Redraw();
        }

        public override void Unbind()
        {

        }

        private void Redraw()
        {
            _window.Render(_questsHudData);
        }
    }
}
