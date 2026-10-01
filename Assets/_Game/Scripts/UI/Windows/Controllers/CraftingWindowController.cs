using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CraftingWindowController : SingleViewWindowControllerBase<CraftingWindow, CraftingWindowControllerArguments, CraftingView, CraftingViewData, CraftingViewController>
    {
        private readonly CraftingViewData _craftingViewData;

        public CraftingWindowController(
            CraftingViewController craftingViewController,
            CraftingViewData craftingViewData) : base(craftingViewController, craftingViewData)
        {
            _craftingViewData = craftingViewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _craftingViewData.SetCrafterEntity(Arguments.CrafterEntityId);
            _craftingViewData.SetEntityId(Arguments.InventoryEntityId);
        }

        protected override CraftingView GetView() => Window.CraftingView;
    }

    public readonly struct CraftingWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> CrafterEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public CraftingWindowControllerArguments(
            IReadOnlyObservableData<string> crafterEntityId,
            IReadOnlyObservableData<string> inventoryEntityId,
            IReadOnlyObservableData<string> equipmentEntityId)
        {
            CrafterEntityId = crafterEntityId;
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
