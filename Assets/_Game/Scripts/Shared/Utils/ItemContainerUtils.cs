using Assets._Game.Scripts.Items;
using System;

namespace Assets._Game.Scripts.Shared.Utils
{
    // todo: convert to non-static class; encapsulate rules
    public static class ItemContainerUtils
    {
        /// <summary>
        /// Move between containers without specifying destination slot. Target container chooses placement.
        /// Returns actually moved amount.
        /// </summary>
        public static int MoveAmount<TFromSlot>(
            IItemContainer<TFromSlot> from,
            TFromSlot fromSlot,
            IItemContainer to,
            int amount)
            where TFromSlot : notnull
        {
            if (amount <= 0) return 0;

            var fromSnapshotNullable = from.Get(fromSlot);
            if (fromSnapshotNullable == null) return 0;
            var fromSnapshot = fromSnapshotNullable.Value;
            if (fromSnapshot.Amount <= 0) return 0;

            int take = Math.Min(amount, fromSnapshot.Amount);

            // Phase 1: remove from source
            int removed = from.RemoveFromSlot(fromSlot, take);
            if (removed <= 0) return 0;

            // Phase 2: add to target
            int added = to.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, removed), AddPolicy.StackThenEmpty);

            // Rollback remainder if target couldn't accept all
            int remainder = removed - added;
            if (remainder > 0)
            {
                // Best-effort rollback: try stack-then-empty back into source (not necessarily same slot)
                int rolledBack = from.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, remainder), AddPolicy.StackThenEmpty);

                // If rollback also couldn't fit (shouldn't happen unless capacity changed),
                // we "lose" items - in production you'd drop to world or keep an overflow buffer.
                // Here we enforce correctness:
                if (rolledBack != remainder)
                    throw new InvalidOperationException($"Rollback failed. Expected {remainder}, rolled back {rolledBack}.");
            }

            return added;
        }

        /// <summary>
        /// Move amount from specific slot to specific slot. If toSlot is null, target container chooses placement.
        /// Returns actually moved amount.
        /// </summary>
        public static int MoveAmount<TFromSlot, TToSlot>(
            IItemContainer<TFromSlot> from,
            TFromSlot fromSlot,
            IItemContainer<TToSlot> to,
            TToSlot toSlot,
            int amount)
            where TFromSlot : notnull
            where TToSlot : notnull
        {
            if (amount <= 0) return 0;

            var fromSnapshotNullable = from.Get(fromSlot);
            if (fromSnapshotNullable == null) return 0;
            var fromSnapshot = fromSnapshotNullable.Value;
            if (fromSnapshot.Amount <= 0) return 0;

            int take = Math.Min(amount, fromSnapshot.Amount);

            // Phase 1: remove from source
            int removed = from.RemoveFromSlot(fromSlot, take);
            if (removed <= 0) return 0;

            // Phase 2: add to target
            int added;
            if (toSlot is null)
            {
                added = to.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, removed), AddPolicy.StackThenEmpty);
            }
            else
            {
                added = to.AddToSlot(toSlot, new(fromSnapshot.Definition, fromSnapshot.InstanceData, removed));
            }

            // Rollback remainder if target couldn't accept all
            int remainder = removed - added;
            if (remainder > 0)
            {
                // Best-effort rollback: try stack-then-empty back into source (not necessarily same slot)
                int rolledBack = from.Add(new(fromSnapshot.Definition, fromSnapshot.InstanceData, remainder), AddPolicy.StackThenEmpty);

                // If rollback also couldn't fit (shouldn't happen unless capacity changed),
                // we "lose" items - in production you'd drop to world or keep an overflow buffer.
                // Here we enforce correctness:
                if (rolledBack != remainder)
                    throw new InvalidOperationException($"Rollback failed. Expected {remainder}, rolled back {rolledBack}.");
            }

            return added;
        }

        /// <summary>Move between containers without specifying destination slot.</summary>
        public static int MoveAmount<TFromSlot, TToSlot>(
            IItemContainer<TFromSlot> from,
            TFromSlot fromSlot,
            IItemContainer<TToSlot> to,
            int amount)
            where TFromSlot : notnull
            where TToSlot : notnull
        {
            return MoveAmount(from, fromSlot, to, toSlot: default, amount: amount);
        }

        /// <summary>Move within same container (drag-drop), optional destination slot.</summary>
        public static int MoveAmount<TSlot>(
            IItemContainer<TSlot> container,
            TSlot fromSlot,
            TSlot toSlot,
            int amount)
            where TSlot : notnull
        {
            return MoveAmount(container, fromSlot, container, toSlot, amount);
        }

        /// <summary>
        /// Returns actually removed amount.
        /// </summary>
        public static int RemoveAmount<TSlot>(
            IItemContainer<TSlot> container,
            TSlot fromSlot,
            int amount)
            where TSlot : notnull
        {
            if (container.Get(fromSlot) == null) return 0;
            return container.RemoveFromSlot(fromSlot, amount);
        }

        public static bool Has(this IItemContainer container, ItemKey key, int amount)
        {
            if (amount <= 0) return true;
            return container.Count(key) >= amount;
        }
    }
}
