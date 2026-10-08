using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Systems;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IEntityAiToggleViewData : IDataAggregator
    {
        bool IsAIEnabled { get; }
        void SetAIEnabled(bool enabled);
    }

    public class EntityAiToggleViewData : EntityBoundDataAggregatorBase, IEntityAiToggleViewData
    {
        public bool IsAIEnabled { get; private set; }

        public EntityAiToggleViewData(EntityRepository entityRepository) : base(entityRepository) { }

        protected override void OnBoundEntityChanged(string entityId)
        {
            IsAIEnabled = false;

            if (Entity != null && Entity.TryGetModule<ControlModule>(out var controlModule))
            {
                foreach (var provider in controlModule.Providers)
                {
                    if (provider is AiControlProvider aiControlProvider)
                    {
                        IsAIEnabled = aiControlProvider.IsEnabled;
                        break;
                    }
                }
            }
        }

        public void SetAIEnabled(bool enabled)
        {
            IsAIEnabled = enabled;
            Entity?.Publish(new EntityAiToggleRequest(enabled));
            NotifyChanged();
        }
    }
}
