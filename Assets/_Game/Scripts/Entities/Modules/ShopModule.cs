using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Shop;
using System.Collections.Generic;

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

            // Prepare initial item definitions (infinite stock items)
            List<ItemDefinition> initialItemDefinitions = null;
            if (shopDefinition.InitialItems != null)
            {
                initialItemDefinitions = new List<ItemDefinition>();
                foreach (var item in shopDefinition.InitialItems)
                {
                    if (item.ItemDefinition != null)
                    {
                        initialItemDefinitions.Add(item.ItemDefinition);
                    }
                }
            }

            var shop = new ShopModel(_itemStackFactory, initialItemDefinitions);

            return new ShopModule(shop, shopDefinition, shopModuleDefinition.Radius);
        }
    }
}
