using Assets._Game.Scripts.Shared.Attributes;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public class RestrictionTrait : ItemTraitBase
    {
        [field: SerializeField]
        public ItemRestrictions Restrictions { get; private set; }
    }

    [Flags]
    public enum ItemRestrictions
    {
        None = 0,
        [EnumDescription("Destroyed")]
        Undestructible = 1 << 0,
        [EnumDescription("Dropped")]
        Undroppable = 1 << 1,
        [EnumDescription("Sold")]
        Unsellable = 1 << 2,
        [EnumDescription("Stored")]
        Unstorable = 1 << 3,
    }
}
