namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct PickupItemIntent : IIntent
    {
        public readonly Entity LootItem;

        IIntent IIntent.None => None;
        public static PickupItemIntent None => default;

        public PickupItemIntent(Entity lootItem)
        {
            LootItem = lootItem;
        }
    }
}
