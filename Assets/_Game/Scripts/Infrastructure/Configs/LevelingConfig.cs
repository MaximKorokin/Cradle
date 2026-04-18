using Assets._Game.Scripts.Entities.Stats;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "LevelingConfig", menuName = "Configs/LevelingConfig")]
    public sealed class LevelingConfig : ScriptableObject
    {
        [field: SerializeField]
        public ExperienceTable ExperienceTable { get; private set; }
        [field: SerializeField]
        public float ExperienceMultiplier { get; private set; } = 1f;
        [field: Space]
        [field: Header("Changes on level up")]
        [field: SerializeField]
        public StatModifier[] StatModifiersOnLevelUp { get; private set; }
        [field: Space]
        [field: Header("Level difference Penalties (per level)")]
        [field: SerializeField]
        public float ExperiencePenaltyForOverLeveling { get; private set; }
        [field: SerializeField]
        public float AttackPenaltyForUnderLeveling { get; private set; }
        [field: SerializeField]
        public float DefencePenaltyForUnderLeveling { get; private set; }
        [field: Space]
        [field: Header("Level difference Bonuses (per level)")]
        [field: SerializeField]
        public float ExperienceBonusForUnderLeveling { get; private set; }
    }
}
