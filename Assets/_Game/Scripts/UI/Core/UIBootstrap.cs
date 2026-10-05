using Assets._Game.Scripts.UI.Views.Controllers;
using System;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable, IDisposable
    {
        private readonly WindowsInitializerService _windowsInitializerService;
        private readonly PlayerAiToggleViewController _playerAiToggleViewController;

        public UIBootstrap(
            WindowsInitializerService windowsInitializerService,
            PlayerAiToggleViewController playerAiToggleViewController)
        {
            _windowsInitializerService = windowsInitializerService;
            _playerAiToggleViewController = playerAiToggleViewController;
        }

        public void Start()
        {
            _windowsInitializerService.InitializeWindows();
            _playerAiToggleViewController.Render();
        }

        public void Dispose()
        {
            _playerAiToggleViewController.Dispose();
        }
    }
}
