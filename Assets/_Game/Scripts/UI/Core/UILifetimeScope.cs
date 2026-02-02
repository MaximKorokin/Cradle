using Assets._Game.Scripts.UI.Windows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Core
{
    public class UILifetimeScope : LifetimeScope
    {
        [Header("Windows")]
        [SerializeField]
        private InventoryEquipmentWindow _inventoryEquipmentWindow;
        [SerializeField]
        private InventoryInventoryWindow _inventoryInventoryWindow;
        [Space]
        [Header("MonoBehaviours")]
        [SerializeField]
        private UIRootReferences _rootReferences;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_inventoryEquipmentWindow);
            builder.RegisterInstance(_inventoryInventoryWindow);
            builder.RegisterInstance(_rootReferences);

            builder.Register<WindowManager>(Lifetime.Scoped);

            builder.RegisterEntryPoint<UIBootstrap>(Lifetime.Scoped);
        }
    }
}
