using Assets._Game.Scripts.Entities.Modules;
using Assets.CoreScripts;
using VContainer;

namespace Assets._Game.Scripts.Entities.Interactions.Ability
{
    public sealed class AbilityInstance
    {
        public readonly CooldownCounter Cooldown;
        public readonly AbilityDefinition Definition;

        private InteractionInstance _interactionInstance;

        public AbilityInstance(AbilityDefinition abilityDefinition)
        {
            Cooldown = new(abilityDefinition.Cooldown);
            Definition = abilityDefinition;
        }

        public bool CanStartCast(InteractionContext context)
        {
            if (!Cooldown.IsOver())
                return false;

            var stats = context.Source.GetModule<StatModule>();
            //return stats.Mana >= 10;
            return true;
        }
        
        public void OnCastStart(InteractionContext context)
        {
            // cast animation, play a sound, etc.
            //context.Source.GetModule<StatModule>().Mana -= 10;
        }

        public void OnCastComplete(InteractionContext context, IObjectResolver resolver)
        {
            _interactionInstance = Definition.Interaction.BuildRuntime(context, resolver);
            _interactionInstance.Start();
            _interactionInstance.Tick(0);
        }

        public void OnChannelTick(InteractionContext context, float delta)
        {
            if (_interactionInstance == null) return;

            if (_interactionInstance.Tick(delta))
            {
                _interactionInstance.Cancel();
                _interactionInstance = null;
            }
        }
    }
}
