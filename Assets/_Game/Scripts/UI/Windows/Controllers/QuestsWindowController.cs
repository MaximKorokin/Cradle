namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class QuestsWindowController : WindowControllerBase<QuestsWindow, EmptyWindowControllerArguments>
    {
        private QuestsWindow _window;

        public override void Bind(QuestsWindow window)
        {
            _window = window;
        }
    }
}
