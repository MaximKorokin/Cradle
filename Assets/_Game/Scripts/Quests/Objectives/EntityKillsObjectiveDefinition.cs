using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using UnityEngine;

namespace Assets._Game.Scripts.Quests.Objectives
{
    public sealed class EntityKillsObjectiveDefinition : ObjectiveDefinition
    {
        [field: SerializeField]
        public EntityDefinition Entity { get; private set; }

        public override ObjectiveProgress CreateProgress()
        {
            return new EntityKillsObjectiveProgress(this);
        }
    }

    public sealed class EntityKillsObjectiveProgress : ObjectiveProgress<EntityKillsObjectiveDefinition>
    {
        public EntityKillsObjectiveProgress(EntityKillsObjectiveDefinition definition) : base(definition)
        {
        }

        public override void HandleEvent(IEvent e)
        {
            if (e is EntityDiedEvent entityDiedEvent && entityDiedEvent.Victim.Definition == Definition.Entity)
            {
                SetProgress(CurrentAmount + 1);
            }
        }
    }
}
