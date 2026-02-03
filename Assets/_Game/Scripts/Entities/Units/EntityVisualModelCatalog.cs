using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class EntityVisualModelCatalog
    {
        private readonly Dictionary<string, EntityVisualModel> _views;

        public EntityVisualModelCatalog()
        {
            _views = Resources.LoadAll<EntityVisualModel>("").ToDictionary(x => x.Name, x => x);
        }

        public EntityVisualModel GetEntityVisualModel(string name)
        {
            if (_views.TryGetValue(name, out var itemDefinition))
            {
                return itemDefinition;
            }

            SLog.Error($"{nameof(EntityVisualModel)} with {nameof(EntityVisualModel.Name)} '{name}' not found in catalog.");
            return null;
        }
    }
}
