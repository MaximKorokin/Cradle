using Assets._Game.Scripts.Items.Equipment;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    [CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/EntityDefinition")]
    public class EntityDefinition : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public string Id { get; set; }

        [field: SerializeField]
        public string EntityId { get; set; }
        [field: SerializeField]
        public GameObject Prefab { get; set; }
        [field: SerializeField]
        public int InventorySlotsAmount { get; set; }
        [field: SerializeField]
        public EquipmentSlotType[] EquipmentSlots { get; set; }
        [field: SerializeField]
        public EntityAttributesModel Attributes { get; set; }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = Guid.NewGuid().ToString();
            }
        }
    }
}
