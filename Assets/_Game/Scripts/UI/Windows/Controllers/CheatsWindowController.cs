using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : SingleViewWindowControllerBase<CheatsWindow, CheatsWindowControllerArguments, CheatsView, CheatsViewData, CheatsViewController>
    {
        private readonly CheatsViewController _cheatsViewController;
        private readonly EquipmentViewData _equipmentViewData;

        public CheatsWindowController(
            CheatsViewController cheatsViewController,
            CheatsViewData cheatsViewData,
            EquipmentViewData equipmentViewData) : base(cheatsViewController, cheatsViewData)
        {
            _cheatsViewController = cheatsViewController;
            _equipmentViewData = equipmentViewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _cheatsViewController.SetInventoryEntityId(Arguments.InventoryEntityId);
            _equipmentViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override CheatsView GetView() => Window.CheatsView;

        public override void Dispose()
        {
            base.Dispose();

            _equipmentViewData.Dispose();
        }
    }

    public readonly struct CheatsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public CheatsWindowControllerArguments(IReadOnlyObservableData<string> inventoryEntityId, IReadOnlyObservableData<string> equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
