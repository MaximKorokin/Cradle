using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class ItemUseSettingsViewController : ViewControllerBase<ItemUseSettingsView, ItemUseSettingsViewData>
    {
        public override void Initialize(ItemUseSettingsView view)
        {
            base.Initialize(view);

            View.Changed += OnViewChanged;
        }

        public override void Dispose()
        {
            View.Changed -= OnViewChanged;

            base.Dispose();
        }

        private void OnViewChanged(ItemUseSettings itemUseSettings)
        {
            Data.Entity.Publish(new ItemUseSettingsUpdateRequest(itemUseSettings));
        }
    }
}
