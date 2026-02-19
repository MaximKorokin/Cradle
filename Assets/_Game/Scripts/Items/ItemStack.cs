using System;

namespace Assets._Game.Scripts.Items
{
    public class ItemStack
    {
        public ItemDefinition Definition { get; set; }
        public IItemInstanceData InstanceData { get; set; }
        public int Amount { get; set; }

        public ItemKey Key => ItemKey.From(Definition, InstanceData);

        public ItemStack(ItemDefinition definition, IItemInstanceData instanceData, int amount)
        {
            Definition = definition;
            InstanceData = instanceData;
            Amount = amount;
        }

        public ItemStack(ItemStackSnapshot snapshot) : this(snapshot.Definition, snapshot.InstanceData, snapshot.Amount)
        {
        }

        public ItemStackSnapshot Snapshot => new(Definition, InstanceData, Amount);

        public bool CanStackWith(ItemKey key) => Key.Equals(key);

        public int AddUpTo(int amount)
        {
            if (amount <= 0) return 0;

            int space = Math.Max(0, Definition.MaxAmount - Amount);
            int add = Math.Min(space, amount);
            Amount += add;
            return add;
        }

        public int RemoveUpTo(int amount)
        {
            if (amount <= 0) return 0;

            int rem = Math.Min(Amount, amount);
            Amount -= rem;
            return rem;
        }

        public void SetAmount(int newAmount)
        {
            Amount = newAmount;
            Validate();
        }

        private void Validate()
        {
            if (Amount < 0) throw new InvalidOperationException("Amount < 0");
            if (Definition.MaxAmount <= 0) throw new InvalidOperationException("MaxAmount <= 0");
            if (Amount > Definition.MaxAmount) throw new InvalidOperationException("Amount > MaxAmount");
            if (Definition.MaxAmount == 1 && Amount > 1) throw new InvalidOperationException("Unique item stack > 1");
        }
    }

    public readonly struct ItemKey
    {
        public readonly string DefinitionId;
        public readonly string StackingKey;

        public ItemKey(string definitionId, string stackingKey)
        {
            DefinitionId = definitionId;
            StackingKey = stackingKey;
        }

        public static ItemKey From(ItemDefinition definition, IItemInstanceData instanceData)
        {
            var key = (instanceData as IImmutableItemInstanceData)?.GetStackingKey();
            if (string.IsNullOrEmpty(key)) key = null;
            return new ItemKey(definition.Id, key);
        }
    }

    public readonly struct ItemStackSnapshot
    {
        public readonly ItemDefinition Definition;
        public readonly IItemInstanceData InstanceData;
        public readonly int Amount;

        public ItemStackSnapshot(ItemDefinition definition, IItemInstanceData instanceData, int amount)
        {
            Definition = definition;
            InstanceData = instanceData;
            Amount = amount;
        }

        public ItemKey Key => ItemKey.From(Definition, InstanceData);
        public bool IsEmpty => Amount <= 0;
    }
}
