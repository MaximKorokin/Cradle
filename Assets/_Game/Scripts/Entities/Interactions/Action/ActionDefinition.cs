using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Interactions.Action
{
    [CreateAssetMenu(menuName = "Definitions/Action")]
    public sealed class ActionDefinition : GuidScriptableObject
    {
        [field: Header("Visual")]
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Description { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: Space]
        [field: Header("Logic")]
        [field: SerializeField]
        public InteractionDefinition Interaction { get; private set; }
        [field: SerializeField]
        public float ManaCost { get; private set; }
        [field: SerializeField]
        public bool ConsumesCharges { get; private set; }
        [field: SerializeField]
        public float PreparationTime { get; private set; }
        [field: SerializeField]
        public float MaxChannelingTime { get; private set; }
        [field: SerializeField]
        public float Cooldown { get; private set; }
        [field: SerializeField]
        public float Range { get; private set; }
        [field: SerializeField]
        public ActionExecutionSpeedMultiplier SpeedMultiplier { get; private set; }
        [field: Space]
        [field: Header("Evaluation")]
        [field: SerializeField]
        public EntityQueryData EntityQueryData { get; private set; }
        [field: SerializeField]
        public FactionRelation FactionRelation { get; private set; }
        [field: SerializeField]
        public float BaseScore { get; private set; }
    }

    public enum ActionExecutionSpeedMultiplier
    {
        None = 0,
        PhysicalAttackSpeed = 1,
        MagicalAttackSpeed = 2,
    }

    public static class ActionExecutionSpeedMultiplierExtensions
    {
        public static float GetValue(this ActionExecutionSpeedMultiplier multiplier, StatModule stats)
        {
            return multiplier switch
            {
                ActionExecutionSpeedMultiplier.None => 1f,
                ActionExecutionSpeedMultiplier.PhysicalAttackSpeed => stats.Stats.Get(StatId.PhysicalAttackSpeed),
                ActionExecutionSpeedMultiplier.MagicalAttackSpeed => stats.Stats.Get(StatId.MagicalAttackSpeed),
                _ => 1f
            };
        }
    }
}
