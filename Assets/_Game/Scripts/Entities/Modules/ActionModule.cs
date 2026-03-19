using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;
using VContainer;

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

    public sealed class ActionModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<ActionModuleDefinition>(out var moduleDefinition))
            {
                return null;
            }

            var actions = new List<ActionInstance>();
            foreach (var definition in moduleDefinition.Actions)
            {
                actions.Add(new(definition));
            }
            return new ActionModule(actions);
        }
    }
}
