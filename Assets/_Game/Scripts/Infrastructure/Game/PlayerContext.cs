using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using System;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public interface IPlayerProvider
    {
        Entity Player { get; }
    }

    public sealed class PlayerContext : IPlayerProvider
    {
        private readonly PlayerControlProvider _playerControlProvider;

        public Entity Player { get; private set; }

        public event Action PlayerChanging;
        public event Action PlayerChanged;

        public PlayerContext(PlayerControlProvider playerControlProvider)
        {
            _playerControlProvider = playerControlProvider;
        }

        public T GetModule<T>() where T : class, IEntityModule
            => Player.GetModule<T>();

        public bool TryGetModule<T>(out T module) where T : class, IEntityModule
            => Player.TryGetModule(out module);

        public void SetPlayer(Entity player)
        {
            PlayerChanging?.Invoke();

            // Remove the control provider from the old player, if there is one
            if (Player != null && Player.TryGetModule<ControlModule>(out var controlModule))
            {
                controlModule.RemoveProvider(_playerControlProvider);
            }

            Player = player;

            // Add the control provider to the new player
            if (Player.TryGetModule(out controlModule))
            {
                controlModule.AddProvider(_playerControlProvider);
            }

            PlayerChanged?.Invoke();
        }
    }
}
