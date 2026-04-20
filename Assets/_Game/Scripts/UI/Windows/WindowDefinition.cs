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
            // Choose the generic route only if generic types are provided
            if (ControllerType.IsGenericType && genericTypes.Length > 0)
            {
                var genericArgCount = ControllerType.GetGenericArguments().Length;
                if (genericTypes.Length != genericArgCount)
                {
                    throw new InvalidOperationException(
                        $"Controller {ControllerType.Name} requires {genericArgCount} generic arguments but {genericTypes.Length} were provided.");
                }
                return ControllerType.MakeGenericType(genericTypes);
            }
            return ControllerType;
        }
    }

    public enum WindowId
    {
        None = 0,
        Cheats = 10,
        Stats = 20,
        InventoryInventory = 30,
        InventoryEquipment = 31,
        //ItemStacksPreview = 40,
        LocationTransitionList = 50,

        ItemUseSettings = 1100,

        AmountPicker = 2100,
    }
}
