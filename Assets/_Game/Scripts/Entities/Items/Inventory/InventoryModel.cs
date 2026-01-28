namespace Assets._Game.Scripts.Entities.Items.Inventory
{
    public class InventoryModel : IItemContainer
    {
        public int SlotCount => throw new System.NotImplementedException();

        public bool CanPut(int slot, ItemStack stack)
        {
            throw new System.NotImplementedException();
        }

        public ItemStack Get(int slot)
        {
            throw new System.NotImplementedException();
        }

        public void Put(int slot, ItemStack stack)
        {
            throw new System.NotImplementedException();
        }

        public ItemStack Take(int slot, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
