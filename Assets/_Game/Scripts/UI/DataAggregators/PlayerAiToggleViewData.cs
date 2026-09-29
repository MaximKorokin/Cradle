using Assets._Game.Scripts.Entities;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IPlayerAiToggleViewData : IDataAggregator
    {
        bool IsAIEnabled { get; }
        void SetAIEnabled(bool enabled);
    }

    public class PlayerAiToggleViewData : EntityBoundDataAggregatorBase, IPlayerAiToggleViewData
    {
        public bool IsAIEnabled { get; private set; }

        public PlayerAiToggleViewData(EntityRepository entityRepository) : base(entityRepository) { }

        protected override void OnBoundEntityChanged(string entityId)
        {
        }

        public void SetAIEnabled(bool enabled)
        {
            IsAIEnabled = enabled;
            NotifyChanged();
        }
    }
}
