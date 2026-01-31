using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Shared.Utils
{
    public static class ItemsMoveUtils
    {
        public static bool Move<T1, T2>(IItemContainer<T1> sourceContainer, T1 sourceSlot, IItemContainer<T2> targetContainer, T2 targetSlot, ref int amount)
        {
            var sourceItem = sourceContainer.Get(sourceSlot);
            var targetItem = targetContainer.Get(targetSlot);
            if (sourceItem == null || amount <= 0)
                return false;

            if (targetContainer.CanPut(targetSlot, sourceItem))
            {
                targetContainer.Put(targetSlot, targetItem);
                return true;
            } 

            return false;
        }

        public static bool Swap<T1, T2>(IItemContainer<T1> firstContainer, T1 firstSlot, IItemContainer<T2> secondContainer, T2 secondSlot)
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
