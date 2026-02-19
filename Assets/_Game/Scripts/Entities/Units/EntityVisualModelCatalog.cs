using Assets._Game.Scripts.Infrastructure.Storage;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Units
{
    public class EntityVisualModelCatalog : DefinitionCatalogBase<EntityVisualModel>
    {
        private Dictionary<string, EntityVisualModel> _visualModels;

        protected override void OnDefinitionsLoaded(IEnumerable<EntityVisualModel> definitions)
        {
            base.OnDefinitionsLoaded(definitions);
            _visualModels = definitions.ToDictionary(x => x.Name);
        }

        public EntityVisualModel GetByName(string name)
        {
            if (_visualModels.TryGetValue(name, out var itemDefinition))
            {
                return itemDefinition;
            }

            SLog.Error($"{nameof(EntityVisualModel)} with {nameof(EntityVisualModel.Name)} '{name}' not found in catalog.");
            return null;
        }
    }
}
