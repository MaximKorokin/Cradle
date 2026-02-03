using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class EntityUnitVariantsCatalog
    {
        private readonly Dictionary<EntityVisualModelUnitPath, EntityUnitVariants> _variants;

        public EntityUnitVariantsCatalog()
        {
            _variants = Resources.LoadAll<EntityUnitVariants>("").ToDictionary(x => x.Path, x => x);
        }

        public EntityUnitVariants GetUnit(EntityVisualModelUnitPath path)
        {
            if (_variants.TryGetValue(path, out var itemDefinition))
            {
                return itemDefinition;
            }

            SLog.Error($"{nameof(EntityUnitVariants)} with {nameof(EntityUnitVariants.Path)} '{path}' not found in catalog.");
            return null;
        }

        public EntityUnitVariant GetUnitVariant(EntityVisualModelUnitPath path, string variantName)
        {
            var unit = GetUnit(path);
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
