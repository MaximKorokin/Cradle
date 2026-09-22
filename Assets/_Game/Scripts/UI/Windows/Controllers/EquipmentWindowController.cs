using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class EquipmentWindowController : WindowControllerBase<EquipmentWindow, EquipmentWindowControllerArguments>
    {
        private readonly EquipmentHudData _equipmentHudData;
        private readonly EquipmentViewController _equipmentViewController;

        public EquipmentWindowController(
            EquipmentHudData equipmentHudData,
            EquipmentViewController equipmentViewController)
        {
            _equipmentHudData = equipmentHudData;
            _equipmentViewController = equipmentViewController;
        }

        protected override void OnBind()
        {
            base.OnBind();

            _equipmentHudData.SetEntityId(Arguments.EquipmentEntityId);

            _equipmentViewController.Initialize(Window.EquipmentView);
            _equipmentViewController.Bind(_equipmentHudData);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _equipmentViewController.Unbind();
        }

        protected override void Redraw()
        {
            _equipmentViewController.Redraw();
        }

        public override void Dispose()
        {
            base.Dispose();

            _equipmentHudData.Dispose();
            _equipmentViewController.Dispose();
        }
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
