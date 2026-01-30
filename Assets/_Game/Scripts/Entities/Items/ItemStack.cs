namespace Assets._Game.Scripts.Entities.Items
{
    public class ItemStack
    {
        public ItemStack(ItemDefinition definition, IItemInstanceData instanceData, int amount)
        {
            Definition = definition;
            Instance = instanceData;
            Amount = amount;
        }

        public ItemDefinition Definition { get; set; }
        public IItemInstanceData Instance { get; set; }
        public int Amount { get; set; }
    }
}
