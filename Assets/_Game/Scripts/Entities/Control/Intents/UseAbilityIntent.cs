using Assets._Game.Scripts.Entities.Interactions.Ability;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct UseAbilityIntent
    {
        public readonly AbilityInstance AbilityInstance;
        // can be null if the ability doesn't require a target
        public readonly Entity Target;
        // for ground-targeted abilities, the point where the ability should be used. If HasPoint is false, this value should be ignored.
        public readonly Vector2? Point;

        public static UseAbilityIntent None { get; } = default;

        public UseAbilityIntent(AbilityInstance abilityInstance, Entity target, Vector2? point)
        {
            AbilityInstance = abilityInstance;
            Target = target;
            Point = point;
        }
    }
}
