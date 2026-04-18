using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class FloatingTextSystem : EntitySystemBase
    {
        private readonly FloatingTextConfig _floatingTextConfig;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead,
                new[] { typeof(SpatialModule) }
            );

        public FloatingTextSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            FloatingTextConfig floatingTextConfig) : base(globalEventBus, entityRepository)
        {
            _floatingTextConfig = floatingTextConfig;

            TrackGlobalEvent<DamageAppliedEvent>(OnDamageApplied);
            TrackGlobalEvent<HealAppliedEvent>(OnHealApplied);

            TrackEntityEvent<ExperienceChangedEvent>(OnExperienceChanged);
        }

        private void OnDamageApplied(DamageAppliedEvent damageAppliedEvent)
        {
            var damageText = damageAppliedEvent.Damage.ToString();
            var position = damageAppliedEvent.Target.GetModule<SpatialModule>().Position;

            if (damageAppliedEvent.IsCritical)
                GlobalEventBus.Publish(new FloatingTextRequest(damageText, position, _floatingTextConfig.CriticalStyle));
            else
                GlobalEventBus.Publish(new FloatingTextRequest(damageText, position, _floatingTextConfig.DamageStyle));
        }

        private void OnHealApplied(HealAppliedEvent healAppliedEvent)
        {
            var healText = healAppliedEvent.Heal.ToString();
            var position = healAppliedEvent.Target.GetModule<SpatialModule>().Position;

            GlobalEventBus.Publish(new FloatingTextRequest(healText, position, _floatingTextConfig.HealStyle));
        }

        private void OnExperienceChanged(Entity entity, ExperienceChangedEvent experienceChangedEvent)
        {
            var experienceText = $"{experienceChangedEvent.NewAmount - experienceChangedEvent.OldAmount} XP";
            var position = entity.GetModule<SpatialModule>().Position;

            GlobalEventBus.Publish(new FloatingTextRequest(experienceText, position, _floatingTextConfig.ExperienceStyle));
        }
    }

    public readonly struct FloatingTextRequest : IGlobalEvent
    {
        public string Text { get; }
        public Vector3 WorldPosition { get; }
        public FloatingTextStyle Style { get; }

        public FloatingTextRequest(string text, Vector3 position, FloatingTextStyle style)
        {
            Text = text;
            WorldPosition = position;
            Style = style;
        }
    }
}
