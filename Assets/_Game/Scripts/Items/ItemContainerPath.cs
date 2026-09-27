using System;

namespace Assets._Game.Scripts.Items
{
    public readonly struct ItemContainerPath
    {
        public readonly bool HasData;

        public readonly string EntityId;
        public readonly ItemContainerId ContainerId;

        private ItemContainerPath(string entityId, ItemContainerId containerId)
        {
            HasData = true;

            EntityId = entityId;
            ContainerId = containerId;
        }

        public static ItemContainerPath Inventory(string entityId) => new(entityId, ItemContainerId.Inventory);
        public static ItemContainerPath Equipment(string entityId) => new(entityId, ItemContainerId.Equipment);
        public static ItemContainerPath Storage(string entityId) => new(entityId, ItemContainerId.Storage);
        public static ItemContainerPath Shop(string entityId) => new(entityId, ItemContainerId.Shop);

        public override string ToString()
        {
            return $"{EntityId}, {ContainerId}";
        }

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
