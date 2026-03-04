using Assets._Game.Scripts.Entities.Interactions.Ability;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AbilityModuleAssembler
    {
        public AbilityModuleAssembler()
        {
        }

        public AbilityModule Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<AbilityModuleDefinition>(out var abilityModuleDefinition))
            {
                return null;
            }

            var abilities = new List<AbilityInstance>();
            foreach (var abilityDefinition in abilityModuleDefinition.Abilities)
            {
                abilities.Add(new(abilityDefinition));
            }
            return new(abilities);
        }
    }
}
