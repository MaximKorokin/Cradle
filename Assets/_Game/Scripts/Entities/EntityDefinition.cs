using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Items.Equipment;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    [CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/EntityDefinition")]
    public class EntityDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public EntityVisualModel VisualModel { get; private set; }
        [field: SerializeField]
        public string VariantName { get; private set; }

        [SerializeReference]
        [SerializeField]
        private EntityModuleDefinition[] _modules = Array.Empty<EntityModuleDefinition>();

        public EntityModuleDefinition[] Modules => _modules;

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

    public class InventoryModuleDefinition : EntityModuleDefinition
    {
        public int SlotsAmount;
    }

    public class EquipmentModuleDefinition : EntityModuleDefinition
    {
        public EquipmentSlotType[] EquipmentSlots;
    }

    public class StatsModuleDefinition : EntityModuleDefinition
    {
        public StatsDefinition Stats;
    }

    public class AiModuleDefinition : EntityModuleDefinition
    {
        public string BehaviorTreeId;
    }

    public class StatusEffectModuleDefinition : EntityModuleDefinition
    {

    }
}
