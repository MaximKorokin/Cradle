using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class ItemEnchantingViewController : ViewControllerBase<ItemEnchantingView, ItemEnchantingViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;
        private readonly ItemContainerResolver _itemContainerResolver;

        public ItemEnchantingViewController(
            IGlobalEventBus globalEventBus,
            ItemContainerResolver itemContainerResolver)
        {
            _globalEventBus = globalEventBus;
            _itemContainerResolver = itemContainerResolver;
        }

        public override void Initialize(ItemEnchantingView view)
        {
            base.Initialize(view);

            View.EnchantButtonClicked += OnEnchantButtonClicked;
            View.CancelButtonClicked += OnCancelButtonClicked;
        }

        public override void Dispose()
        {
            View.EnchantButtonClicked -= OnEnchantButtonClicked;
            View.CancelButtonClicked -= OnCancelButtonClicked;

            base.Dispose();
        }

        private void OnEnchantButtonClicked()
        {
            if (!Data.CanEnchant()) return;
            
            _globalEventBus.Publish(new ItemCommandRequest(new EnchantItemCommand(Data.EnchantingItemPath, Data.EnchantableItemPath)));
            Data.RequestNotifyChanged();
        }

        private void OnCancelButtonClicked()
        {

        }
    }
}
