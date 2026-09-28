using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class InventoryWindowController : WindowControllerBase<InventoryWindow, InventoryWindowControllerArguments>
    {
        private readonly InventoryViewData _inventoryViewData;
        private readonly InventoryViewController _inventoryViewController;

        public InventoryWindowController(
            InventoryViewData inventoryViewData,
            InventoryViewController inventoryViewController)
        {
            _inventoryViewData = inventoryViewData;
            _inventoryViewController = inventoryViewController;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _inventoryViewData.SetEntityId(Arguments.InventoryEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _inventoryViewController.Initialize(Window.InventoryView);
            _inventoryViewController.Bind(_inventoryViewData);
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

            _inventoryViewData.Dispose();
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
