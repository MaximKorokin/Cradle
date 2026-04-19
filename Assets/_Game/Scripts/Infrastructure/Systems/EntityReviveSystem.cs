using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems.Location;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class EntityReviveSystem : SystemBase
    {
        private readonly EntityReviveConfig _reviveConfig;

        public EntityReviveSystem(
            IGlobalEventBus globalEventBus,
            EntityReviveConfig reviveConfig) : base(globalEventBus)
        {
            _reviveConfig = reviveConfig;

            TrackGlobalEvent<EntityReviveRequest>(OnEntityReviveRequested);
        }

        private void OnEntityReviveRequested(EntityReviveRequest reviveRequest)
        {
            var entity = reviveRequest.Entity;

            var healthModule = entity.GetModule<HealthModule>();
            healthModule.Heal(_reviveConfig.HealthRestorePercentage * healthModule.MaxHealth);

            entity.GetModule<AppearanceModule>().RequestSetAnimatorValue(Entities.Units.EntityAnimatorParameterName.ToRevive, true);

            entity.GetModule<RestrictionStateModule>().Remove(RestrictionState.Dead);

            if (!reviveRequest.InPlace)
            {
                GlobalEventBus.Publish(new LocationTransitionRequest(_reviveConfig.ReviveLocation.LocationDefinition.Id, _reviveConfig.ReviveLocation.EntranceDefinition.Id));
            }

            GlobalEventBus.Publish(new EntityRevivedEvent(entity));
        }
    }

    public readonly struct EntityReviveRequest : IGlobalEvent
    {
        public readonly Entity Entity;
        public readonly bool InPlace;

        public EntityReviveRequest(Entity entity, bool inPlace)
        {
            Entity = entity;
            InPlace = inPlace;
        }
    }

    public readonly struct EntityRevivedEvent : IGlobalEvent
    {
        public readonly Entity Entity;

        public EntityRevivedEvent(Entity entity)
        {
            Entity = entity;
        }
    }
}
