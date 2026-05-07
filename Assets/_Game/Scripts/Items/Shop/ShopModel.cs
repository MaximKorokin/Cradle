using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Items.Shop
{
    public sealed class ShopModel : IItemContainer<ShopSlot>
    {
        private readonly List<ShopSlotData> _slots;
        private readonly ItemStackFactory _itemStackFactory;

        public int Capacity => _slots.Count;

        public event Action<ShopChange> ShopChanged;
        public event Action<ShopSlot> SlotChanged;
        public event Action Changed;

        public ShopModel(ItemStackFactory itemStackFactory, IEnumerable<ItemDefinition> initialItems = null)
        {
            _slots = new List<ShopSlotData>();
            _itemStackFactory = itemStackFactory;

            // Add initial items (these have infinite stock and always return MaxAmount)
            if (initialItems != null)
            {
                foreach (var itemDef in initialItems)
                {
                    if (itemDef != null)
                    {
                        var stack = _itemStackFactory.Create(itemDef.Id, null, itemDef.MaxAmount);
                        _slots.Add(new ShopSlotData(stack, isInfinite: true));
                    }
                }
            }
        }

        public bool IsValidSlot(ShopSlot slot) => slot.Index >= 0 && slot.Index < _slots.Count;

        public ItemStackSnapshot? Get(ShopSlot slot)
        {
            if (!IsValidSlot(slot)) return null;
            var slotData = _slots[slot.Index];
            if (slotData?.Stack is null) return null;

            // For infinite items, always return MaxAmount
            if (slotData.IsInfinite)
            {
                var stack = slotData.Stack;
                return new ItemStackSnapshot(
                    stack.Definition,
                    stack.InstanceData,
                    stack.Definition.MaxAmount);
            }

            return slotData.Stack.Snapshot;
        }

        public ItemStackSnapshot? Get(long slot)
        {
            return Get(ShopSlot.FromInt64(slot));
        }

        public IEnumerable<(ShopSlot Slot, ItemStackSnapshot? Snapshot)> Enumerate()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                yield return (new ShopSlot(i), Get(new ShopSlot(i)));
            }
        }

        public int Count(ItemKey key)
        {
            int total = 0;
            foreach (var (_, snapshot) in Enumerate())
                if (snapshot != null && snapshot.Value.Key.Equals(key))
                    total += snapshot.Value.Amount;
            return total;
        }

        public int Add(ItemStackSnapshot snapshot, AddPolicy policy = AddPolicy.StackThenEmpty)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;

            // When adding items (from player selling), they are finite (not infinite)
            var newStack = _itemStackFactory.Create(snapshot.Definition.Id, snapshot.InstanceData, snapshot.Amount);
            _slots.Add(new ShopSlotData(newStack, isInfinite: false));

            var slot = new ShopSlot(_slots.Count - 1);
            ShopChanged?.Invoke(new(slot, ShopChangeKind.Added, newStack.Snapshot));
            SlotChanged?.Invoke(slot);
            Changed?.Invoke();

            return snapshot.Amount;
        }

        public int Remove(ItemKey key, int amount)
        {
            if (amount <= 0) return 0;
            int remaining = amount;
            int removed = 0;

            for (int i = _slots.Count - 1; i >= 0 && remaining > 0; i--)
            {
                var slotData = _slots[i];
                if (slotData?.Stack is null) continue;
                if (!slotData.Stack.Key.Equals(key)) continue;

                // For infinite items, we don't actually remove anything
                if (slotData.IsInfinite)
                {
                    int toRemove = Math.Min(remaining, slotData.Stack.Definition.MaxAmount);
                    remaining -= toRemove;
                    removed += toRemove;

                    var slot = new ShopSlot(i);
                    // Notify updated but item stays at MaxAmount
                    ShopChanged?.Invoke(new(slot, ShopChangeKind.Updated, Get(slot)));
                    SlotChanged?.Invoke(slot);
                    Changed?.Invoke();
                    continue;
                }

                // For finite items, remove normally
                int r = slotData.Stack.RemoveUpTo(remaining);
                remaining -= r;
                removed += r;

                var finiteSlot = new ShopSlot(i);

                if (slotData.Stack.Amount <= 0)
                {
                    _slots.RemoveAt(i);
                    ShopChanged?.Invoke(new(finiteSlot, ShopChangeKind.Removed, null));

                    // Notify that all subsequent slots have shifted down
                    for (int j = i; j < _slots.Count; j++)
                    {
                        var shiftedSlot = new ShopSlot(j);
                        ShopChanged?.Invoke(new(shiftedSlot, ShopChangeKind.Updated, Get(shiftedSlot)));
                        SlotChanged?.Invoke(shiftedSlot);
                    }
                }
                else
                {
                    ShopChanged?.Invoke(new(finiteSlot, ShopChangeKind.Updated, slotData.Stack.Snapshot));
                }

                SlotChanged?.Invoke(finiteSlot);
                Changed?.Invoke();
            }

            return removed;
        }

        public int AddToSlot(ShopSlot slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;

            // Expand list if necessary
            while (slot.Index >= _slots.Count)
            {
                _slots.Add(null);
            }

            var slotData = _slots[slot.Index];

            if (slotData?.Stack is null)
            {
                int put = Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);
                var newStack = _itemStackFactory.Create(snapshot.Definition.Id, snapshot.InstanceData, put);
                _slots[slot.Index] = new ShopSlotData(newStack, isInfinite: false);
                ShopChanged?.Invoke(new(slot, ShopChangeKind.Added, _slots[slot.Index].Stack.Snapshot));
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
                return put;
            }

            // Shop doesn't stack - return 0 if slot is occupied
            return 0;
        }

        public int RemoveFromSlot(ShopSlot slot, int amount)
        {
            if (!IsValidSlot(slot)) return 0;
            if (amount <= 0) return 0;

            var slotData = _slots[slot.Index];
            if (slotData?.Stack is null) return 0;

            // For infinite items, we don't actually remove anything
            if (slotData.IsInfinite)
            {
                int removed = Math.Min(amount, slotData.Stack.Definition.MaxAmount);
                // Notify updated but item stays at MaxAmount
                ShopChanged?.Invoke(new(slot, ShopChangeKind.Updated, Get(slot)));
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
                return removed;
            }

            // For finite items, remove normally
            int actualRemoved = slotData.Stack.RemoveUpTo(amount);
            if (actualRemoved <= 0) return 0;

            if (slotData.Stack.Amount <= 0)
            {
                _slots.RemoveAt(slot.Index);
                ShopChanged?.Invoke(new(slot, ShopChangeKind.Removed, null));

                // Notify that all subsequent slots have shifted down
                for (int i = slot.Index; i < _slots.Count; i++)
                {
                    var shiftedSlot = new ShopSlot(i);
                    ShopChanged?.Invoke(new(shiftedSlot, ShopChangeKind.Updated, Get(shiftedSlot)));
                    SlotChanged?.Invoke(shiftedSlot);
                }
            }
            else
            {
                ShopChanged?.Invoke(new(slot, ShopChangeKind.Updated, slotData.Stack.Snapshot));
            }

            SlotChanged?.Invoke(slot);
            Changed?.Invoke();
            return actualRemoved;
        }

        public int PreviewAdd(ItemStackSnapshot snapshot, AddPolicy policy = AddPolicy.StackThenEmpty)
        {
            if (snapshot.Definition == null) return 0;
            if (snapshot.Amount <= 0) return 0;

            // Shop always accepts all items (infinite capacity)
            return snapshot.Amount;
        }

        public int PreviewAddToSlot(ShopSlot slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) return 0;
            if (snapshot.Amount <= 0) return 0;

            // If slot exists and is occupied, can't add
            if (IsValidSlot(slot) && _slots[slot.Index]?.Stack != null)
                return 0;

            // Otherwise can add full amount
            return Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);
        }

        public void Clear()
        {
            // Only clear finite (player-sold) items, not infinite items
            for (int i = _slots.Count - 1; i >= 0; i--)
            {
                var slotData = _slots[i];
                if (slotData?.Stack is not null && !slotData.IsInfinite)
                {
                    var slot = new ShopSlot(i);
                    _slots.RemoveAt(i);
                    ShopChanged?.Invoke(new(slot, ShopChangeKind.Removed, null));
                    SlotChanged?.Invoke(slot);
                }
            }
            Changed?.Invoke();
        }

        public bool IsInfinite(ShopSlot slot)
        {
            if (!IsValidSlot(slot)) return false;
            var slotData = _slots[slot.Index];
            return slotData?.IsInfinite == true;
        }

        private sealed class ShopSlotData
        {
            public ItemStack Stack { get; }
            public bool IsInfinite { get; }

            public ShopSlotData(ItemStack stack, bool isInfinite)
            {
                Stack = stack;
                IsInfinite = isInfinite;
            }
        }
    }

    public readonly struct ShopSlot : IContainerSlot
    {
        public readonly int Index;

        public ShopSlot(int index)
        {
            Index = index;
        }

        public long ToInt64() => Index;

        public static ShopSlot FromInt64(long value) => new((int)value);

        public override bool Equals(object obj) => obj is ShopSlot other && Index == other.Index;
        public override int GetHashCode() => Index.GetHashCode();
    }

    public readonly struct ShopChange
    {
        public readonly ShopSlot Slot;
        public readonly ShopChangeKind Kind;
        public readonly ItemStackSnapshot? Snapshot;

        public ShopChange(ShopSlot slot, ShopChangeKind kind, ItemStackSnapshot? snapshot)
        {
            Slot = slot;
            Kind = kind;
            Snapshot = snapshot;
        }
    }

    public enum ShopChangeKind
    {
        Added,
        Updated,
        Removed
    }
}
