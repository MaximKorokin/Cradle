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
        private readonly IGlobalEventBus _globalEventBus;
        private readonly LevelingConfig _levelingConfig;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(StatModule), typeof(LevelingModule) }
            );

        public LevelingSystem(EntityRepository repository, IGlobalEventBus globalEventBus, LevelingConfig levelingConfig) : base(repository)
        {
            _globalEventBus = globalEventBus;
            _levelingConfig = levelingConfig;

            _globalEventBus.Subscribe<AddExperienceRequestEvent>(OnAddExperienceRequested);
            TrackEntityEvent<LevelChangedEvent>(OnLevelChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            _globalEventBus.Unsubscribe<AddExperienceRequestEvent>(OnAddExperienceRequested);
        }

        private void OnLevelChanged(LevelChangedEvent levelChangedEvent)
        {
            var statModule = levelChangedEvent.Entity.GetModule<StatModule>();

            statModule.RemoveModifiers(StatModifierSource.Level);
            statModule.AddModifiers(
                StatModifierSource.Level,
                Enumerable.Repeat(0, levelChangedEvent.NewLevel).SelectMany(_ => _levelingConfig.StatModifiersOnLevelUp));
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
