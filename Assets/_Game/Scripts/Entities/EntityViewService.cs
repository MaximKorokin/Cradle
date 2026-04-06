using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityViewService
    {
        private readonly Dictionary<Entity, EntityView> _entitiesMapping = new();

        private readonly EntityViewProvider _entityViewProvider;

        public EntityViewService(EntityViewProvider entityViewProvider)
        {
            _entityViewProvider = entityViewProvider;
        }

        public void SpawnEntityView(Entity entity, Vector2 position)
        {
            var view = _entityViewProvider.Create(entity.Definition);
            view.transform.position = position;

            _entitiesMapping[entity] = view;
            view.Bind(entity);
        }

        public void DespawnEntityView(Entity entity)
        {
            if (!_entitiesMapping.TryGetValue(entity, out var view))
            {
                SLog.Error($"No View found for Entity {entity.Definition}");
                return;
            }
            _entityViewProvider.Destroy(view);
            _entitiesMapping.Remove(entity);
        }

        public EntityView GetEntityView(Entity entity)
        {
            if (!_entitiesMapping.TryGetValue(entity, out var view))
            {
                SLog.Error($"No View found for Entity {entity.Definition}");
                return null;
            }
            return view;
        }
    }
}
