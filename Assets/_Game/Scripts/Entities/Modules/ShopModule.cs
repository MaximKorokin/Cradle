using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Shop;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class ShopModule : EntityModuleBase
    {
        public float Radius { get; }
        public ShopModel Shop { get; }
        public ShopDefinition Definition { get; }

        public ShopModule(ShopModel shop, ShopDefinition definition, float radius)
        {
            Shop = shop;
            Definition = definition;
            Radius = radius;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public sealed class ShopModuleFactory : IEntityModuleFactory
    {
        private readonly ItemStackFactory _itemStackFactory;

        public ShopModuleFactory(ItemStackFactory itemStackFactory)
        {
            _itemStackFactory = itemStackFactory;
        }

        public EntityModuleBase Create(EntityDefinition definition)
        {
            if (!definition.TryGetModuleDefinition<ShopModuleDefinition>(out var shopModuleDefinition))
                return null;

            if (shopModuleDefinition.ShopDefinition == null)
                return null;

            var shopDefinition = shopModuleDefinition.ShopDefinition;
            var shop = new ShopModel(_itemStackFactory);

            // Populate shop with initial items
            if (shopDefinition.InitialItems != null)
            {
                foreach (var item in shopDefinition.InitialItems)
                {
                    if (item.ItemDefinition != null && item.Amount > 0)
                    {
                        shop.Add(new ItemStackSnapshot(item.ItemDefinition, null, item.Amount));
                    }
                }
            }

            return new ShopModule(shop, shopDefinition, shopModuleDefinition.Radius);
        }
    }
}
