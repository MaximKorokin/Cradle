using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Modal;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public class UILifetimeScope : LifetimeScope
    {
        [Header("Windows")]
        [SerializeField]
        private UIWindow[] _windowPrefabs;
        [SerializeField]
        private ModalWrapper _modalWrapperPrefab;
        [Space]
        [Header("MonoBehaviours")]
        [SerializeField]
        private UIRootReferences _rootReferences;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance((IEnumerable<UIWindow>)_windowPrefabs);
            builder.RegisterInstance(_modalWrapperPrefab);
            builder.RegisterInstance(_rootReferences);

            builder.Register<WindowManager>(Lifetime.Scoped);

            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);
        }
    }
}
