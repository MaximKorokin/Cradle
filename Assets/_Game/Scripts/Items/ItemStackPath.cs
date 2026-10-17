using System;

namespace Assets._Game.Scripts.Items
{
    public readonly struct ItemStackPath
    {
        public readonly bool HasData;

        public readonly ItemContainerPath ContainerPath;
        public readonly long Slot;

        public ItemStackPath(ItemContainerPath containerPath, long slot)
        {
            HasData = true;

            ContainerPath = containerPath;
            Slot = slot;
        }

        public override string ToString()
        {
            return $"{ContainerPath}, {Slot}";
        }

        public override bool Equals(object obj)
        {
            return obj is ItemStackPath path &&
                ContainerPath == path.ContainerPath &&
                Slot == path.Slot;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContainerPath, Slot);
        }

        public static bool operator ==(ItemStackPath left, ItemStackPath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemStackPath left, ItemStackPath right)
        {
            return !(left == right);
        }
    }
}
