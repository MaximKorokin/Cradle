using System;

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

        public override bool Equals(object obj)
        {
            return obj is ItemContainerPath path &&
                   EntityId == path.EntityId &&
                   ContainerId == path.ContainerId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EntityId, ContainerId);
        }

        public static bool operator ==(ItemContainerPath left, ItemContainerPath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemContainerPath left, ItemContainerPath right)
        {
            return !(left == right);
        }
    }
}
