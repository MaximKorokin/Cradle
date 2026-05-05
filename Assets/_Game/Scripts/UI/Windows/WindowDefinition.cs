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
        public Type ControllerType { get; set; }

        public WindowDefinition(WindowId id, Type windowType, Type controllerType)
        {
            Id = id;
            WindowType = windowType;
            ControllerType = controllerType;
        }
    }

    public enum WindowId
    {
        None = 0,
        Cheats = 10,
        Stats = 20,
        Storage = 30,
        InventoryEquipment = 31,
        ItemStacksPreview = 40,
        LocationTransitionList = 50,
        Crafting = 60,
        Shop = 70,

        ItemUseSettings = 1100,

        AmountPicker = 2100,
    }
}
