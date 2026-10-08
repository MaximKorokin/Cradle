using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.Inventory)]
    public sealed class InventoryWindowController : SingleViewWindowControllerBase<InventoryWindow, InventoryWindowControllerArguments, InventoryView, IInventoryViewData, InventoryViewController>
    {
        public InventoryWindowController(
            InventoryViewController inventoryViewController,
            InventoryViewData inventoryViewData) : base(inventoryViewController, inventoryViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.InventoryEntityId);
        }

        protected override InventoryView GetView() => Window.InventoryView;
    }

    public readonly struct InventoryWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<EntryRef> InventoryEntityId { get; }

        public InventoryWindowControllerArguments(IReadOnlyObservableData<EntryRef> inventoryEntityId)
        {
            InventoryEntityId = inventoryEntityId;
        }
    }
}
