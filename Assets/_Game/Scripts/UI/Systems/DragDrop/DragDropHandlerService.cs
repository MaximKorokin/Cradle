using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Widgets;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public class DragDropHandlerService
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly WindowControllerArgumentsProvider _windowControllerArgumentsProvider;

        public DragDropHandlerService(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver,
            WindowControllerArgumentsProvider windowControllerArgumentsProvider)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
            _windowControllerArgumentsProvider = windowControllerArgumentsProvider;
        }

        public void Handle(IDragDropSource source, IDragDropTarget target, PointerContext pointerContext)
        {
            if (source is ItemStackWidget sourceItemStackWidget && sourceItemStackWidget.CanStartDrag())
            {
                if (sourceItemStackWidget.Type == ItemStackWidgetType.Shop &&
                    sourceItemStackWidget.TryGetComponentInParent<ShopSlotWidget>(out var shopSlotWidget))
                {
                    if (target is ItemStackWidget targetItemStackWidget &&
                        (targetItemStackWidget.Type == ItemStackWidgetType.Inventory || targetItemStackWidget.Type == ItemStackWidgetType.Equipment))
                    {
                        HandleShopSlotToItemSlotDrop(shopSlotWidget, targetItemStackWidget);
                    }
                }
                else if (target is ItemStackWidget targetItemStackWidget)
                {
                    if (targetItemStackWidget.Type == ItemStackWidgetType.Reference)
                    {
                        HandleItemSlotToReferenceSlotDrop(sourceItemStackWidget, targetItemStackWidget);
                    }
                    else
                    {
                        HandleItemSlotToItemSlotDrop(sourceItemStackWidget, targetItemStackWidget);
                    }
                }
                else if (target is DropArea dropArea)
                {
                    HandleItemSlotToDropAreaDrop(sourceItemStackWidget, dropArea);
                }
                else if (target is ShopView shopView)
                {
                    HandleItemSlotToShopViewDrop(sourceItemStackWidget, shopView);
                }
                else if (!pointerContext.IsOverUI)
                {
                    HandleItemSlotToNonUIDrop(sourceItemStackWidget);
                }
            }
        }

        private void HandleItemSlotToItemSlotDrop(ItemStackWidget itemSlot1, ItemStackWidget itemSlot2)
        {
            if (!TryGetContainerAndItemStack(itemSlot1, out var container1, out var itemStack1)) return;

            if (itemSlot1.ContainerPath == itemSlot2.ContainerPath)
            {
                PublishCommand(new TransferToContainerSlotCommand(
                    itemSlot1.ContainerPath,
                    itemSlot1.GetItemSlotIndex(),
                    itemSlot2.ContainerPath,
                    itemSlot2.SlotIndex,
                    itemStack1.Value.Amount));
            }
            else
            {
                WindowUtils.ShowAmountPickerIfNeeded(_globalEventBus, itemStack1.Value.Amount, itemStack1.Value.Amount, (selectedAmount) =>
                {
                    PublishCommand(new TransferToContainerSlotCommand(
                        itemSlot1.ContainerPath,
                        itemSlot1.GetItemSlotIndex(),
                        itemSlot2.ContainerPath,
                        itemSlot2.SlotIndex,
                        selectedAmount));
                });
            }
        }

        private void HandleItemSlotToReferenceSlotDrop(ItemStackWidget itemSlot1, ItemStackWidget itemSlot2)
        {
            if (!TryGetContainerAndItemStack(itemSlot1, out var _, out var itemStack) || itemSlot2.Type != ItemStackWidgetType.Reference) return;

            itemSlot2.Bind(itemSlot1.ItemStackPath);
            itemSlot2.Render(itemStack);
        }

        private void HandleItemSlotToNonUIDrop(ItemStackWidget itemSlot)
        {
            if (!TryGetContainerAndItemStack(itemSlot, out var container, out var itemStack)) return;

            WindowUtils.ShowConfirmationOrAmountPicker(_globalEventBus, itemStack.Value.Amount, itemStack.Value.Amount, "Drop Item", $"Are you sure you want to drop {itemStack.Value.Definition.Name}?", (selectedAmount) =>
            {
                PublishCommand(new DropItemCommand(
                    itemSlot.ContainerPath,
                    itemSlot.GetItemSlotIndex(),
                    selectedAmount));
            });
        }

        private void HandleItemSlotToDropAreaDrop(ItemStackWidget itemSlot, DropArea dropArea)
        {
            if (!TryGetContainerAndItemStack(itemSlot, out var container, out var itemStack)) return;

            switch (dropArea.Type)
            {
                case DropAreaType.ItemDestroy:
                    WindowUtils.ShowConfirmationOrAmountPicker(_globalEventBus, itemStack.Value.Amount, itemStack.Value.Amount, "Destroy Item", $"Are you sure you want to destroy {itemStack.Value.Definition.Name}?", (selectedAmount) =>
                    {
                        PublishCommand(new DestroyItemCommand(
                            itemSlot.ContainerPath,
                            itemSlot.GetItemSlotIndex(),
                            selectedAmount));
                    });
                    break;
                case DropAreaType.ItemEnchant:
                    if (!itemStack.Value.Definition.TryGetTrait<EnchantableTrait>(out var enchantableTrait) || 
                        !itemStack.Value.InstanceData.TryGet<EnchantInstanceData>(out var enchantInstanceData))
                    {
                        return;
                    }
                    var defaultArguments = (ItemEnchantingWindowControllerArguments)_windowControllerArgumentsProvider.GetPlayerArguments(WindowId.ItemEnchanting);
                    _globalEventBus.Publish(
                        new WindowToggleRequest(WindowId.ItemEnchanting, new ItemEnchantingWindowControllerArguments(defaultArguments.EntityId, default, itemSlot.ItemStackPath)));
                    break;
                default:
                    SLog.Error($"Unhandled drop area type: {dropArea.Type}");
                    break;
            }
        }

        private void HandleItemSlotToShopViewDrop(ItemStackWidget itemSlot, ShopView shopView)
        {
            if (!TryGetContainerAndItemStack(itemSlot, out var container, out var itemStack) ||
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
                        itemSlot.ContainerPath,
                        itemSlot.GetItemSlotIndex(),
                        amount,
                        totalPrice));
                });
        }

        private void HandleShopSlotToItemSlotDrop(ShopSlotWidget shopSlot, ItemStackWidget itemSlot)
        {
            if (!TryGetContainerAndItemStack(shopSlot.SlotView, out var shop, out var shopItemStack) ||
                shop is not ShopModel shopModel ||
                shopSlot.ShopView == null ||
                !shopModel.TryGetBuyPrice(ShopSlot.FromInt64(shopSlot.SlotView.SlotIndex), shopSlot.ShopView.Data.BuyCoefficient, shopSlot.ShopView.Data.SellCoefficient, out var buyPricePerUnit))
            {
                return;
            }

            // Allow to buy to empty slot
            if (TryGetContainerAndItemStack(itemSlot, out var container, out var itemStack) && itemStack != null)
            {
                return;
            }

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
                        shopSlot.SlotView.ContainerPath,
                        shopSlot.ShopView.Data.BuyerInventoryPath,
                        itemSlot.ContainerPath,
                        shopSlot.SlotView.SlotIndex,
                        itemSlot.SlotIndex,
                        amount,
                        totalPrice
                        ));
                });
        }

        private bool TryGetContainerAndItemStack(ItemStackWidget containerSlot, out IItemContainer container, out ItemStackSnapshot? itemStack)
        {
            container = null;
            itemStack = null;

            if (!_itemContainerResolver.TryResolveContainer(containerSlot.ContainerPath, out var resolvedContainer))
                return false;

            var resolvedItemStack = resolvedContainer.Get(containerSlot.GetItemSlotIndex());

            if (resolvedItemStack != null)
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
