namespace Assets._Game.Scripts.Entities
{
    public class Entity
    {
        public Entity(EntityUnitsController unitsController)
        {
            UnitsController = unitsController;
        }

        public EntityUnitsController UnitsController { get; private set; }
    }
}