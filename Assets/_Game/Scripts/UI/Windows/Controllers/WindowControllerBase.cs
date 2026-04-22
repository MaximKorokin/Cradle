using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public abstract class WindowControllerBase<TWindow, TArguments> : IWindowController<TWindow, TArguments>
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        public abstract void Bind(TWindow window);
        public virtual void Unbind() { }
        public virtual void Initialize(TArguments arguments) { }

        public virtual void Dispose()
        {
            Unbind();
        }
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

    public readonly struct ItemStacksPreviewWindowControllerArguments : IWindowControllerArguments
    {
        public readonly EquipmentSlotKey? EquipmentSlot;
        public readonly long PrimaryContainerSlot;
        public readonly ItemContainerId PrimaryContainerId;
        public readonly ItemContainerId SecondaryContainerId;

        public ItemStacksPreviewWindowControllerArguments(
            EquipmentSlotKey? equipmentSlot,
            long primaryContainerSlot,
            ItemContainerId primaryContainerId,
            ItemContainerId secondaryContainerId)
        {
            EquipmentSlot = equipmentSlot;
            PrimaryContainerSlot = primaryContainerSlot;
            PrimaryContainerId = primaryContainerId;
            SecondaryContainerId = secondaryContainerId;
        }
    }

    public readonly struct AmountPickerWindowControllerArguments : IWindowControllerArguments
    {
        public readonly int MinAmount;
        public readonly int MaxAmount;
        public readonly Action<int> OnAmountPickedCallback;

        public AmountPickerWindowControllerArguments(int minAmount, int maxAmount, Action<int> onAmountPicked)
        {
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            OnAmountPickedCallback = onAmountPicked;
        }
    }
}
