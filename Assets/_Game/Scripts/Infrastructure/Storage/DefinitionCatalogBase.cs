using Assets.CoreScripts;
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

        public T Get(string id)
        {
            if (Definitions.TryGetValue(id, out var entityDefinition))
            {
                return entityDefinition;
            }

            SLog.Error($"{nameof(T)} with id '{id}' not found in catalog.");
            return null;
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
