using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Items.Equipment;
using System;
using System.Linq;
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
        public EntityVisualModel VisualModel { get; set; }
        [field: SerializeField]
        public string VariantName { get; set; }

        [SerializeReference]
        [SerializeField]
        private EntityModuleDefinition[] _modules = Array.Empty<EntityModuleDefinition>();

        public EntityModuleDefinition[] Modules => _modules;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = Guid.NewGuid().ToString();
            }
        }

        public bool TryGetModule<T>(out T module) where T : EntityModuleDefinition
        {
            module = _modules.FirstOrDefault(m => m is T) as T;
            return module != null;
        }
    }

    [Serializable]
    public abstract class EntityModuleDefinition
    {
        public bool Enabled = true;
    }

    public class InventoryDefinitionModule : EntityModuleDefinition
    {
        public int SlotsAmount;
    }

    public class EquipmentDefinitionModule : EntityModuleDefinition
    {
        public EquipmentSlotType[] EquipmentSlots;
    }

    public class AttributesDefinitionModule : EntityModuleDefinition
    {
        public EntityAttributesModule Attributes;
    }

    public class AiDefinitionModule : EntityModuleDefinition
    {
        public string BehaviorTreeId;
    }
}
