using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class InventoryExtensions
    {
        public static void SortByName(this InventoryModel inventoryModel)
        {
            inventoryModel.SortBy(x => x.Definition.Name);
        }

        public static void SortByPurpose(this InventoryModel inventoryModel)
        {
            inventoryModel.SortBy(x => x.GetPurpose());
        }
    }
}
