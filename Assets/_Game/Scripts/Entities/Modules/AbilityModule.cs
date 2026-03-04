using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Ability;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AbilityModule : EntityModuleBase
    {
        public bool IsCasting;
        public float RemainingCastTime;

        public bool IsChanneling;
        public float RemainingChannelTime;

        public AbilityInstance ActiveAbility;
        public InteractionContext ActiveContext;

        public IEnumerable<AbilityInstance> Abilities;

        public CooldownCounter GlobalCooldown = new();

        public AbilityModule(IEnumerable<AbilityInstance> abilities)
        {
            Abilities = abilities.ToArray();
        }
    }
}
