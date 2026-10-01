using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class LocationTransitionListWindowController : SingleViewWindowControllerBase<LocationTransitionListWindow, LocationTransitionListWindowControllerArguments, LocationTransitionListView, ILocationTransitionListViewData, LocationTransitionListViewController>
    {
        private readonly LocationConfig _locationConfig;
        private readonly EntityRepository _entityRepository;
        private readonly LocationTransitionListViewData _viewData;

        public LocationTransitionListWindowController(
            LocationTransitionListViewController viewController,
            LocationTransitionListViewData viewData,
            LocationConfig locationConfig,
            EntityRepository entityRepository) : base(viewController, viewData)
        {
            _locationConfig = locationConfig;
            _entityRepository = entityRepository;
            _viewData = viewData;
        }

        protected override void Redraw()
        {
            var playerLevel = _entityRepository.Get(Arguments.EntityId.Value).GetModule<LevelingModule>().Level;
            var locations = _locationConfig.GetAvailableLocations(playerLevel);
            _viewData.SetTransitions(locations);

            base.Redraw();
        }

        protected override LocationTransitionListView GetView() => Window.LocationTransitionListView;
    }

    public readonly struct LocationTransitionListWindowControllerArguments : IWindowControllerArguments
    {
        public readonly IReadOnlyObservableData<string> EntityId;

        public LocationTransitionListWindowControllerArguments(IReadOnlyObservableData<string> entityId)
        {
            EntityId = entityId;
        }
    }
}
