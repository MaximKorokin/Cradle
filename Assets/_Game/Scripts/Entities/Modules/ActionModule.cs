using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class ActionModule : EntityModuleBase
    {
        public bool IsCasting;
        public float RemainingCastTime;

        public bool IsChanneling;
        public float RemainingChannelTime;

        public ActionInstance ActiveAction;
        public InteractionContext ActiveContext;

        public IReadOnlyList<ActionInstance> Actions;

        public CooldownCounter GlobalCooldown = new();

        public ActionModule(IEnumerable<ActionInstance> actions)
        {
            Actions = actions.ToArray();
        }
    }
}
