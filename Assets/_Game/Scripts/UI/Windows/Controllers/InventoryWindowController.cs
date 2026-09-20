using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryWindowController : WindowControllerBase<InventoryWindow, InventoryWindowControllerArguments>
    {
        private readonly InventoryHudData _inventoryHudData;
        private readonly InventoryViewController _inventoryViewController;

        public InventoryWindowController(
            InventoryHudData inventoryHudData,
            InventoryViewController inventoryViewController)
        {
            _inventoryHudData = inventoryHudData;
            _inventoryViewController = inventoryViewController;
        }

        public override void Bind(InventoryWindow window)
        {
            _inventoryHudData.SetEntityId(Arguments.InventoryEntityId);

            _inventoryViewController.Initialize(window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);
            Redraw();
        }

        public override void Unbind()
        {
            base.Unbind();

            _inventoryViewController.Unbind();
        }

        private void Redraw()
        {
            _inventoryViewController.Redraw();
        }
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
