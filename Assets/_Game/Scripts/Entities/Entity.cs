using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Entities.Units;

namespace Assets._Game.Scripts.Entities
{
    public class Entity
    {
        public Entity(
            UnitsController unitsController,
            BehaviourController behaviourController,
            Attributes attributes,
            ItemContainersController containersController)
        {
            UnitsController = unitsController;
            BehaviourController = behaviourController;
            ContainersController = containersController;
        }

        public UnitsController UnitsController { get; private set; }
        public BehaviourController BehaviourController { get; private set; }
        public ItemContainersController ContainersController { get; private set; }
    }
}