using Assets._Game.Scripts.Shared;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public abstract class EntityBoundDataAggregatorBase : DataAggregatorBase, IEntityBoundDataAggregatorBase
    {
        private IReadOnlyObservableData<string> ObservableEntityId;

        public string EntityId => ObservableEntityId?.Value;

        public override void Dispose()
        {
            base.Dispose();

            SetEntityId(null);
        }

        public void SetEntityId(IReadOnlyObservableData<string> observableEntityId)
        {
            if (ObservableEntityId == observableEntityId) return;

            if (ObservableEntityId != null)
            {
                ObservableEntityId.ValueChanged -= OnBoundEntityChanged;
            }

            ObservableEntityId = observableEntityId;

            if (ObservableEntityId != null)
            {
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
