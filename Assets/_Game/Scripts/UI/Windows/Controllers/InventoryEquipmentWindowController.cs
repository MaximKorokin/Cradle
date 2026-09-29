using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    /// <summary>
    /// Example of a window hosting more than one view. Each view is registered via
    /// <see cref="MultiViewWindowControllerBase{TWindow, TArguments}.RegisterView{TView, TData, TViewController}"/>,
    /// and the base class takes care of binding/unbinding/redrawing/disposing all of them.
    /// </summary>
    public sealed class InventoryEquipmentWindowController : MultiViewWindowControllerBase<InventoryEquipmentWindow, InventoryEquipmentWindowControllerArguments>
    {
        private readonly InventoryViewData _inventoryViewData;
        private readonly EquipmentViewData _equipmentViewData;

        public InventoryEquipmentWindowController(
            InventoryViewController inventoryViewController,
            InventoryViewData inventoryViewData,
            EquipmentViewController equipmentViewController,
            EquipmentViewData equipmentViewData)
        {
            _inventoryViewData = inventoryViewData;
            _equipmentViewData = equipmentViewData;

            RegisterView<InventoryView, IInventoryViewData, InventoryViewController>(inventoryViewController, inventoryViewData, () => Window.InventoryView);
            RegisterView<EquipmentView, IEquipmentViewData, EquipmentViewController>(equipmentViewController, equipmentViewData, () => Window.EquipmentView);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _inventoryViewData.SetEntityId(Arguments.InventoryEntityId);
            _equipmentViewData.SetEntityId(Arguments.EquipmentEntityId);
        }
    }

    public readonly struct InventoryEquipmentWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public InventoryEquipmentWindowControllerArguments(
            IReadOnlyObservableData<string> inventoryEntityId,
            IReadOnlyObservableData<string> equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
