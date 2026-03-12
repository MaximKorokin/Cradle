using Assets._Game.Scripts.Entities.Interactions.Action;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class ActionModuleAssembler
    {
        public ActionModuleAssembler()
        {
        }

        public ActionModule Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<ActionModuleDefinition>(out var actionModuleDefinition))
            {
                return null;
            }

            var actions = new List<ActionInstance>();
            foreach (var definition in actionModuleDefinition.Actions)
            {
                actions.Add(new(definition));
            }
            return new(actions);
        }
    }
}
