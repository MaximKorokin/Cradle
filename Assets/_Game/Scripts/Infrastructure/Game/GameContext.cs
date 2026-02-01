using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public sealed class GameContext
    {
        public Entity Player { get; private set; }
        public InventoryEquipmentController IEController { get; private set; }

        public void SetPlayer(Entity player)
        {
            Player = player;
        }

        public void SetIEController(InventoryEquipmentController controller)
        {
            IEController = controller;
        }
    }
}