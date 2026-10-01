using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using Assets._Game.Scripts.UI.Windows;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class LocationTransitionListViewController : ViewControllerBase<LocationTransitionListView, ILocationTransitionListViewData>
    {
        private readonly IGlobalEventBus _globalEventBus;

        public LocationTransitionListViewController(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public override void Initialize(LocationTransitionListView view)
        {
            base.Initialize(view);
            View.TransitionButtonClicked += OnTransitionButtonClicked;
        }

        public override void Dispose()
        {
            View.TransitionButtonClicked -= OnTransitionButtonClicked;
            base.Dispose();
        }

        private void OnTransitionButtonClicked(LocationTransitionData transitionData)
        {
            _globalEventBus.Publish(new WindowCloseRequest(View.GetComponentInParent<UIWindowBase>()));
            _globalEventBus.Publish(new LocationTransitionRequest(transitionData.LocationDefinition.Id, transitionData.EntranceDefinition.Id));
        }
    }
}
