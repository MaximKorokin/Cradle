using Assets._Game.Scripts.UI.Views.Controllers;
using System;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable, IDisposable
    {
        private readonly PlayerAiToggleViewController _playerAiToggleViewController;

        public UIBootstrap(
            PlayerAiToggleViewController playerAiToggleViewController)
        {
            _playerAiToggleViewController = playerAiToggleViewController;
        }

        public void Start()
        {
            _playerAiToggleViewController.Render();
        }

        public void Dispose()
        {
            _playerAiToggleViewController.Dispose();
        }
    }
}
