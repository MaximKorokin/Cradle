namespace Assets._Game.Scripts.Entities.Items.Equipment
{
    public class EquipmentModel : IItemContainer
    {
        public int SlotCount => 11;

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
