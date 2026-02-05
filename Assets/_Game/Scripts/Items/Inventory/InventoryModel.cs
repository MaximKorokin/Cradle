using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Items.Inventory
{
    public sealed class InventoryModel : IItemContainer<int>
    {
        private readonly ItemStack[] _slots;
        public int Capacity => _slots.Length;

        public event Action<int> SlotChanged;
        public event Action Changed;

        public InventoryModel(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _slots = new ItemStack[capacity];
        }

        public bool IsValidSlot(int slot) => slot >= 0 && slot < _slots.Length;

        public ItemStackSnapshot? Get(int slot)
        {
            if (!IsValidSlot(slot)) return null;
            var s = _slots[slot];
            if (s is null) return null;
            return s.Snapshot;
        }

        public IEnumerable<(int Slot, ItemStackSnapshot? Snapshot)> Enumerate()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var s = _slots[i];
                yield return (i, s?.Snapshot);
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
                        SlotChanged?.Invoke(i);
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
                    _slots[i] = new ItemStack(snapshot);
                    remaining -= put;
                    added += put;
                    SlotChanged?.Invoke(i);
                    Changed?.Invoke();
                }
            }

            return added;
        }

        public int AddToSlot(int slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (!IsValidSlot(slot)) return 0;
            if (snapshot.Amount <= 0) return 0;

            var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);
            var s = _slots[slot];

            if (s is null)
            {
                int put = Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);
                _slots[slot] = new ItemStack(snapshot);
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
                return put;
            }

            if (!s.CanStackWith(key)) return 0;
            int added = s.AddUpTo(snapshot.Amount);
            if (added > 0)
            {
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

                    if (s.Amount == 0) _slots[i] = null;
                    SlotChanged?.Invoke(i);
                    Changed?.Invoke();
                }
            }

            return removed;
        }

        public int RemoveFromSlot(int slot, int amount)
        {
            if (!IsValidSlot(slot)) return 0;
            if (amount <= 0) return 0;

            var s = _slots[slot];
            if (s is null) return 0;

            int removed = s.RemoveUpTo(amount);
            if (removed > 0)
            {
                if (s.Amount == 0) _slots[slot] = null;
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

        public int PreviewAddToSlot(int slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;
            if (!IsValidSlot(slot)) return 0;

            var existing = _slots[slot];

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
        public bool Swap(int a, int b)
        {
            if (!IsValidSlot(a) || !IsValidSlot(b) || a == b) return false;
            (_slots[a], _slots[b]) = (_slots[b], _slots[a]);
            SlotChanged?.Invoke(a);
            SlotChanged?.Invoke(b);
            Changed?.Invoke();
            return true;
        }
    }
}