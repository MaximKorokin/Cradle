using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Items
{
    public interface IItemContainer<TSlot> : IItemContainer where TSlot : notnull
    {
        event Action<TSlot> SlotChanged;

        ItemStackSnapshot? Get(TSlot slot);
        IEnumerable<(TSlot Slot, ItemStackSnapshot? Snapshot)> Enumerate();

        int AddToSlot(TSlot slot, ItemStackSnapshot snapshot);
        int RemoveFromSlot(TSlot slot, int amount);

        bool IsValidSlot(TSlot slot);

        int PreviewAddToSlot(TSlot slot, ItemStackSnapshot snapshot);
    }

    public interface IItemContainer
    {
        event Action Changed;

        int Count(ItemKey key);

        int Add(ItemStackSnapshot snapshot, AddPolicy policy = AddPolicy.StackThenEmpty);
        int Remove(ItemKey key, int amount);
        int PreviewAdd(ItemStackSnapshot snapshot, AddPolicy policy = AddPolicy.StackThenEmpty);
    }

    public enum AddPolicy
    {
        /// <summary>Try to stack into existing compatible stacks first, then into empty slots.</summary>
        StackThenEmpty,

        /// <summary>Only stack into existing compatible stacks (no placing into empty slots).</summary>
        StackOnly,

        /// <summary>Only place into empty slots (do not stack).</summary>
        EmptyOnly
    }
}
