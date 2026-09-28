using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class EquipmentWindowController : WindowControllerBase<EquipmentWindow, EquipmentWindowControllerArguments>
    {
        private readonly EquipmentViewData _equipmentViewData;
        private readonly EquipmentViewController _equipmentViewController;

        public EquipmentWindowController(
            EquipmentViewData equipmentViewData,
            EquipmentViewController equipmentViewController)
        {
            _equipmentViewData = equipmentViewData;
            _equipmentViewController = equipmentViewController;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            _equipmentViewController.Initialize(Window.EquipmentView);
            _equipmentViewController.Bind(_equipmentViewData);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _equipmentViewController.Unbind();
        }

        protected override void Redraw()
        {
            _equipmentViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();

            _equipmentViewData.Dispose();
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
