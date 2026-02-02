using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Inventory;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public sealed class GameContext
    {
        public Entity Player { get; private set; }
        public EntityInventoryEquipmentModule IEModule { get; private set; }
        public InventoryModel StashInventory { get; private set; }

        public void SetPlayer(Entity player)
        {
            Player = player;
        }

        public void SetIEModule(EntityInventoryEquipmentModule controller)
        {
            IEModule = controller;
        }

        public void SetStash(InventoryModel inventoryModel)
        {
            StashInventory = inventoryModel;
        }
    }
}
