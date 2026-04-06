using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class CameraFollowSystem : SystemBase
    {
        private readonly ICameraService _cameraService;
        private readonly IGlobalEventBus _globalEventBus;

        public CameraFollowSystem(ICameraService cameraService, IGlobalEventBus globalEventBus)
        {
            _cameraService = cameraService;
            _globalEventBus = globalEventBus;

            _globalEventBus.Subscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
        }

        public override void Dispose()
        {
            base.Dispose();

            _globalEventBus.Unsubscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
        }

        public void OnPlayerSpawned(PlayerSpawnedEvent playerTransform)
        {
            _cameraService.Follow(playerTransform.Player);
        }
    }
}
