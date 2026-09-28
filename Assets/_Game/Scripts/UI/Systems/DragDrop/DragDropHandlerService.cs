using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Widgets;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public class DragDropHandlerService
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;

        public DragDropHandlerService(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
        }

        public void Handle(IDragDropSource source, IDragDropTarget target, PointerContext pointerContext)
        {
            if (source is InventorySlotWidget sourceInventorySlot)
            {
                if (target is InventorySlotWidget targetInventorySlot)
                {
                    HandleInventorySlotToInventorySlotDrop(sourceInventorySlot, targetInventorySlot);
                }
                else if (target is DropArea dropArea)
                {
                    HandleInventorySlotToDropAreaDrop(sourceInventorySlot, dropArea);
                }
                else if (target is ShopView shopView)
                {
                    HandleInventorySlotToShopViewDrop(sourceInventorySlot, shopView);
                }
                else if (!pointerContext.IsOverUI)
                {
                    HandleInventorySlotToNonUIDrop(sourceInventorySlot);
                }
            }
            else if (source is ShopSlotWidget shopSlot)
            {
                if (target is InventorySlotWidget inventorySlot)
                {
                    HandleShopSlotToInventorySlotDrop(shopSlot, inventorySlot);
                }
            }
        }

        private void HandleInventorySlotToInventorySlotDrop(InventorySlotWidget inventorySlot1, InventorySlotWidget inventorySlot2)
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

        private void HandleInventorySlotToNonUIDrop(InventorySlotWidget inventorySlot)
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

        private void HandleInventorySlotToDropAreaDrop(InventorySlotWidget inventorySlot, DropArea dropArea)
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

        private void HandleInventorySlotToShopViewDrop(InventorySlotWidget inventorySlot, ShopView shopView)
        {
            if (!TryGetContainerAndItemStack(inventorySlot, out var container, out var itemStack) ||
                !itemStack.Value.Definition.TryGetSellPrice(shopView.Data.SellCoefficient, out var sellPricePerUnit)) return;

            WindowUtils.ShowAmountPickerThenConfirmation(
                _globalEventBus,
                itemStack.Value.Amount,
                itemStack.Value.Amount,
                "Confirm Sale",
                amount =>
                {
                    int totalPrice = sellPricePerUnit * amount;
                    return $"Sell {amount}x {itemStack.Value.Definition.Name} for {totalPrice}g?";
                },
                amount =>
                {
                    int totalPrice = sellPricePerUnit * amount;
                    PublishCommand(new SellToShopCommand(
                        shopView.Data.ContainerPath,
                        shopView.Data.BuyerInventoryPath,
                        inventorySlot.ContainerPath,
                        inventorySlot.SlotIndex,
                        amount,
                        totalPrice));
                });
        }

        private void HandleShopSlotToInventorySlotDrop(ShopSlotWidget shopSlot, InventorySlotWidget inventorySlot)
        {
            if (!TryGetContainerAndItemStack(shopSlot, out var shop, out var shopItemStack) ||
                shop is not ShopModel shopModel ||
                shopSlot.ShopView == null ||
                !shopModel.TryGetBuyPrice(ShopSlot.FromInt64(shopSlot.SlotIndex), shopSlot.ShopView.Data.BuyCoefficient, shopSlot.ShopView.Data.SellCoefficient, out var buyPricePerUnit)) return;
            
            // Allow to buy to empty slot
            if (TryGetContainerAndItemStack(inventorySlot, out var container, out var itemStack) && itemStack != null) return;

            WindowUtils.ShowAmountPickerThenConfirmation(
                _globalEventBus,
                shopItemStack.Value.Amount,
                shopItemStack.Value.Amount,
                "Confirm Purchase",
                amount =>
                {
                    int totalPrice = buyPricePerUnit * amount;
                    return $"Buy {amount}x {shopItemStack.Value.Definition.Name} for {totalPrice}g?";
                },
                amount =>
                {
                    int totalPrice = buyPricePerUnit * amount;
                    PublishCommand(new BuyFromShopCommand(
                        shopSlot.ContainerPath,
                        shopSlot.ShopView.Data.BuyerInventoryPath,
                        inventorySlot.ContainerPath,
                        shopSlot.SlotIndex,
                        inventorySlot.SlotIndex,
                        amount,
                        totalPrice
                        ));
                });
        }

        private bool TryGetContainerAndItemStack(ContainerSlotWidget containerSlot, out IItemContainer container, out ItemStackSnapshot? itemStack)
        {
            container = null;
            itemStack = null;

            var resolvedContainer = _itemContainerResolver.ResolveContainer(containerSlot.ContainerPath);
            var resolvedItemStack = resolvedContainer.Get(containerSlot.SlotIndex);

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
