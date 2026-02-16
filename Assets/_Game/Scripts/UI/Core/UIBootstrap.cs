using Assets._Game.Scripts.UI.Views;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        private readonly HudViewController _hudViewController;

        public UIBootstrap(HudViewController hudViewController)
        {
            _hudViewController = hudViewController;
        }

        public void Start()
        {
            _hudViewController.Render();
        }
    }
}
