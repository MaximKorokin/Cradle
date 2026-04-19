using Assets._Game.Scripts.Entities.Modules;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Control.AI
{
    internal class StandStillBehaviour : IAiBehaviour
    {
        public ActionEvaluation Evaluate(Entity entity)
        {
            return new ActionEvaluation(0.01f, default);
        }

        public void Tick(Entity entity, ActionContext context, float delta)
        {
            var intent = entity.GetModule<IntentModule>();
            intent.ClearAct();
            intent.ClearMove();
        }
    }
}
