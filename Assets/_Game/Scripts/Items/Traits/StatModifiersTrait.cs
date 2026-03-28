using Assets._Game.Scripts.Entities.Stats;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    [Serializable]
    public sealed class StatModifiersTrait : FunctionalItemTraitBase
    {
        [field: SerializeField]
        public StatModifier[] Modifiers { get; private set; }
    }
}
