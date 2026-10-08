using Assets._Game.Scripts.UI.Views.Controllers;
using System;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public sealed class UIBootstrap : IStartable
    {
        private readonly WindowsInitializerService _windowsInitializerService;
        
        public UIBootstrap(
            WindowsInitializerService windowsInitializerService)
        {
            _windowsInitializerService = windowsInitializerService;
        }

        public void Start()
        {
            _windowsInitializerService.InitializeWindows();
        }
    }
}
