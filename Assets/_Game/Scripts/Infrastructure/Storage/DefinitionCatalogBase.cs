using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Storage
{
    public abstract class DefinitionCatalogBase<T> : IEnumerable<T>
        where T : GuidScriptableObject
    {
        private protected readonly Dictionary<string, T> Definitions;

        public DefinitionCatalogBase()
        {
            var definitions = Resources.LoadAll<T>("").ToList();
            Definitions = definitions.ToDictionary(x => x.Id, x => x);
            OnDefinitionsLoaded(definitions);
        }

        protected virtual void OnDefinitionsLoaded(IEnumerable<T> definitions)
        {
        }

        public virtual T Get(string id)
        {
            if (Definitions.TryGetValue(id, out var entityDefinition))
            {
                return entityDefinition;
            }

            SLog.Error($"{typeof(T)} with id '{id}' not found in catalog.");
            return null;
        }

        public virtual bool TryGet(string id, out T definition)
        {
            return Definitions.TryGetValue(id, out definition);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Definitions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
