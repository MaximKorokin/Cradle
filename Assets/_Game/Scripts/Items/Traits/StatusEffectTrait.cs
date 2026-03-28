using Assets._Game.Scripts.Entities.StatusEffects;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class StatusEffectTrait : FunctionalItemTraitBase
    {
        [field: SerializeField]
        public StatusEffectDefinition StatusEffect { get; private set; }
    }
}
