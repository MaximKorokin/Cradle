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

            ViewData.SetCrafterEntity(Arguments.CrafterEntityId);
            ViewData.SetEntityId(Arguments.InventoryEntityId);
        }

        protected override CraftingView GetView() => Window.CraftingView;
    }

    public readonly struct CraftingWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> CrafterEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }

        public CraftingWindowControllerArguments(
            IReadOnlyObservableData<string> crafterEntityId,
            IReadOnlyObservableData<string> inventoryEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
        }
    }
}
