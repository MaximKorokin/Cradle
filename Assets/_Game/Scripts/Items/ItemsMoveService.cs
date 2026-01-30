using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    public class ItemsMoveService : MonoBehaviour
    {
        // ai generated; seems cursed
        public bool Move<T1, T2>(IItemContainer<T1> sourceContainer, T1 sourceSlot, IItemContainer<T2> targetContainer, T2 targetSlot, int amount)
        {
            var sourceItem = sourceContainer.Get(sourceSlot);
            if (sourceItem.Definition == null || amount <= 0 || sourceItem.Amount < amount)
                return false;

            var part = new ItemStack(sourceItem.Definition, sourceItem.Instance, amount);

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

            sourceContainer.Take(sourceSlot, ref amount);
            return true;
        }

        public bool Swap<T1, T2>(IItemContainer<T1> firstContainer, T1 firstSlot, IItemContainer<T2> secondContainer, T2 secondSlot)
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
