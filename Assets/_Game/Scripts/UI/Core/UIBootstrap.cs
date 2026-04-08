using Assets._Game.Scripts.UI.Views;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        private readonly CompactPlayerStateViewController _compactPlayerStateViewController;

        public UIBootstrap(CompactPlayerStateViewController compactPlayerStateViewController)
        {
            _compactPlayerStateViewController = compactPlayerStateViewController;
        }

        public void Start()
        {
            _compactPlayerStateViewController.Render();
        }
    }
}
