using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Persistence.Codecs;
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

    public sealed class EntityKillsObjectiveProgress : ObjectiveProgress<EntityKillsObjectiveDefinition>, ISaveableObjectiveProgress, ILoadableObjectiveProgress
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

        public bool TryLoad(object state)
        {
            if (state is QuestObjectiveProgressData data)
            {
                if (data.Payload is string entityId && entityId == Definition.Entity.Id)
                {
                    SetProgress(data.CurrentAmount);
                    return true;
                }
            }
            return false;
        }

        public object Save()
        {
            return new QuestObjectiveProgressData(CurrentAmount, Definition.Entity.Id);
        }
    }
}
