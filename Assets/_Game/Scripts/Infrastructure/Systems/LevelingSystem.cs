using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class LevelingSystem : EntitySystemBase
    {
        private readonly LevelingConfig _levelingConfig;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(StatModule), typeof(LevelingModule) }
            );

        public LevelingSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository,
            LevelingConfig levelingConfig) : base(globalEventBus, repository)
        {
            _levelingConfig = levelingConfig;

            TrackGlobalEvent<AddExperienceRequestEvent>(OnAddExperienceRequested);
        }

        private void OnAddExperienceRequested(AddExperienceRequestEvent e)
        {
            if (e.Target == null)
                return;

            if (!e.Target.TryGetModule(out LevelingModule levelingModule))
                return;

            // todo: add penalty and bonus calculation here

            levelingModule.AddExperience(e.Experience);
        }
    }
}
