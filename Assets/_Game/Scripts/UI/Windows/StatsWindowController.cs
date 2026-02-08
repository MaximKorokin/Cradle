using Assets._Game.Scripts.Entities.Modules;
using System;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class StatsWindowController : IDisposable
    {
        private readonly StatsWindow _window;
        private readonly StatsModule _statsModule;

        public StatsWindowController(
            StatsWindow statsWindow,
            StatsModule statsModule)
        {
            _window = statsWindow;
            _statsModule = statsModule;

            _statsModule.Stats.Changed += Redraw;

            Redraw();
        }

        private void Redraw()
        {
            _window.Render(_statsModule.Stats.Enumerate().Select(s => (s.Id.ToString(), s.Final.ToString())));
        }

        public void Dispose()
        {
            _statsModule.Stats.Changed -= Redraw;
        }
    }
}
