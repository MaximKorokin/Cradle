using Assets._Game.Scripts.Entities.Modules;
using Assets.CoreScripts;
using VContainer;

namespace Assets._Game.Scripts.Entities.Interactions.Action
{
    public sealed class ActionInstance
    {
        public readonly CooldownCounter Cooldown;
        public readonly ActionDefinition Definition;

        private InteractionInstance _interactionInstance;

        public ActionInstance(ActionDefinition actionDefinition)
        {
            Cooldown = new(actionDefinition.Cooldown);
            Definition = actionDefinition;
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
