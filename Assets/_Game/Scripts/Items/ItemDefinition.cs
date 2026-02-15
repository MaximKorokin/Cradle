using Assets._Game.Scripts.Infrastructure.Definitions;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemDefinition")]
    public class ItemDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public Sprite Sprite { get; private set; }
        [field: SerializeField]
        public int MaxAmount { get; private set; }
        [field: SerializeField]
        public int Weight { get; private set; }
        [field: SerializeReference]
        public ItemTraitBase[] Traits { get; private set; } = Array.Empty<ItemTraitBase>();
    }
}
