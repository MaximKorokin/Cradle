using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public class DragDropHandler
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly WindowManager _windowManager;
        private readonly ItemContainerResolver _itemContainerResolver;

        public DragDropHandler(IGlobalEventBus globalEventBus, WindowManager windowManager, ItemContainerResolver itemContainerResolver)
        {
            _globalEventBus = globalEventBus;
            _windowManager = windowManager;
            _itemContainerResolver = itemContainerResolver;
        }

        public void Handle(IDragDropSource source, IDragDropTarget target, PointerContext pointerContext)
        {
            if (source is InventorySlotView inventorySlot1 && target is InventorySlotView inventorySlot2)
            {
                HandleInventorySlotToInventorySlotDrop(inventorySlot1, inventorySlot2);
            }
            else if (!pointerContext.IsOverUI && source is InventorySlotView inventorySlot)
            {
                HandleInventoryToNonUIDrop(inventorySlot);
            }
        }

        private void HandleInventorySlotToInventorySlotDrop(InventorySlotView inventorySlot1, InventorySlotView inventorySlot2)
        {
            if (!TryGetContainerAndItemStack(inventorySlot1, out var container1, out var itemStack1)) return;

            if (inventorySlot1.ContainerPath == inventorySlot2.ContainerPath)
            {
                PublishCommand(new TransferToContainerSlotCommand(
                    inventorySlot1.ContainerPath,
                    inventorySlot1.SlotIndex,
                    inventorySlot2.ContainerPath,
                    inventorySlot2.SlotIndex,
                    itemStack1.Value.Amount));
            }
            else
            {
                _windowManager.ShowAmountPickerIfNeeded(itemStack1.Value.Amount, itemStack1.Value.Amount, (selectedAmount) =>
                {
                    PublishCommand(new TransferToContainerSlotCommand(
                        inventorySlot1.ContainerPath,
                        inventorySlot1.SlotIndex,
                        inventorySlot2.ContainerPath,
                        inventorySlot2.SlotIndex,
                        selectedAmount));
                });
            }
        }

        private void HandleInventoryToNonUIDrop(InventorySlotView inventorySlot)
        {
            if (!TryGetContainerAndItemStack(inventorySlot, out var container, out var itemStack)) return;

            _windowManager.ShowConfirmationOrAmountPicker(itemStack.Value.Amount, itemStack.Value.Amount, "Drop Item", $"Are you sure you want to drop {itemStack.Value.Definition.Name}?", (selectedAmount) =>
            {
                PublishCommand(new DropItemCommand(
                    inventorySlot.ContainerPath,
                    inventorySlot.SlotIndex,
                    selectedAmount));
            });
        }

        private bool TryGetContainerAndItemStack(InventorySlotView inventorySlot, out IItemContainer container, out ItemStackSnapshot? itemStack)
        {
            container = null;
            itemStack = null;

            var resolvedContainer = _itemContainerResolver.ResolveContainer(inventorySlot.ContainerPath);
            var resolvedItemStack = resolvedContainer.Get(inventorySlot.SlotIndex);

            if (resolvedContainer != null && resolvedItemStack != null)
            {
                container = resolvedContainer;
                itemStack = resolvedItemStack;
                return true;
            }

            return false;
        }

        private void PublishCommand(IItemCommand command)
        {
            _globalEventBus.Publish(new ItemCommandRequest(command));
        }
    }
}
