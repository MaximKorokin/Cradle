using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Entities.Units;

namespace Assets._Game.Scripts.Entities
{
    public class Entity
    {
        public Entity(
            string id,
            EntityAttributesModel attributes,
            UnitsController unitsController,
            BehaviourController behaviourController,
            InventoryEquipmentController containersController)
        {
            Id = id;

            Attributes = attributes;
            UnitsController = unitsController;
            BehaviourController = behaviourController;
            ContainersController = containersController;
        }

        public string Id { get; private set; }

        public EntityAttributesModel Attributes { get; private set; }
        public UnitsController UnitsController { get; private set; }
        public BehaviourController BehaviourController { get; private set; }
        public InventoryEquipmentController ContainersController { get; private set; }
    }
}
