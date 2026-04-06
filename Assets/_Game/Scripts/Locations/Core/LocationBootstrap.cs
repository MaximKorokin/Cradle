using Assets._Game.Scripts.Infrastructure.Game;
using VContainer.Unity;

namespace Assets._Game.Scripts.Locations.Core
{
    public sealed class LocationBootstrap : IStartable
    {
        private readonly LocationBordersContext _locationBordersContext;
        private readonly ICameraService _cameraService;

        public LocationBootstrap(LocationBordersContext locationBordersContext, ICameraService cameraService)
        {
            _locationBordersContext = locationBordersContext;
            _cameraService = cameraService;
        }

        public void Start()
        {
            _cameraService.SetConfiner(_locationBordersContext.CameraBorder);
        }
    }
}
