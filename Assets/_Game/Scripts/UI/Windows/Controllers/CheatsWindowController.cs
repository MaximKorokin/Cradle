using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : SingleViewWindowControllerBase<CheatsWindow, CheatsWindowControllerArguments, CheatsView, CheatsViewData, CheatsViewController>
    {
        public CheatsWindowController(
            CheatsViewController cheatsViewController,
            CheatsViewData cheatsViewData) : base(cheatsViewController, cheatsViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewController.SetInventoryEntityId(Arguments.InventoryEntityId);
        }

        protected override CheatsView GetView() => Window.CheatsView;
    }

    public readonly struct CheatsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> InventoryEntityId { get; }

        public CheatsWindowControllerArguments(IReadOnlyObservableData<string> inventoryEntityId)
        {
            InventoryEntityId = inventoryEntityId;
        }
    }
}
