using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.ItemEnchanting)]
    public sealed class ItemEnchantingWindowController : SingleViewWindowControllerBase<ItemEnchantingWindow, ItemEnchantingWindowControllerArguments, ItemEnchantingView, ItemEnchantingViewData, ItemEnchantingViewController>
    {
        public ItemEnchantingWindowController(
            ItemEnchantingViewController viewController,
            ItemEnchantingViewData viewData) : base(viewController, viewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.EntityId);
            ViewData.SetEnchantingItem(Arguments.EnchantingItemStackPath);
            ViewData.SetEnchantableItem(Arguments.EnchantableItemStackPath);
        }

        protected override ItemEnchantingView GetView() => Window.EnchantingView;
    }

    public readonly struct ItemEnchantingWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<EntryRef> EntityId { get; }
        public readonly ItemStackPath EnchantingItemStackPath;
        public readonly ItemStackPath EnchantableItemStackPath;

        public ItemEnchantingWindowControllerArguments(IReadOnlyObservableData<EntryRef> entityId, ItemStackPath enchantingItemStackPath, ItemStackPath enchantableItemStackPath)
        {
            EntityId = entityId;
            EnchantingItemStackPath = enchantingItemStackPath;
            EnchantableItemStackPath = enchantableItemStackPath;
        }
    }
}
