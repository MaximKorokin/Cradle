using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public sealed class PlayerContext
    {
        public Entity Player { get; private set; }
        public StatModule StatsModule { get; private set; }
        public StatusEffectModule StatusEffectModule { get; private set; }
        public InventoryEquipmentModule IEModule { get; private set; }
        public InventoryModel StashInventory { get; private set; }

        public void SetPlayer(Entity player)
        {
            Player = player;
            if (Player.TryGetModule<InventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                IEModule = inventoryEquipmentModule;
            }
            if (Player.TryGetModule<StatModule>(out var statsModule))
            {
                StatsModule = statsModule;
            }
            if (Player.TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                StatusEffectModule = statusEffectModule;
            }
        }

        public void SetStash(InventoryModel inventoryModel)
        {
            StashInventory = inventoryModel;
        }
    }
}
