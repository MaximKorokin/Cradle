using UnityEngine;

namespace Assets._Game.Scripts.Entities.Items
{
    public class ItemsMoveService : MonoBehaviour
    {
        // ai generated; seems cursed
        public bool Move(IItemContainer sourceContainer, int sourceSlot, IItemContainer targetContainer, int targetSlot, int amount)
        {
            var sourceItem = sourceContainer.Get(sourceSlot);
            if (sourceItem.Definition == null || amount <= 0 || sourceItem.Amount < amount)
                return false;

            var part = new ItemStack(sourceItem.Definition, sourceItem.InstanceData, amount);

            if (!targetContainer.CanPut(targetSlot, part))
                return false;

            var targetItem = targetContainer.Get(targetSlot);

            // stack
            if (targetItem.Definition == sourceItem.Definition)
            {
                targetItem.Amount += amount;
                targetContainer.Put(targetSlot, targetItem);
            }
            // empty
            else if (targetItem.Definition == null)
            {
                targetContainer.Put(targetSlot, part);
            }
            else
            {
                return false;
            }

            sourceContainer.Take(sourceSlot, amount);
            return true;
        }

        public bool Swap(IItemContainer firstContainer, int firstSlot, IItemContainer secondContainer, int secondSlot)
        {
            var firstItem = firstContainer.Get(firstSlot);
            var secondItem = secondContainer.Get(secondSlot);

            if (!firstContainer.CanPut(firstSlot, secondItem) || !secondContainer.CanPut(secondSlot, firstItem))
                return false;

            firstContainer.Put(firstSlot, secondItem);
            secondContainer.Put(secondSlot, firstItem);
            return true;
        }
    }
}
