using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class CameraFollowSystem : SystemBase
    {
        private readonly ICameraService _cameraService;

        public CameraFollowSystem(
            IGlobalEventBus globalEventBus,
            ICameraService cameraService) : base(globalEventBus)
        {
            _cameraService = cameraService;

            TrackGlobalEvent<PlayerSpawnedEvent>(OnPlayerSpawned);
        }

        public void OnPlayerSpawned(PlayerSpawnedEvent playerTransform)
        {
            _cameraService.Follow(playerTransform.Player);
        }
    }
}
