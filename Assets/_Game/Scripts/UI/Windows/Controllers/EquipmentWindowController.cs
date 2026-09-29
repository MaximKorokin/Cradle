using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class EquipmentWindowController : SingleViewWindowControllerBase<EquipmentWindow, EquipmentWindowControllerArguments, EquipmentView, IEquipmentViewData, EquipmentViewController>
    {
        private readonly EquipmentViewData _equipmentViewData;

        public EquipmentWindowController(
            EquipmentViewController equipmentViewController,
            EquipmentViewData equipmentViewData) : base(equipmentViewController, equipmentViewData)
        {
            _equipmentViewData = equipmentViewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override EquipmentView GetView() => Window.EquipmentView;
    }

    public readonly struct EquipmentWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public EquipmentWindowControllerArguments(IReadOnlyObservableData<string> equipmentEntityId)
        {
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
