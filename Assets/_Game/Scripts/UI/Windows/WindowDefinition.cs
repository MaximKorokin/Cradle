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
        [field: SerializeField]
        public Type StrategyType { get; private set; }

        public WindowDefinition(WindowId id, Type windowType, Type controllerType, Type strategyType = null)
        {
            Id = id;
            WindowType = windowType;
            ControllerType = controllerType;
            StrategyType = strategyType;
        }
    }

    public enum WindowId
    {
        None = 0,
        Cheats = 10,
        Stats = 20,
        Storage = 30,
        Equipment = 31,
        ItemStacksPreview = 40,
        LocationTransitionList = 50,
        Crafting = 60,
        Shop = 70,
        Quests = 80,
        QuestGiver = 81,
        QuestDescription = 82,

        ItemUseSettings = 1100,

        AmountPicker = 2100,
    }
}
