using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Items.Shop
{
    public sealed class ShopModel : IItemContainer<ShopSlot>
    {
        private readonly List<ItemStack> _slots;
        private readonly ItemStackFactory _itemStackFactory;
        public int Capacity => _slots.Count;

        public event Action<ShopChange> ShopChanged;
        public event Action<ShopSlot> SlotChanged;
        public event Action Changed;

        public ShopModel(ItemStackFactory itemStackFactory)
        {
            _slots = new List<ItemStack>();
            _itemStackFactory = itemStackFactory;
        }

        public bool IsValidSlot(ShopSlot slot) => slot.Index >= 0 && slot.Index < _slots.Count;

        public ItemStackSnapshot? Get(ShopSlot slot)
        {
            if (!IsValidSlot(slot)) return null;
            var s = _slots[slot.Index];
            if (s is null) return null;
            return s.Snapshot;
        }

        public ItemStackSnapshot? Get(long slot)
        {
            return Get(ShopSlot.FromInt64(slot));
        }

        public IEnumerable<(ShopSlot Slot, ItemStackSnapshot? Snapshot)> Enumerate()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var s = _slots[i];
                yield return (new ShopSlot(i), s?.Snapshot);
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

            // Shop doesn't stack items - each item goes to a new slot
            // Add the entire stack as-is to a single new slot
            var newStack = _itemStackFactory.Create(snapshot.Definition.Id, snapshot.InstanceData, snapshot.Amount);
            _slots.Add(newStack);

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
                var s = _slots[i];
                if (s is null) continue;
                if (!s.Key.Equals(key)) continue;

                int r = s.RemoveUpTo(remaining);
                remaining -= r;
                removed += r;

                var slot = new ShopSlot(i);

                if (s.Amount <= 0)
                {
                    _slots.RemoveAt(i);
                    ShopChanged?.Invoke(new(slot, ShopChangeKind.Removed, null));

                    // Notify that all subsequent slots have shifted down
                    for (int j = i; j < _slots.Count; j++)
                    {
                        var shiftedSlot = new ShopSlot(j);
                        ShopChanged?.Invoke(new(shiftedSlot, ShopChangeKind.Updated, _slots[j]?.Snapshot));
                        SlotChanged?.Invoke(shiftedSlot);
                    }
                }
                else
                {
                    ShopChanged?.Invoke(new(slot, ShopChangeKind.Updated, s.Snapshot));
                }

                SlotChanged?.Invoke(slot);
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

            var s = _slots[slot.Index];

            if (s is null)
            {
                int put = Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);
                _slots[slot.Index] = _itemStackFactory.Create(snapshot.Definition.Id, snapshot.InstanceData, put);
                ShopChanged?.Invoke(new(slot, ShopChangeKind.Added, _slots[slot.Index].Snapshot));
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

            var s = _slots[slot.Index];
            if (s is null) return 0;

            int removed = s.RemoveUpTo(amount);
            if (removed <= 0) return 0;

            if (s.Amount <= 0)
            {
                _slots.RemoveAt(slot.Index);
                ShopChanged?.Invoke(new(slot, ShopChangeKind.Removed, null));

                // Notify that all subsequent slots have shifted down
                for (int i = slot.Index; i < _slots.Count; i++)
                {
                    var shiftedSlot = new ShopSlot(i);
                    ShopChanged?.Invoke(new(shiftedSlot, ShopChangeKind.Updated, _slots[i]?.Snapshot));
                    SlotChanged?.Invoke(shiftedSlot);
                }
            }
            else
            {
                ShopChanged?.Invoke(new(slot, ShopChangeKind.Updated, s.Snapshot));
            }

            SlotChanged?.Invoke(slot);
            Changed?.Invoke();
            return removed;
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
            if (IsValidSlot(slot) && _slots[slot.Index] != null)
                return 0;

            // Otherwise can add full amount
            return Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);
        }

        public void Clear()
        {
            for (int i = _slots.Count - 1; i >= 0; i--)
            {
                if (_slots[i] is not null)
                {
                    var slot = new ShopSlot(i);
                    _slots.RemoveAt(i);
                    ShopChanged?.Invoke(new(slot, ShopChangeKind.Removed, null));
                    SlotChanged?.Invoke(slot);
                }
            }
            Changed?.Invoke();
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
