using Assets._Game.Scripts.Items.Shop;

namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct BuyFromShopCommand : IItemCommand
    {
        public ShopModel ShopModel { get; }
        public long ShopSlot { get; }
        public int Amount { get; }
        public int Price { get; }

        public BuyFromShopCommand(ShopModel shopModel, long shopSlot, int amount, int price)
        {
            ShopModel = shopModel;
            ShopSlot = shopSlot;
            Amount = amount;
            Price = price;
        }
    }

    public readonly struct SellToShopCommand : IItemCommand
    {
        public ShopModel ShopModel { get; }
        public long InventorySlot { get; }
        public int Amount { get; }
        public int Price { get; }

        public SellToShopCommand(ShopModel shopModel, long inventorySlot, int amount, int price)
        {
            ShopModel = shopModel;
            InventorySlot = inventorySlot;
            Amount = amount;
            Price = price;
        }
    }
}
