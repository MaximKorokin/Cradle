using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryWindowController : WindowControllerBase<InventoryWindow, InventoryWindowControllerArguments>
    {
        private readonly InventoryViewData _inventoryHudData;
        private readonly InventoryViewController _inventoryViewController;

        public InventoryWindowController(
            InventoryViewData inventoryHudData,
            InventoryViewController inventoryViewController)
        {
            _inventoryHudData = inventoryHudData;
            _inventoryViewController = inventoryViewController;
        }

        protected override void OnBind()
        {
            base.OnBind();

            _inventoryHudData.SetEntityId(Arguments.InventoryEntityId);

            _inventoryViewController.Initialize(Window.InventoryView);
            _inventoryViewController.Bind(_inventoryHudData);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _inventoryViewController.Unbind();
        }

        protected override void Redraw()
        {
            _inventoryViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();

            _inventoryHudData.Dispose();
            _inventoryViewController.Dispose();
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
