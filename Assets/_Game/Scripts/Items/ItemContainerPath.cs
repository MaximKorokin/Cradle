using Assets._Game.Scripts.Items.Commands;

namespace Assets._Game.Scripts.Items
{
    public readonly struct ItemContainerPath
    {
        public string EntityId { get; }
        public ItemContainerId ContainerId { get; }

        private ItemContainerPath(string entityId, ItemContainerId containerId)
        {
            EntityId = entityId;
            ContainerId = containerId;
        }

        public static ItemContainerPath Inventory(string entityId) => new(entityId, ItemContainerId.Inventory);
        public static ItemContainerPath Equipment(string entityId) => new(entityId, ItemContainerId.Equipment);
        public static ItemContainerPath Storage(string entityId) => new(entityId, ItemContainerId.Storage);
        public static ItemContainerPath Shop(string entityId) => new(entityId, ItemContainerId.Shop);
    }
}
