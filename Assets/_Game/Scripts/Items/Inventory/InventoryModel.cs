using Assets._Game.Scripts.Items.Equipment;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Items.Inventory
{
    public sealed class InventoryModel : IItemContainer<InventorySlot>
    {
        private readonly ItemStack[] _slots;
        private readonly ItemStackFactory _itemStackFactory;
        public int Capacity => _slots.Length;

        public event Action<InventoryChange> InventoryChanged;
        public event Action<InventorySlot> SlotChanged;
        public event Action Changed;

        public InventoryModel(int capacity, ItemStackFactory itemStackFactory)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _slots = new ItemStack[capacity];
            _itemStackFactory = itemStackFactory;
        }

        public bool IsValidSlot(InventorySlot slot) => slot.Index >= 0 && slot.Index < _slots.Length;

        public ItemStackSnapshot? Get(InventorySlot slot)
        {
            if (!IsValidSlot(slot)) return null;
            var s = _slots[slot.Index];
            if (s is null) return null;
            return s.Snapshot;
        }

        public ItemStackSnapshot? Get(long slot)
        {
            return Get(InventorySlot.FromInt64(slot));
        }

        public IEnumerable<(InventorySlot Slot, ItemStackSnapshot? Snapshot)> Enumerate()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var s = _slots[i];
                yield return (new InventorySlot(i), s?.Snapshot);
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

            var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);
            int remaining = snapshot.Amount;
            int added = 0;

            if (policy is AddPolicy.StackThenEmpty or AddPolicy.StackOnly)
            {
                // fill existing stacks
                for (int i = 0; i < _slots.Length && remaining > 0; i++)
                {
                    var s = _slots[i];
                    if (s is null) continue;
                    if (!s.CanStackWith(key)) continue;
                    int a = s.AddUpTo(remaining);
                    if (a > 0)
                    {
                        remaining -= a;
                        added += a;
                        var slot = new InventorySlot(i);
                        InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Updated, s.Snapshot));
                        SlotChanged?.Invoke(slot);
                        Changed?.Invoke();
                    }
                }
            }

            if (policy is AddPolicy.StackOnly) return added;
            if (policy is AddPolicy.EmptyOnly or AddPolicy.StackThenEmpty)
            {
                // place new stacks into empty slots (can require multiple slots)
                for (int i = 0; i < _slots.Length && remaining > 0; i++)
                {
                    if (_slots[i] is not null) continue;

                    int put = Math.Min(snapshot.Definition.MaxAmount, remaining);
                    _slots[i] = _itemStackFactory.Create(snapshot.Definition.Id, snapshot.InstanceData, put);
                    remaining -= put;
                    added += put;
                    var slot = new InventorySlot(i);
                    InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Added, _slots[i].Snapshot));
                    SlotChanged?.Invoke(slot);
                    Changed?.Invoke();
                }
            }

            return added;
        }

        public int AddToSlot(InventorySlot slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (!IsValidSlot(slot)) return 0;
            if (snapshot.Amount <= 0) return 0;

            var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);
            var s = _slots[slot.Index];

            if (s is null)
            {
                int put = Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);
                _slots[slot.Index] = _itemStackFactory.Create(snapshot.Definition.Id, snapshot.InstanceData, put);
                InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Added, _slots[slot.Index].Snapshot));
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
                return put;
            }

            if (!s.CanStackWith(key)) return 0;
            int added = s.AddUpTo(snapshot.Amount);
            if (added > 0)
            {
                InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Updated, s.Snapshot));
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
            }
            return added;
        }

        public int Remove(ItemKey key, int amount)
        {
            if (amount <= 0) return 0;

            int remaining = amount;
            int removed = 0;

            // Remove from any stacks that match the key
            for (int i = 0; i < _slots.Length && remaining > 0; i++)
            {
                var s = _slots[i];
                if (s is null) continue;
                if (!s.Key.Equals(key)) continue;

                int r = s.RemoveUpTo(remaining);
                if (r > 0)
                {
                    remaining -= r;
                    removed += r;

                    var slot = new InventorySlot(i);
                    if (s.Amount == 0)
                    {
                        _slots[i] = null;
                        InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Removed, s.Snapshot));
                    }
                    else
                    {
                        InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Updated, s.Snapshot));
                    }
                    SlotChanged?.Invoke(slot);
                    Changed?.Invoke();
                }
            }

            return removed;
        }

        public int RemoveFromSlot(InventorySlot slot, int amount)
        {
            if (!IsValidSlot(slot)) return 0;
            if (amount <= 0) return 0;

            var s = _slots[slot.Index];
            if (s is null) return 0;

            int removed = s.RemoveUpTo(amount);
            if (removed > 0)
            {
                if (s.Amount == 0)
                {
                    _slots[slot.Index] = null;
                    InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Removed, s.Snapshot));
                }
                else
                {
                    InventoryChanged?.Invoke(new(slot, InventoryChangeKind.Updated, s.Snapshot));
                }
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
            }

            return removed;
        }

        public int PreviewAdd(ItemStackSnapshot snapshot, AddPolicy policy = AddPolicy.StackThenEmpty)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;

            var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);
            int remaining = snapshot.Amount;
            int canAdd = 0;

            if (policy is AddPolicy.StackThenEmpty or AddPolicy.StackOnly)
            {
                foreach (var s in _slots)
                {
                    if (remaining <= 0) break;
                    if (s is null) continue;
                    if (!s.CanStackWith(key)) continue;

                    int space = s.Definition.MaxAmount - s.Amount;
                    if (space <= 0) continue;

                    int take = Math.Min(space, remaining);
                    remaining -= take;
                    canAdd += take;
                }
            }

            if (policy is AddPolicy.StackOnly)
                return canAdd;

            if (policy is AddPolicy.EmptyOnly or AddPolicy.StackThenEmpty)
            {
                foreach (var s in _slots)
                {
                    if (remaining <= 0) break;
                    if (s is not null) continue;

                    int take = Math.Min(snapshot.Definition.MaxAmount, remaining);
                    remaining -= take;
                    canAdd += take;
                }
            }

            return canAdd;
        }

        public int PreviewAddToSlot(InventorySlot slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;
            if (!IsValidSlot(slot)) return 0;

            var existing = _slots[slot.Index];

            // Empty slot, can put a new stack
            if (existing is null)
                return Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);

            // Slot is taken, checking that can stack
            var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);
            if (!existing.CanStackWith(key))
                return 0;

            int space = Math.Max(0, existing.Definition.MaxAmount - existing.Amount);
            return Math.Min(space, snapshot.Amount);
        }

        // Utility: swap stacks (UI drag-drop inside inventory)
        public bool Swap(InventorySlot a, InventorySlot b)
        {
            if (!IsValidSlot(a) || !IsValidSlot(b) || a.Index == b.Index) return false;
            (_slots[a.Index], _slots[b.Index]) = (_slots[b.Index], _slots[a.Index]);
            InventoryChanged?.Invoke(new(a, InventoryChangeKind.Replaced, _slots[a.Index].Snapshot));
            InventoryChanged?.Invoke(new(b, InventoryChangeKind.Replaced, _slots[b.Index].Snapshot));
            SlotChanged?.Invoke(a);
            SlotChanged?.Invoke(b);
            Changed?.Invoke();
            return true;
        }
    }

    public readonly struct InventoryChange
    {
        public readonly InventorySlot Slot;
        public readonly InventoryChangeKind Kind;
        public readonly ItemStackSnapshot? Item;

        public InventoryChange(
            InventorySlot slot,
            InventoryChangeKind kind,
            ItemStackSnapshot? item)
        {
            Slot = slot;
            Kind = kind;
            Item = item;
        }
    }

    public enum InventoryChangeKind
    {
        /// <summary>Slot was empty</summary>
        Added,
        /// <summary>Slot is now empty</summary>
        Removed,
        /// <summary>Slot was item and now other item</summary>
        Replaced,
        /// <summary>Slot item updated</summary>
        Updated
    }
}
