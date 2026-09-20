using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class WindowDefinition
    {
        [field: SerializeField]
        public WindowId Id { get; private set; }
        [field: SerializeField]
        public Type ControllerType { get; set; }
        [field: SerializeField]
        public WindowConfiguration Configuration { get; private set; }

        public WindowDefinition(WindowId id, Type controllerType, WindowConfiguration configuration)
        {
            Id = id;
            ControllerType = controllerType;
            Configuration = configuration;
        }
    }

    public enum WindowId
    {
        None = 0,
        Cheats = 10,
        Stats = 20,
        Storage = 30,
        Equipment = 31,
        Inventory = 32,
        ItemStacksPreview = 40,
        LocationTransitionList = 50,
        Crafting = 60,
        Shop = 70,
        Quests = 80,
        QuestGiver = 81,
        QuestDescription = 82,

        ItemUseSettings = 1100,

        AmountPicker = 2100,
        Confirmation = 2200,
    }

    [Serializable]
    public readonly struct WindowConfiguration
    {
        public bool IsSingleton { get; }
        public bool IsModal { get; }
        public bool CanMove { get; }

        public WindowConfiguration(bool isSingleton, bool isModal, bool canMove)
        {
            IsSingleton = isSingleton;
            IsModal = isModal;
            CanMove = canMove;
        }
    }
}
