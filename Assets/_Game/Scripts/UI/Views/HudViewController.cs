using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class HudViewController : IDisposable
    {
        private readonly CompactPlayerStateViewController _compactPlayerStateViewController;

        public HudViewController(
            HudView hudView,
            CompactPlayerStateViewController compactPlayerStateViewController)
        {
            _compactPlayerStateViewController = compactPlayerStateViewController;
        }

        public void Render()
        {
            _compactPlayerStateViewController.Render();
        }

        public void Dispose()
        {
        }
    }
}
