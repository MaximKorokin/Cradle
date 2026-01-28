using Assets._Game.Scripts.Entities.Controllers;
using Assets._Game.Scripts.Entities.Items;
using Assets._Game.Scripts.Entities.Units;

namespace Assets._Game.Scripts.Entities
{
    public class Entity
    {
        public Entity(
            UnitsController unitsController,
            BehaviourController behaviourController,
            Attributes attributes,
            Inventory inventory)
        {
            UnitsController = unitsController;
            BehaviourController = behaviourController;
        }

        public UnitsController UnitsController { get; private set; }
        public BehaviourController BehaviourController { get; private set; }
    }
}