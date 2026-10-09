using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.Crafting)]
    public sealed class CraftingWindowController : SingleViewWindowControllerBase<CraftingWindow, CraftingWindowControllerArguments, CraftingView, CraftingViewData, CraftingViewController>
    {
        public CraftingWindowController(
            CraftingViewController craftingViewController,
            CraftingViewData craftingViewData) : base(craftingViewController, craftingViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.CrafterEntityId);
            ViewData.SetConsumerEntity(Arguments.InventoryEntityId);
        }

        protected override CraftingView GetView() => Window.CraftingView;
    }

    public readonly struct CraftingWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<EntryRef> CrafterEntityId { get; }
        public IReadOnlyObservableData<EntryRef> InventoryEntityId { get; }

        public CraftingWindowControllerArguments(
            IReadOnlyObservableData<EntryRef> crafterEntityId,
            IReadOnlyObservableData<EntryRef> inventoryEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }
}
