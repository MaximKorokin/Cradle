using Assets._Game.Scripts.Items.Shop;

namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct BuyFromShopCommand : IItemCommand
    {
        public ItemContainerPath ShopModelPath { get; }
        public ItemContainerPath InventoryModelPath { get; }
        public long ShopSlot { get; }
        public int Amount { get; }
        public int Price { get; }

        public BuyFromShopCommand(ItemContainerPath shopModelPath, ItemContainerPath inventoryModelPath, long shopSlot, int amount, int price)
        {
            ShopModelPath = shopModelPath;
            InventoryModelPath = inventoryModelPath;
            ShopSlot = shopSlot;
            Amount = amount;
            Price = price;
        }
    }

    public readonly struct SellToShopCommand : IItemCommand
    {
        public ItemContainerPath ShopModelPath { get; }
        public ItemContainerPath InventoryModelPath { get; }
        public long InventorySlot { get; }
        public int Amount { get; }
        public int Price { get; }

        public SellToShopCommand(ItemContainerPath shopModelPath, ItemContainerPath inventoryModelPath, long inventorySlot, int amount, int price)
        {
            ShopModelPath = shopModelPath;
            InventoryModelPath = inventoryModelPath;
            InventorySlot = inventorySlot;
            Amount = amount;
            Price = price;
        }
    }
}
