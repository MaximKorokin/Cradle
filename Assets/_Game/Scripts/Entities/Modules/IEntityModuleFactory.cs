using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Entities.Modules
{
    public interface IEntityModuleFactory
    {
        EntityModuleBase Create(EntityDefinition entityDefinition);
    }

    public interface IEntityModulePersistance
    {
        void Apply(Entity entity, EntitySave entitySave);
        void Save(Entity entity, EntitySave entitySave);
    }
}
