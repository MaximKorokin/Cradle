using Assets._Game.Scripts.Infrastructure.Storage;
using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitVariantsCatalog : DefinitionCatalogBase<UnitVariants>
    {
        private Dictionary<EntityVisualModelUnitPath, UnitVariants> _variants;

        protected override void OnDefinitionsLoaded(IEnumerable<UnitVariants> definitions)
        {
            base.OnDefinitionsLoaded(definitions);
            _variants = definitions.ToDictionary(x => x.Path);
        }

        public UnitVariants GetByPath(EntityVisualModelUnitPath path)
        {
            if (_variants.TryGetValue(path, out var itemDefinition))
            {
                return itemDefinition;
            }

            SLog.Error($"{nameof(UnitVariants)} with {nameof(UnitVariants.Path)} '{path}' not found in catalog.");
            return null;
        }

        public EntityUnitVariant GetVariant(EntityVisualModelUnitPath path, string variantName)
        {
            var unit = GetByPath(path);
            if (unit == null) return null;

            var vartiant = unit.GetVariant(variantName);

            if (vartiant == null)
            {
                SLog.Error($"{nameof(EntityUnitVariant)} with {nameof(EntityUnitVariant.Name)} '{variantName}' not found in catalog.");
                return null;
            }
            return vartiant;
        }
    }
}
