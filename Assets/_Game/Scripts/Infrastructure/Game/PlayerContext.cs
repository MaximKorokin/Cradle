using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Inventory;
using System;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public sealed class PlayerContext
    {
        private readonly PlayerControlProvider _playerControlProvider;

        public Entity Player { get; private set; }
        public StatModule StatModule { get; private set; }
        public LevelingModule LevelingModule { get; private set; }
        public StatusEffectModule StatusEffectModule { get; private set; }
        public InventoryEquipmentModule IEModule { get; private set; }
        public InventoryModel StashInventory { get; private set; }
        public SpatialModule SpatialModule { get; private set; }

        public event Action PlayerChanging;
        public event Action PlayerChanged;

        public PlayerContext(PlayerControlProvider playerControlProvider)
        {
            _playerControlProvider = playerControlProvider;
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

            // Add the control provider to the new player
            if (Player.TryGetModule(out controlModule))
            {
                controlModule.AddProvider(_playerControlProvider);
            }

            // Try to get the modules we care about from the new player
            if (Player.TryGetModule<InventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                IEModule = inventoryEquipmentModule;
            }
            if (Player.TryGetModule<StatModule>(out var statsModule))
            {
                StatModule = statsModule;
            }
            if (Player.TryGetModule<LevelingModule>(out var levelingModule))
            {
                LevelingModule = levelingModule;
            }
            if (Player.TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                StatusEffectModule = statusEffectModule;
            }
            if (Player.TryGetModule<SpatialModule>(out var spatialModule))
            {
                SpatialModule = spatialModule;
            }

            PlayerChanged?.Invoke();
        }

        public void SetStash(InventoryModel inventoryModel)
        {
            StashInventory = inventoryModel;
        }
    }
}
