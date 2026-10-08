using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class EntityAiToggleViewController : ViewControllerBase<EntityAiToggleView, IEntityAiToggleViewData>
    {
        public override void Initialize(EntityAiToggleView view)
        {
            base.Initialize(view);
            View.ValueChanged += OnValueChanged;
        }

        public override void Dispose()
        {
            if (View != null)
                View.ValueChanged -= OnValueChanged;

            base.Dispose();
        }

        private void OnValueChanged(bool enabled)
        {
            Data.SetAIEnabled(enabled);
        }
    }
}
