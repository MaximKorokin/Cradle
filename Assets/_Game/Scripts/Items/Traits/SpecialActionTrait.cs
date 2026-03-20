using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Traits
{
    public sealed class SpecialActionTrait : ItemTraitBase
    {
        [field: SerializeField]
        public SpecialActionKind Kind { get; private set; }
        [field: SerializeField]
        public ActionDefinition Action { get; private set; }
    }
}
