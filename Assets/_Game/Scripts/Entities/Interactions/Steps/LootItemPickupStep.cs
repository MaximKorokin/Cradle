namespace Assets._Game.Scripts.Entities.Interactions.Steps
{
    public sealed class LootItemPickupStep : IInteractionStep
    {
        private bool _done;

        public void Start(in InteractionContext context) => _done = false;

        public void Cancel(in InteractionContext context) => _done = true;

        public StepStatus Tick(in InteractionContext context, float delta)
        {
            if (_done) return StepStatus.Completed;

            //if (Entity.TryGetModule<LootPickupModule>(out var _lootPickupModule) &&
            //    _entitySensor.TryGetNearestInRange(Entity, _lootPickupModule.DetectionRange, FactionRelation.None, _entityQuery, out var item))
            //{
            //    _targetLootItem = item;
            //}

            //var directionToItem = _targetLootItem.GetModule<SpatialModule>().Position - Entity.GetModule<SpatialModule>().Position;
            //var sqrDistanceToItem = directionToItem.sqrMagnitude;

            //var intent = Entity.GetModule<IntentModule>();

            //if (sqrDistanceToItem > _lootPickupModule.PickupRange * _lootPickupModule.PickupRange)
            //{
            //    intent.SetMove(new(directionToItem));
            //}
            //else
            //{
            //    intent.SetPickupItem(new(_targetLootItem));
            //    _targetLootItem = null;
            //}

            _done = true;
            return StepStatus.Completed;
        }
    }
}
