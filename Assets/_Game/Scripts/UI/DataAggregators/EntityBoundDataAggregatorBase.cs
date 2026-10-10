using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Shared;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public abstract class EntityBoundDataAggregatorBase : DataAggregatorBase, IEntityBoundDataAggregatorBase
    {
        protected readonly EntityRepository EntityRepository;

        public IReadOnlyObservableData<EntryRef> ObservableEntityId { get; private set; }
        public string EntityId => ObservableEntityId?.Value.Id;
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
                ObservableEntityId.ValueChanged -= OnEntryRefChanged;
            }
        }

        public void SetEntityId(IReadOnlyObservableData<EntryRef> observableEntityId)
        {
            if (ObservableEntityId == observableEntityId) return;

            if (ObservableEntityId != null)
            {
                ObservableEntityId.ValueChanged -= OnEntryRefChanged;
            }

            ObservableEntityId = observableEntityId;
            Entity = null;

            if (ObservableEntityId != null)
            {
                ObservableEntityId.ValueChanged += OnEntryRefChanged;
                ApplyEntryRef(ObservableEntityId.Value);
            }
            else
            {
                OnBoundEntityChanged();
            }

            NotifyChanged();
        }

        private void OnEntryRefChanged(EntryRef entryRef)
        {
            ApplyEntryRef(entryRef);
            NotifyChanged();
        }

        private void ApplyEntryRef(EntryRef entryRef)
        {
            if (entryRef.IsBound && !entryRef.Exists)
            {
                Entity = null;
                NotifyInvalidated();
                return;
            }

            EntityRepository.TryGet(entryRef, out var entity);
            Entity = entity;
            OnBoundEntityChanged();
        }

        /// <summary>Triggers when the bound entity changes. <see cref="EntityId"/> is null when nothing is bound.</summary>
        protected abstract void OnBoundEntityChanged();
    }

    public interface IEntityBoundDataAggregatorBase : IDataAggregator
    {
        void SetEntityId(IReadOnlyObservableData<EntryRef> observableEntityId);
    }
}
