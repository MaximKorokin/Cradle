using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class CameraFollowSystem : SystemBase
    {
        private readonly ICameraService _cameraService;
        private readonly IPlayerProvider _playerProvider;

        public CameraFollowSystem(
            IGlobalEventBus globalEventBus,
            ICameraService cameraService,
            IPlayerProvider playerProvider) : base(globalEventBus)
        {
            _cameraService = cameraService;
            _playerProvider = playerProvider;

            TrackGlobalEvent<EntitySpawnedEvent>(OnEntitySpawned);
        }

        public void OnEntitySpawned(EntitySpawnedEvent e)
        {
            if (e.Entity != _playerProvider.Player) return;

            _cameraService.Follow(e.Entity);
        }
    }
}
