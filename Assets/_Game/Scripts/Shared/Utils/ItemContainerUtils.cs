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
                sourceContainer.Take(sourceItem);
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
                sourceContainer.Take(sourceItem);
                targetContainer.Put(sourceItem);
                return true;
            } 

            return false;
        }

        public static bool TryMove(IItemContainer sourceContainer, ItemStack sourceItem, IItemContainer targetContainer, ref int amount)
        {
            if (sourceItem == null || amount <= 0 || !sourceContainer.Contains(sourceItem))
                return false;

            if (targetContainer.CanPut(sourceItem))
            {
                sourceContainer.Take(sourceItem);
                targetContainer.Put(sourceItem);
                return true;
            } 

            return false;
        }

        // todo: move to slots
        public static bool TrySwap<T1, T2>(IItemContainer<T1> firstContainer, T1 firstSlot, IItemContainer<T2> secondContainer, T2 secondSlot)
        {
            var firstItem = firstContainer.Get(firstSlot);
            var secondItem = secondContainer.Get(secondSlot);
            return TrySwap(firstContainer, firstItem, secondContainer, secondItem);
        }

        // todo: move to slot
        public static bool TrySwap<T>(IItemContainer<T> firstContainer, T firstSlot, IItemContainer secondContainer, ItemStack secondItem)
        {
            var firstItem = firstContainer.Get(firstSlot);
            return TrySwap(firstContainer, firstItem, secondContainer, secondItem);
        }

        public static bool TrySwap(IItemContainer firstContainer, ItemStack firstItem, IItemContainer secondContainer, ItemStack secondItem)
        {
            // No reason to swap empty slots
            if (firstItem == null && secondItem == null) return false;

            // Invalid state: one of items doesn't belong to their container
            if ((firstItem != null && !firstContainer.Contains(firstItem)) || 
                (secondItem != null && !secondContainer.Contains(secondItem))) return false;

            // Take items out of thier containers
            if (firstItem != null) firstContainer.Take(firstItem);
            if (secondItem != null) secondContainer.Take(secondItem);

            // Put items back if con't swap
            if ((secondItem != null && !firstContainer.CanPut(secondItem)) ||
                (firstItem != null && !secondContainer.CanPut(firstItem)))
            {
                if (firstItem != null) firstContainer.Put(firstItem);
                if (secondItem != null) secondContainer.Put(secondItem);
                return false;
            }

            // Put each item to the opposite container
            if (secondItem != null) firstContainer.Put(secondItem);
            if (firstItem != null) secondContainer.Put(firstItem);
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

        public static bool TryRemove(IItemContainer container, ItemStack itemStack, ref int amount)
        {
            if (container.Contains(itemStack) && amount > 0)
            {
                container.Take(itemStack);
                amount = 0;
                return true;
            }
            return false;
        }
    }
}
