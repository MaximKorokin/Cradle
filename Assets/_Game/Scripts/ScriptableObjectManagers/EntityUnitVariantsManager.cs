using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.ScriptableObjectManagers
{
    [CreateAssetMenu(fileName = "EntityUnitsManager", menuName = "ScriptableObjects/EntityUnitsManager")]
    public class EntityUnitVariantsManager : ScriptableObject
    {
        [field: SerializeField]
        public EntityUnitVariants[] Units { get; private set; }

        public EntityUnitVariants GetUnit(string name)
        {
            return Units.FirstOrDefault(v => v.Name == name);
        }
    }

    [Serializable]
    public class EntityUnitVariants
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public EntityUnitVariant[] Variants { get; private set; }

        public EntityUnitVariant GetVariant(string name)
        {
            return Variants.FirstOrDefault(v => v.Name == name);
        }
    }

    [Serializable]
    public class EntityUnitVariant
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Sprite Sprite { get; private set; }
    }
}
