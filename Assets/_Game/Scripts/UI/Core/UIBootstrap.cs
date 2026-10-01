using Assets._Game.Scripts.UI.Views.Controllers;
using System;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable, IDisposable
    {
        private readonly CompactPlayerStateViewController _compactPlayerStateViewController;
        private readonly PlayerAiToggleViewController _playerAiToggleViewController;

        public UIBootstrap(
            CompactPlayerStateViewController compactPlayerStateViewController,
            PlayerAiToggleViewController playerAiToggleViewController)
        {
            _compactPlayerStateViewController = compactPlayerStateViewController;
            _playerAiToggleViewController = playerAiToggleViewController;
        }

        public void Start()
        {
            _compactPlayerStateViewController.Render();
            _playerAiToggleViewController.Render();
        }

        public void Dispose()
        {
            _compactPlayerStateViewController.Dispose();
            _playerAiToggleViewController.Dispose();
        }
    }
}
