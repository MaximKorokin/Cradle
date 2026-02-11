using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class WindowControllerBase<TWindow, TArguments> : IWindowController<TWindow, TArguments>
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        public abstract void Bind(TWindow window);
        public virtual void Initialize(TArguments arguments) { }

        public abstract void Dispose();
    }

    public interface IWindowController<TWindow, TArguments> : IDisposable
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        void Bind(TWindow window);
        void Initialize(TArguments arguments);
    }

    public interface IWindowControllerArguments { }

    public readonly struct EmptyWindowControllerArguments : IWindowControllerArguments { }
    public readonly struct ItemStacksPreviewWindowControllerArguments<T1, T2> : IWindowControllerArguments
    {
        public readonly EquipmentModel EquipmentModel;
        public readonly EquipmentSlotKey? EquipmentSlot;
        public readonly IItemContainer<T1> PrimaryItemContainer;
        public readonly T1 PrimaryContainerSlot;
        public readonly IItemContainer<T2> SecondaryItemContainer;

        public ItemStacksPreviewWindowControllerArguments(EquipmentModel equipmentModel, EquipmentSlotKey? equipmentSlot,  IItemContainer<T1> primaryItemContainer, T1 primaryContainerSlot, IItemContainer<T2> secondaryItemContainer)
        {
            EquipmentModel = equipmentModel;
            EquipmentSlot = equipmentSlot;
            PrimaryItemContainer = primaryItemContainer;
            PrimaryContainerSlot = primaryContainerSlot;
            SecondaryItemContainer = secondaryItemContainer;
        }
    }
}
