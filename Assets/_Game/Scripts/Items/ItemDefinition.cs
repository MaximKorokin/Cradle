using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemDefinition")]
    public class ItemDefinition : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public string Id { get; set; }
        [field: SerializeField]
        public string Name { get; set; }
        [field: SerializeField]
        public Sprite Icon { get; set; }
        [field: SerializeField]
        public Sprite Sprite { get; set; }
        [field: SerializeField]
        public int MaxAmount { get; set; }
        [field: SerializeField]
        public int Weight { get; set; }
        [field: SerializeReference]
        public ItemTraitBase[] Traits { get; set; }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = Guid.NewGuid().ToString();
            }
        }
    }
}
