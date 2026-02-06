using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items.Equipment
{
    public sealed class EquipmentModel : IItemContainer<EquipmentSlotKey>
    {
        private readonly Dictionary<EquipmentSlotKey, ItemStack> _slots;
        private readonly IEquipmentRules _rules;

        public event Action<EquipmentChange> EquipmentChanged;
        public event Action<EquipmentSlotKey> SlotChanged;
        public event Action Changed;

        public EquipmentModel(EquipmentSlotType[] slots, IEquipmentRules rules)
        {
            _rules = rules;
            _slots = new();
            var slotTypeCounts = new Dictionary<EquipmentSlotType, int>();
            foreach (var slotType in slots)
            {
                var count = 0;
                if (slotTypeCounts.TryGetValue(slotType, out var slotTypeCount))
                    count = slotTypeCount;
                _slots[new(slotType, count)] = null;
                slotTypeCounts[slotType] = count + 1;
            }
        }

        public bool IsValidSlot(EquipmentSlotKey slot)
            => _slots.ContainsKey(slot);

        public ItemStackSnapshot? Get(EquipmentSlotKey slot)
        {
            if (!_slots.TryGetValue(slot, out var s) || s is null) return null;
            return s.Snapshot;
        }

        public IEnumerable<(EquipmentSlotKey Slot, ItemStackSnapshot? Snapshot)> Enumerate()
        {
            foreach (var kv in _slots)
                yield return (kv.Key, kv.Value?.Snapshot);
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
            // For equipment: by default we try to place into ANY valid empty slot.
            // Stacking doesn't make sense here usually, but we support MaxStack > 1 if you want "ammo pouch" etc.
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;

            int remaining = snapshot.Amount;
            int added = 0;

            // If stacking is allowed, stack into existing matching stacks first:
            if (policy is AddPolicy.StackThenEmpty or AddPolicy.StackOnly)
            {
                var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);
                foreach (var slot in _slots.Keys.ToArray())
                {
                    if (remaining <= 0) break;
                    var s = _slots[slot];
                    if (s is null) continue;
                    if (!_rules.CanPlace(slot, snapshot)) continue;
                    if (!s.CanStackWith(key)) continue;

                    int a = s.AddUpTo(remaining);
                    if (a > 0)
                    {
                        remaining -= a;
                        added += a;
                        EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Updated, s.Snapshot));
                        SlotChanged?.Invoke(slot);
                        Changed?.Invoke();
                    }
                }
            }

            if (policy is AddPolicy.StackOnly) return added;

            // Place into empty slots
            foreach (var slot in _slots.Keys.ToArray())
            {
                if (remaining <= 0) break;
                if (_slots[slot] is not null) continue;
                if (!_rules.CanPlace(slot, snapshot)) continue;

                int put = Math.Min(snapshot.Definition.MaxAmount, remaining);
                _slots[slot] = new ItemStack(snapshot.Definition, snapshot.InstanceData, put);
                remaining -= put;
                added += put;
                EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Equipped, _slots[slot].Snapshot));
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
            }

            return added;
        }

        public int AddToSlot(EquipmentSlotKey slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;
            if (!IsValidSlot(slot)) return 0;
            if (!_rules.CanPlace(slot, snapshot)) return 0;

            var existing = _slots[slot];
            var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);

            if (existing is null)
            {
                int put = Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);
                _slots[slot] = new ItemStack(snapshot.Definition, snapshot.InstanceData, put);
                EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Equipped, _slots[slot].Snapshot));
                SlotChanged?.Invoke(slot);
                Changed?.Invoke();
                return put;
            }

            // Only stacking into same item
            if (!existing.CanStackWith(key)) return 0;
            int added = existing.AddUpTo(snapshot.Amount);
            if (added > 0)
            {
                EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Updated, existing.Snapshot));
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

            foreach (var slot in _slots.Keys.ToArray())
            {
                if (remaining <= 0) break;
                var s = _slots[slot];
                if (s is null) continue;
                if (!s.Key.Equals(key)) continue;

                int r = s.RemoveUpTo(remaining);
                if (r > 0)
                {
                    remaining -= r;
                    removed += r;

                    if (s.Amount == 0)
                    {
                        _slots[slot] = null;
                        EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Unequipped, s.Snapshot));
                    }
                    else
                    {
                        EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Updated, s.Snapshot));
                    }
                    SlotChanged?.Invoke(slot);
                    Changed?.Invoke();
                }
            }

            return removed;
        }

        public int RemoveFromSlot(EquipmentSlotKey slot, int amount)
        {
            if (amount <= 0) return 0;
            if (!IsValidSlot(slot)) return 0;

            var s = _slots[slot];
            if (s is null) return 0;

            int removed = s.RemoveUpTo(amount);
            if (removed > 0)
            {
                if (s.Amount == 0)
                {
                    _slots[slot] = null;
                    EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Unequipped, s.Snapshot));
                }
                else
                {
                    EquipmentChanged?.Invoke(new(slot, EquipmentChangeKind.Updated, s.Snapshot));
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

            // 1) Stack into existing stacks (if allowed by policy + rules)
            if (policy is AddPolicy.StackThenEmpty or AddPolicy.StackOnly)
            {
                foreach (var slot in _slots.Keys) // safe: no mutation
                {
                    if (remaining <= 0) break;

                    var s = _slots[slot];
                    if (s is null) continue;

                    // Must be allowed in this equipment slot (rule may depend on slot/type)
                    if (!_rules.CanPlace(slot, snapshot)) continue;

                    // Must be same stack key
                    if (!s.CanStackWith(key)) continue;

                    int space = Math.Max(0, s.Definition.MaxAmount - s.Amount);
                    if (space <= 0) continue;

                    int take = Math.Min(space, remaining);
                    remaining -= take;
                    canAdd += take;
                }
            }

            if (policy is AddPolicy.StackOnly)
                return canAdd;

            // 2) Place into empty slots that accept the item (rules)
            if (policy is AddPolicy.EmptyOnly or AddPolicy.StackThenEmpty)
            {
                foreach (var slot in _slots.Keys)
                {
                    if (remaining <= 0) break;

                    if (_slots[slot] is not null) continue;

                    if (!_rules.CanPlace(slot, snapshot)) continue;

                    int take = Math.Min(snapshot.Definition.MaxAmount, remaining);
                    remaining -= take;
                    canAdd += take;
                }
            }

            return canAdd;
        }

        // Optional: preview for a specific equipment slot (drag onto exact slot)
        public int PreviewAddToSlot(EquipmentSlotKey slot, ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) throw new ArgumentNullException(nameof(snapshot.Definition));
            if (snapshot.Amount <= 0) return 0;
            if (!IsValidSlot(slot)) return 0;
            if (!_rules.CanPlace(slot, snapshot)) return 0;

            var existing = _slots[slot];
            if (existing is null)
                return Math.Min(snapshot.Definition.MaxAmount, snapshot.Amount);

            var key = ItemKey.From(snapshot.Definition, snapshot.InstanceData);
            if (!existing.CanStackWith(key)) return 0;

            int space = Math.Max(0, existing.Definition.MaxAmount - existing.Amount);
            return Math.Min(space, snapshot.Amount);
        }

        /// <summary>
        /// Ignores if there is anything in the slot
        /// </summary>
        public bool CanEquip(ItemStackSnapshot snapshot)
        {
            if (snapshot.Definition == null) return false;

            return _rules.CanEquip(snapshot);
        }
    }

    public readonly struct EquipmentChange
    {
        public readonly EquipmentSlotKey Slot;
        public readonly EquipmentChangeKind Kind;
        public readonly ItemStackSnapshot? Item;

        public EquipmentChange(
            EquipmentSlotKey slot,
            EquipmentChangeKind kind,
            ItemStackSnapshot? item)
        {
            Slot = slot;
            Kind = kind;
            Item = item;
        }
    }

    public enum EquipmentChangeKind
    {
        Equipped,
        Unequipped,
        Replaced,
        Updated
    }
}
