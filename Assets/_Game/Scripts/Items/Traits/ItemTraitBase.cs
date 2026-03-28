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
    }

    [Flags]
    public enum ItemTrigger
    {
        OnEquipmentChange = 1,
        OnUse = 64,
    }
}
