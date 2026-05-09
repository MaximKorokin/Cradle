using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Quests.Objectives
{
    public sealed class LevelObjectiveDefinition : ObjectiveDefinition
    {
        public override ObjectiveProgress CreateProgress()
        {
            return new LevelObjectiveProgress(this);
        }
    }

    public sealed class LevelObjectiveProgress : ObjectiveProgress<LevelObjectiveDefinition>
    {
        public LevelObjectiveProgress(LevelObjectiveDefinition definition) : base(definition)
        {
        }

        public override void HandleEvent(IEvent e)
        {
            if (e is LevelChangedEvent levelChangedEvent)
            {
                SetProgress(levelChangedEvent.NewLevel);
            }
        }
    }
}
