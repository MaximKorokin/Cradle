using Assets._Game.Scripts.UI.Windows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public class UILifetimeScope : LifetimeScope
    {
        [Header("Prefabs")]
        [SerializeField]
        private InventoryEquipmentWindow _inventoryWindow;
        [Space]
        [Header("MonoBehaviours")]
        [SerializeField]
        private UIRootReferences _rootReferences;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_inventoryWindow);
            builder.RegisterInstance(_rootReferences);

            builder.Register<WindowManager>(Lifetime.Scoped);

            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);
        }
    }
}
