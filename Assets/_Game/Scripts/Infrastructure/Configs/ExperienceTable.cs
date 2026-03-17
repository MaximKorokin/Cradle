using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "ExperienceTable", menuName = "Configs/ExperienceTable")]
    public sealed class ExperienceTable : ScriptableObject
    {
        [field: SerializeField]
        public long[] ExperienceToLevelUp { get; private set; }

        public int MaxLevel => ExperienceToLevelUp.Length;

        public long GetExperienceForLevel(int level)
        {
            if (level <= 0 || level > ExperienceToLevelUp.Length)
            {
                SLog.Error($"Invalid level: {level}. Valid levels are between 1 and {ExperienceToLevelUp.Length}.");
                return long.MaxValue; // Return a very high value to prevent leveling up
            }
            return ExperienceToLevelUp[level - 1];
        }
    }
}
