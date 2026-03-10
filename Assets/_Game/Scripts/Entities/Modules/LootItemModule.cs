using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class LootItemModule : EntityModuleBase
    {
        public ItemDefinition ItemDefinition { get; }
        public int Amount { get; }

        public LootItemModule(ItemDefinition itemDefinition, int amount)
        {
            ItemDefinition = itemDefinition;
            Amount = amount;
        }
    }
}
