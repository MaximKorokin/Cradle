namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class LootPickupModule : EntityModuleBase
    {
        public float DetectionRange { get; }

        public LootPickupModule(float lootDetectionange)
        {
            DetectionRange = lootDetectionange;
        }
    }

    public sealed class LootPickupModuleFactory
    {
        public LootPickupModuleFactory()
        {

        }

        public LootPickupModule Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<LootPickupModuleDefinition>(out var module))
            {
                return new(module.DetectionRange);
            }
            return null;
        }
    }
}
