using VContainer;

namespace Assets._Game.Scripts.Entities.Modules
{
    public interface IEntityModuleFactory
    {
        EntityModuleBase Create(EntityDefinition entityDefinition);
    }

    public interface IEntityModulePersistance<T, S> where T : IEntityModule
    {
        void Apply(T module, S save);
        S Save(T module);
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
