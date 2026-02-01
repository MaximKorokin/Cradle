using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public class EntityDefinitionCatalog
    {
        private readonly Dictionary<string, EntityDefinition> _entityDefinitions;

        public EntityDefinitionCatalog()
        {
            SLog.Log("Loading Entities Catalog...");
            _entityDefinitions = Resources.LoadAll<EntityDefinition>("").ToDictionary(x => x.Id, x => x);
        }

        public EntityDefinition GetEntityDefinition(string id)
        {
            if (_entityDefinitions.TryGetValue(id, out var entityDefinition))
            {
                return entityDefinition;
            }

            SLog.Error($"{nameof(EntityDefinition)} with id '{id}' not found in catalog.");
            return null;
        }
    }
}
