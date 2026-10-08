using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.Equipment)]
    public sealed class EquipmentWindowController : SingleViewWindowControllerBase<EquipmentWindow, EquipmentWindowControllerArguments, EquipmentView, IEquipmentViewData, EquipmentViewController>
    {
        public EquipmentWindowController(
            EquipmentViewController equipmentViewController,
            EquipmentViewData equipmentViewData) : base(equipmentViewController, equipmentViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override EquipmentView GetView() => Window.EquipmentView;
    }

    public readonly struct EquipmentWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<EntryRef> EquipmentEntityId { get; }

        public EquipmentWindowControllerArguments(IReadOnlyObservableData<EntryRef> equipmentEntityId)
        {
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
