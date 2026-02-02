using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Shared.Utils
{
    // todo: convert to non-static class; encapsulate rules
    public static class ItemContainerUtils
    {
        public static bool TryMove<T1, T2>(IItemContainer<T1> sourceContainer, T1 sourceSlot, IItemContainer<T2> targetContainer, T2 targetSlot, ref int amount)
        {
            var sourceItem = sourceContainer.Get(sourceSlot);
            if (sourceItem == null || amount <= 0)
                return false;

            if (targetContainer.CanPut(targetSlot, sourceItem))
            {
                targetContainer.Put(targetSlot, sourceItem);
                return true;
            } 

            return false;
        }

        public static bool TryMove<T1>(IItemContainer<T1> sourceContainer, T1 sourceSlot, IItemContainer targetContainer, ref int amount)
        {
            var sourceItem = sourceContainer.Get(sourceSlot);
            if (sourceItem == null || amount <= 0)
                return false;

            if (targetContainer.CanPut(sourceItem))
            {
                targetContainer.Put(sourceItem);
                return true;
            } 

            return false;
        }

        public static bool TrySwap<T1, T2>(IItemContainer<T1> firstContainer, T1 firstSlot, IItemContainer<T2> secondContainer, T2 secondSlot)
        {
            var firstItem = firstContainer.Get(firstSlot);
            var secondItem = secondContainer.Get(secondSlot);

            if (!firstContainer.CanPut(firstSlot, secondItem) || !secondContainer.CanPut(secondSlot, firstItem))
                return false;

            firstContainer.Put(firstSlot, secondItem);
            secondContainer.Put(secondSlot, firstItem);
            return true;
        }

        public static bool TryPut(IItemContainer container, ItemStack itemStack)
        {
            if (container.CanPut(itemStack))
            {
                container.Put(itemStack);
                return true;
            }
            return false;
        }
    }
}
