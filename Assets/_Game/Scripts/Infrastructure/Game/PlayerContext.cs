using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public sealed class PlayerContext
    {
        public Entity Player { get; private set; }
        public EntityInventoryEquipmentModule IEModule { get; private set; }
        public InventoryModel StashInventory { get; private set; }

        public void SetPlayer(Entity player)
        {
            Player = player;
            if (Player.TryGetModule<EntityInventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                IEModule = inventoryEquipmentModule;
            }
        }

        public void SetStash(InventoryModel inventoryModel)
        {
            StashInventory = inventoryModel;
        }
    }
}
