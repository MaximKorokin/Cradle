using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities
{
    public class EntityFactory
    {
        private readonly IReadOnlyList<IEntityModuleFactory> _moduleFactories;
        private readonly IReadOnlyList<IEntityModulePersistance> _modulePersistances;

        public EntityFactory(
            IReadOnlyList<IEntityModuleFactory> moduleFactories,
            IReadOnlyList<IEntityModulePersistance> modulePersistances)
        {
            _moduleFactories = moduleFactories;
            _modulePersistances = modulePersistances;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entity = new Entity(entityDefinition);

            for (int i = 0; i < _moduleFactories.Count; i++)
            {
                var module = _moduleFactories[i].Create(entityDefinition);
                if (module != null)
                {
                    entity.AddModule(module);
                }
            }

            return entity;
        }

        public void Apply(Entity entity, EntitySave save)
        {
            for (int i = 0; i < _modulePersistances.Count; i++)
            {
                _modulePersistances[i].Apply(entity, save);
            }
        }

        public EntitySave Save(Entity entity)
        {
            var save = new EntitySave();
            for (int i = 0; i < _modulePersistances.Count; i++)
            {
                _modulePersistances[i].Save(entity, save);
            }
            return save;
        }
    }
}
