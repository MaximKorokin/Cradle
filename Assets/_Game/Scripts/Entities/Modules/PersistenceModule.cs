namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class PersistenceModule : EntityModuleBase
    {
        public string PersistenceKey { get; }

        public PersistenceModule(string persistenceKey)
        {
            PersistenceKey = persistenceKey;
        }
    }

    public sealed class PersistenceModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<PersistenceModuleDefinition>(out var definition))
            {
                return new PersistenceModule(definition.PersistenceKey);
            }
            return null;
        }
    }
} 