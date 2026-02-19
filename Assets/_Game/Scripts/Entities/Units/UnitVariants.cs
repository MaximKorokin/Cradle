using Assets._Game.Scripts.Infrastructure.Storage;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    [CreateAssetMenu(fileName = "UnitVariants", menuName = "ScriptableObjects/EntityUnitVariants")]
    public class UnitVariants : GuidScriptableObject
    {
        [field: SerializeField]
        public EntityVisualModelUnitPath Path { get; private set; }
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
