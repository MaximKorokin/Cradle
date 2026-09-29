using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryWindowController : SingleViewWindowControllerBase<InventoryWindow, InventoryWindowControllerArguments, InventoryView, IInventoryViewData, InventoryViewController>
    {
        private readonly InventoryViewData _inventoryViewData;

        public InventoryWindowController(
            InventoryViewController inventoryViewController,
            InventoryViewData inventoryViewData) : base(inventoryViewController, inventoryViewData)
        {
            _inventoryViewData = inventoryViewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _inventoryViewData.SetEntityId(Arguments.InventoryEntityId);
        }

        protected override InventoryView GetView() => Window.InventoryView;
    }

    public readonly struct InventoryWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> InventoryEntityId { get; }

        public InventoryWindowControllerArguments(IReadOnlyObservableData<string> inventoryEntityId)
        {
            InventoryEntityId = inventoryEntityId;
        }
    }
}
