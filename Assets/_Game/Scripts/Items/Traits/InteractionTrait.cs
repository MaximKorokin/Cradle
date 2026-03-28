using Assets._Game.Scripts.Entities.Interactions;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class InteractionTrait : FunctionalItemTraitBase
    {
        [field: SerializeField]
        public InteractionDefinition Interaction { get; private set; }
    }
}
