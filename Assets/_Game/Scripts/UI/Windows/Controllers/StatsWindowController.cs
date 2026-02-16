using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class StatsWindowController : WindowControllerBase<StatsWindow, EmptyWindowControllerArguments>
    {
        private StatsWindow _window;
        private readonly StatModule _statsModule;

        public StatsWindowController(
            PlayerContext playerContext)
        {
            _statsModule = playerContext.StatsModule;

            _statsModule.Stats.Changed += Redraw;
        }

        public override void Bind(StatsWindow window)
        {
            _window = window;

            Redraw();
        }

        public override void Dispose()
        {
            _statsModule.Stats.Changed -= Redraw;
        }

        private void Redraw()
        {
            _window.Render(_statsModule.Stats.Enumerate().Select(s => (s.Id.ToString(), s.Final.ToString())));
        }
    }
}
