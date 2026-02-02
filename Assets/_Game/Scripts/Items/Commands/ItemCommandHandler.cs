using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.Shared.Utils;
using System;

namespace Assets._Game.Scripts.Items.Commands
{
    public class ItemCommandHandler
    {
        public bool Handle(IItemCommand cmd)
        {
            return cmd switch
            {
                PickupLootCommand c => HandlePickup(c),
                MoveItemCommand c => HandleMove(c),
                MoveItemToSlotCommand c => HandleMoveToSlot(c),
                EquipFromInventoryCommand c => HandleEquip(c),
                UnequipToInventoryCommand c => HandleUnequip(c),
                _ => throw new NotSupportedException(cmd.GetType().Name),
            };
        }

        private bool HandlePickup(PickupLootCommand c)
        {
            return ItemContainerUtils.TryPut(c.InventoryModel, c.ItemStack);
        }

        private bool HandleMove(MoveItemCommand c)
        {
            return ItemContainerUtils.TryMove(c.From, c.FromSlot, c.To, ref c.Amount);
        }

        private bool HandleMoveToSlot(MoveItemToSlotCommand c)
        {
            return ItemContainerUtils.TryMove(c.From, c.FromSlot, c.To, c.ToSlot, ref c.Amount);
        }

        private bool HandleEquip(EquipFromInventoryCommand c)
        {
            var item = c.InventoryModel.Get(c.InventorySlot);
            return ItemContainerUtils.TrySwap(c.EquipmentModel, item.GetEquipmentSlotType(), c.InventoryModel, c.InventorySlot);
        }

        private bool HandleUnequip(UnequipToInventoryCommand c)
        {
            var itemAmount = c.EquipmentModel.Get(c.SlotType).Amount;
            return ItemContainerUtils.TryMove(c.EquipmentModel, c.SlotType, c.InventoryModel, ref itemAmount);
        }
    }
}
