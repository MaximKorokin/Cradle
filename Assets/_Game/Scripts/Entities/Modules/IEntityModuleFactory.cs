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

    //public abstract class EntityModuleFactory<T, D> : IEntityModuleFactory<T>
    //    where T : IEntityModule
    //    where D : EntityModuleDefinition
    //{
    //    protected readonly IObjectResolver Resolver;

    //    public EntityModuleFactory(IObjectResolver resolver)
    //    {
    //        Resolver = resolver;
    //    }

    //    public virtual T Create(EntityDefinition entityDefinition)
    //    {
    //        if (entityDefinition.TryGetModuleDefinition<D>(out var moduleDefinition))
    //        {
    //            return CreateModule(moduleDefinition);
    //        }
    //        return default;
    //    }

    //    protected abstract T CreateModule(D moduleDefinition);
    //}
}
