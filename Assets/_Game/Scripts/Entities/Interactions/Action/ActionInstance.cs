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
            Definition = actionDefinition;
            Cooldown = new(Definition.Cooldown);
        }

        public bool CanStartPreparation(InteractionContext context)
        {
            if (!Cooldown.IsOver())
                return false;

            var stats = context.Source.GetModule<StatModule>();
            //return stats.Mana >= 10;
            return true;
        }
        
        public void OnPreparationStart(InteractionContext context)
        {
            //context.Source.GetModule<StatModule>().Mana -= 10;
        }

        public void OnPreparationComplete(InteractionContext context, IObjectResolver resolver)
        {
            _interactionInstance = Definition.Interaction.BuildRuntime(context, resolver);
            _interactionInstance.Start();

            if (Definition.MaxChannelingTime == 0)
            {
                _interactionInstance.Tick(0);
                _interactionInstance.Cancel();
                _interactionInstance = null;
            }
        }

        /// <summary>
        /// return true if the interaction has finished (either completed or failed), false if it is still running
        /// </summary>
        /// <returns>true = finished</returns>
        public bool OnChannelTick(InteractionContext context, float delta)
        {
            if (_interactionInstance == null) return true;

            if (_interactionInstance.Tick(delta))
            {
                _interactionInstance.Cancel();
                _interactionInstance = null;
                return true;
            }
            return false;
        }
    }
}
