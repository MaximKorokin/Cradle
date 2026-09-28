namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct BuyFromShopCommand : IItemCommand
    {
        public ItemContainerPath ShopModelPath { get; }
        public ItemContainerPath GoldInventoryModelPath { get; }
        public ItemContainerPath ItemContainerModelPath { get; }
        public long ShopSlot { get; }
        public long InventorySlot { get; }
        public int Amount { get; }
        public int Price { get; }

        public BuyFromShopCommand(
            ItemContainerPath shopModelPath,
            ItemContainerPath goldInventoryModelPath,
            ItemContainerPath itemContainerModelPath,
            long shopSlot,
            long inventorySlot,
            int amount,
            int price)
        {
            ShopModelPath = shopModelPath;
            GoldInventoryModelPath = goldInventoryModelPath;
            ItemContainerModelPath = itemContainerModelPath;
            ShopSlot = shopSlot;
            InventorySlot = inventorySlot;
            Amount = amount;
            Price = price;
        }
    }

    public readonly struct SellToShopCommand : IItemCommand
    {
        public ItemContainerPath ShopModelPath { get; }
        public ItemContainerPath GoldInventoryModelPath { get; }
        public ItemContainerPath ItemContainerModelPath { get; }
        public long InventorySlot { get; }
        public int Amount { get; }
        public int Price { get; }

        public SellToShopCommand(
            ItemContainerPath shopModelPath,
            ItemContainerPath goldInventoryModelPath,
            ItemContainerPath itemContainerModelPath,
            long inventorySlot,
            int amount,
            int price)
        {
            ShopModelPath = shopModelPath;
            GoldInventoryModelPath = goldInventoryModelPath;
            ItemContainerModelPath = itemContainerModelPath;
            InventorySlot = inventorySlot;
            Amount = amount;
            Price = price;
        }
    }
}
