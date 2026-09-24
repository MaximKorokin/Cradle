using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public class DragDropHandler
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;

        public DragDropHandler(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
        }

        public void Handle(IDragDropSource source, IDragDropTarget target, PointerContext pointerContext)
        {
            if (source is InventorySlotView inventorySlot1)
            {
                if (target is InventorySlotView inventorySlot2)
                {
                    HandleInventorySlotToInventorySlotDrop(inventorySlot1, inventorySlot2);
                }
                else if (target is DropArea dropArea)
                {
                    HandleInventorySlotToDropAreaDrop(inventorySlot1, dropArea);
                }
                else if (!pointerContext.IsOverUI)
                {
                    HandleInventorySlotToNonUIDrop(inventorySlot1);
                }
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
                WindowUtils.ShowAmountPickerIfNeeded(_globalEventBus, itemStack1.Value.Amount, itemStack1.Value.Amount, (selectedAmount) =>
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

        private void HandleInventorySlotToNonUIDrop(InventorySlotView inventorySlot)
        {
            if (!TryGetContainerAndItemStack(inventorySlot, out var container, out var itemStack)) return;

            WindowUtils.ShowConfirmationOrAmountPicker(_globalEventBus, itemStack.Value.Amount, itemStack.Value.Amount, "Drop Item", $"Are you sure you want to drop {itemStack.Value.Definition.Name}?", (selectedAmount) =>
            {
                PublishCommand(new DropItemCommand(
                    inventorySlot.ContainerPath,
                    inventorySlot.SlotIndex,
                    selectedAmount));
            });
        }

        private void HandleInventorySlotToDropAreaDrop(InventorySlotView inventorySlot, DropArea dropArea)
        {
            if (!TryGetContainerAndItemStack(inventorySlot, out var container, out var itemStack)) return;
            switch (dropArea.Type)
            {
                case DropAreaType.ItemDestroy:
                    WindowUtils.ShowConfirmationOrAmountPicker(_globalEventBus, itemStack.Value.Amount, itemStack.Value.Amount, "Destroy Item", $"Are you sure you want to destroy {itemStack.Value.Definition.Name}?", (selectedAmount) =>
                    {
                        PublishCommand(new DestroyItemCommand(
                            inventorySlot.ContainerPath,
                            inventorySlot.SlotIndex,
                            selectedAmount));
                    });
                    break;
                case DropAreaType.ItemEnchant:
                    var enchantableTrait = itemStack.Value.GetTrait<EnchantableTrait>();
                    if (!itemStack.Value.InstanceData.TryGet<EnchantInstanceData>(out var enchantInstanceData)) return;
                    WindowUtils.ShowConfirmation(_globalEventBus, "Enchant Item", $"Are you sure you want to enchant {itemStack.Value.Definition.Name}?\nChance is {enchantableTrait.ItemEnchantingDefinition.Methods[0].Rules[enchantInstanceData.Level].SuccessChance}", confirmed =>
                    {
                        if (!confirmed) return;
                        PublishCommand(new EnchantItemCommand(
                            inventorySlot.ContainerPath,
                            inventorySlot.SlotIndex));
                    });
                    break;
                default:
                    SLog.Error($"Unhandled drop area type: {dropArea.Type}");
                    break;
            }
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
