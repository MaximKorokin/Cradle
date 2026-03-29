using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System.Collections.Generic;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class InteractionSystem : EntitySystemBase, ITickSystem
    {
        private readonly HashSet<InteractionInstance> _activeInteractions = new();
        private readonly List<InteractionInstance> _completedInteractions = new();
        private readonly IObjectResolver _resolver;

        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled);

        public InteractionSystem(EntityRepository repository, IObjectResolver resolver) : base(repository)
        {
            _resolver = resolver;

            TrackEntityEvent<ItemUseStartedEvent>(OnItemUseStarted);
        }

        public void Tick(float delta)
        {
            _completedInteractions.Clear();

            foreach (var interaction in _activeInteractions)
            {
                if (interaction.Tick(delta))
                    _completedInteractions.Add(interaction);
            }

            for (int i = 0; i < _completedInteractions.Count; i++)
                _activeInteractions.Remove(_completedInteractions[i]);
        }

        private void OnItemUseStarted(ItemUseStartedEvent e)
        {
            var trigger = ItemTrigger.OnUse;
            foreach (var trait in e.Item.GetFunctionalTraits<InteractionTrait>(trigger))
            {
                var context = new InteractionContext(e.Entity, e.Entity, e.Entity.GetPosition());
                _activeInteractions.Add(trait.Interaction.BuildRuntime(context, _resolver));
            }
        }
    }
}
