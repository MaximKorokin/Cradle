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
                MoveItemCommand c => HandleMove(c),
                MoveItemToSlotCommand c => HandleMoveToSlot(c),
                EquipFromContainerCommand c => HandleEquip(c),
                UnequipToContainerCommand c => HandleUnequip(c),
                DropItemCommand c => HandleDrop(c),
                _ => throw new NotSupportedException(cmd.GetType().Name),
            };
        }

        private bool HandleMove(MoveItemCommand c)
        {
            var amount = c.Amount;
            return ItemContainerUtils.TryMove(c.FromContainer, c.FromItem, c.ToContainer, ref amount);
        }

        private bool HandleMoveToSlot(MoveItemToSlotCommand c)
        {
            var amount = c.Amount;
            return ItemContainerUtils.TryMove(c.From, c.FromSlot, c.To, c.ToSlot, ref amount);
        }

        private bool HandleEquip(EquipFromContainerCommand c)
        {
            var slotType = c.ItemStack.GetEquipmentSlotType();
            // Try to find empty slot
            if (!c.EquipmentModel.TryGetSlot(slotType, item => item == null, out var key))
            {
                // No empty slots found in equipment, will try to swap the first one
                key = new(slotType, 0);
            }
            return ItemContainerUtils.TrySwap(c.EquipmentModel, key, c.FromContainer, c.ItemStack);
        }

        private bool HandleUnequip(UnequipToContainerCommand c)
        {
            var itemAmount = c.EquipmentModel.Get(c.SlotKey).Amount;
            return ItemContainerUtils.TryMove(c.EquipmentModel, c.SlotKey, c.ToContainer, ref itemAmount);
        }

        private bool HandleDrop(DropItemCommand c)
        {
            var amount = c.Amount;
            return ItemContainerUtils.TryRemove(c.FromContainer, c.Item, ref amount);
        }
    }
}
