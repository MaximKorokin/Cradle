using Assets._Game.Scripts.Entities.Interactions.Action;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.Intents
{
    public readonly struct ActionIntent : IIntent
    {
        public readonly ActionInstance ActionInstance;
        // can be null if the action doesn't require a target
        public readonly Entity Target;
        // for ground-targeted actions, the point where the action should be used. If HasPoint is false, this value should be ignored.
        public readonly Vector2? Point;

        IIntent IIntent.None => None;
        public static ActionIntent None => default;

        public ActionIntent(ActionInstance actionInstance, Entity target, Vector2? point)
        {
            ActionInstance = actionInstance;
            Target = target;
            Point = point;
        }
    }
}
