using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared;
using System;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public interface IPlayerProvider
    {
        IReadOnlyObservableData<string> ObservablePlayerId { get; }
        Entity Player { get; }
    }

    public sealed class PlayerContext : IPlayerProvider
    {
        private readonly PlayerControlProvider _playerControlProvider;

        private readonly ObservableData<string> _observablePlayerId = new(null);

        public Entity Player { get; private set; }

        public IReadOnlyObservableData<string> ObservablePlayerId => _observablePlayerId;

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

        public void SetPlayerAiEnabled(bool enabled)
        {
            Player?.Publish(new EntityAiToggleRequest(enabled));
        }

        public void SetPlayer(Entity player)
        {
            PlayerChanging?.Invoke();

            // Remove the control provider from the old player, if there is one
            if (Player != null && Player.TryGetModule<ControlModule>(out var controlModule))
            {
                controlModule.RemoveProvider(_playerControlProvider);
            }

            Player = player;
            _observablePlayerId.SetData(player.Id);

            // Add the control provider to the new player
            if (Player.TryGetModule(out controlModule))
            {
                controlModule.AddProvider(_playerControlProvider);
            }

            PlayerChanged?.Invoke();
        }
    }
}
