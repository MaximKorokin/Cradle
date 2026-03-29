using Assets._Game.Scripts.Entities;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    public interface IItemTrait { }

    [Serializable]
    public abstract class ItemTraitBase : IItemTrait { }

    public abstract class FunctionalItemTraitBase : ItemTraitBase
    {
        [field: SerializeField]
        public ItemTrigger Triggers { get; private set; }

        public virtual bool CanTrigger(in ItemTriggerContext context)
        {
            return (Triggers & context.Trigger) != 0;
        }
    }

    [Flags]
    public enum ItemTrigger
    {
        OnEquipmentChange = 1,
        OnUse = 64,
    }

    public readonly struct ItemTriggerContext
    {
        public readonly Entity User;
        public readonly ItemTrigger Trigger;
        public readonly ItemStackSnapshot Item;
        public readonly object Payload;

        public ItemTriggerContext(Entity user, ItemTrigger trigger, ItemStackSnapshot item, object payload = null)
        {
            User = user;
            Trigger = trigger;
            Item = item;
            Payload = payload;
        }
    }
}
