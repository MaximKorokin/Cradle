namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class CreatureModule : EntityModuleBase
    {
    }

    public sealed class CreatureModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<CreatureModuleDefinition>(out _))
                return new CreatureModule();

            return null;
        }
    }
}
