using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Shared.Extensions;
using System;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public sealed class LootPickupControlProvider : ControlProviderBase
    {
        private readonly IEntitySensor _entitySensor;

        private readonly EntityQuery _entityQuery = new(RestrictionState.Disabled, new[] { typeof(LootItemModule) });

        public override ControlPriority Priority => ControlPriority.OverrideAI;
        public override ControlMask Mask => ControlMask.All;
        public override bool IsActive => _lootPickupModule != null && _targetLootItem != null;

        private LootPickupModule _lootPickupModule;
        private Entity _targetLootItem;

        public LootPickupControlProvider(IEntitySensor entitySensor)
        {
            _entitySensor = entitySensor;
        }

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            if (Entity.TryGetModule<LootPickupModule>(out _lootPickupModule) &&
                _entitySensor.TryGetNearestInRange(Entity, _lootPickupModule.DetectionRange, FactionRelation.None, _entityQuery, out var item))
            {
                _targetLootItem = item;
            }
        }

        public override void Tick(float delta)
        {
            var directionToItem = _targetLootItem.GetModule<SpatialModule>().Position - Entity.GetModule<SpatialModule>().Position;
            var sqrDistanceToItem = directionToItem.sqrMagnitude;

            var intent = Entity.GetModule<IntentModule>();
            //if (sqrDistanceToItem > _lootPickupModule.PickupRange * _lootPickupModule.PickupRange)
            //{
            //    intent.SetMove(new(directionToItem));
            //}
            //else
            //{
            //    intent.SetPickupItem(new(_targetLootItem));
            //    _targetLootItem = null;
            //}
        }
    }

    [Serializable]
    public sealed class LootPickupControlProviderData : ControlProviderData
    {
        public override IControlProvider CreateInstance(IObjectResolver resolver) => new LootPickupControlProvider(resolver.Resolve<IEntitySensor>());
    }
}
