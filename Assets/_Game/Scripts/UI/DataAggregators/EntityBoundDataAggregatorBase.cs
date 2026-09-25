using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Shared;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public abstract class EntityBoundDataAggregatorBase : DataAggregatorBase, IEntityBoundDataAggregatorBase
    {
        protected readonly EntityRepository EntityRepository;

        private IReadOnlyObservableData<string> ObservableEntityId;

        public string EntityId => ObservableEntityId?.Value;
        public Entity Entity { get; private set; }

        public EntityBoundDataAggregatorBase(EntityRepository entityRepository)
        {
            EntityRepository = entityRepository;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (ObservableEntityId != null)
            {
                ObservableEntityId.ValueChanged -= OnBoundEntityChanged;
            }
        }

        public void SetEntityId(IReadOnlyObservableData<string> observableEntityId)
        {
            if (ObservableEntityId == observableEntityId) return;

            if (ObservableEntityId != null)
            {
                ObservableEntityId.ValueChanged -= OnBoundEntityChanged;
            }

            ObservableEntityId = observableEntityId;
            Entity = null;

            if (ObservableEntityId != null)
            {
                Entity = EntityRepository.Get(ObservableEntityId.Value);
                ObservableEntityId.ValueChanged += OnBoundEntityChanged;
            }

            OnBoundEntityChanged(ObservableEntityId?.Value);
        }

        /// <summary>Triggers when ObservableEntityId.Value is changed</summary>
        protected abstract void OnBoundEntityChanged(string entityId);
    }

    public interface IEntityBoundDataAggregatorBase
    {
        void SetEntityId(IReadOnlyObservableData<string> observableEntityId);
    }
}
