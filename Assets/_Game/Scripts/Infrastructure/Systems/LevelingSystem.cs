using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using System;

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

            if (!e.Source.TryGetModule(out LevelingModule sourceLevelingModule) || !e.Target.TryGetModule(out LevelingModule targetLevelingModule))
                return;

            var resultingExperience = (long)(e.Experience * _levelingConfig.ExperienceMultiplier);

            var levelDifference = sourceLevelingModule.Level - targetLevelingModule.Level;

            if (levelDifference < 0 && _levelingConfig.ExperiencePenaltyForOverLeveling != 0)
                resultingExperience += (long)(_levelingConfig.ExperiencePenaltyForOverLeveling * levelDifference * resultingExperience);

            if (levelDifference > 0 && _levelingConfig.ExperienceBonusForUnderLeveling != 0)
                resultingExperience += (long)(_levelingConfig.ExperienceBonusForUnderLeveling * levelDifference * resultingExperience);

            targetLevelingModule.AddExperience(Math.Max(0, resultingExperience));
        }
    }
}
