using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class WindowDefinition
    {
        [field: SerializeField]
        public WindowId Id { get; private set; }
        [field: SerializeField]
        public Type WindowType { get; private set; }
        [field: SerializeField]
        private Type ControllerType { get; set; }

        public WindowDefinition(WindowId id, Type windowType, Type controllerType)
        {
            Id = id;
            WindowType = windowType;
            ControllerType = controllerType;
        }

        public Type GetControllerType(params Type[] genericTypes)
        {
            if (ControllerType.IsGenericType && genericTypes.Length > 0)
                return ControllerType.MakeGenericType(genericTypes);
            return ControllerType;
        }
    }

    public enum WindowId
    {
        None = 0,
        Cheats = 10,
        Stats = 20,
        InventoryInventory = 30,
        InvestoryEquipment = 31,
        //ItemStacksPreview = 40,
        Pause = 50,
    }
}
