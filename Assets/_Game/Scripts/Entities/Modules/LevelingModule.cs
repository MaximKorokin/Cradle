using Assets._Game.Scripts.Infrastructure.Configs;
using System;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class LevelingModule : EntityModuleBase
    {
        private readonly ExperienceTable _experienceTable;

        public int Level { get; private set; }
        // Experience towards the next level. Resets to 0 on level up.
        public long Experience { get; private set; }

        public event Action<LevelChangedEvent> LevelChanged;
        public event Action<ExperienceChangedEvent> ExperienceChanged;
        public event Action Changed;

        public LevelingModule(ExperienceTable experienceTable)
        {
            _experienceTable = experienceTable;
        }

        // Returns a value between 0 and 1 representing the progress towards the next level.
        public float GetNormalizedExperience()
        {
            if (Level >= _experienceTable.MaxLevel)
                return 1f;

            var experienceForNextLevel = _experienceTable.GetExperienceForLevel(Level + 1);
            return (float)Experience / experienceForNextLevel;
        }

        public void AddExperience(long amount)
        {
            if (amount <= 0) return;

            Experience += amount;
            InvokeExperienceChanged(Experience - amount, Experience);
            Changed?.Invoke();

            while (Level < _experienceTable.MaxLevel && Experience >= _experienceTable.GetExperienceForLevel(Level + 1))
            {
                Experience -= _experienceTable.GetExperienceForLevel(Level + 1);
                Level++;

                InvokeChanged(Experience + amount, Experience, Level - 1, Level);
            }
        }

        public void SetLevel(int level)
        {
            if (level < 1 || level > _experienceTable.MaxLevel)
            {
                SLog.Error($"Level must be between {1} and {_experienceTable.MaxLevel}");
                return;
            }

            var oldLevel = Level;
            var oldExperience = Experience;

            Level = level;
            Experience = 0;

            InvokeChanged(oldExperience, Experience, oldLevel, Level);
        }

        private void InvokeExperienceChanged(long oldExperience, long newExperience)
        {
            ExperienceChanged?.Invoke(new ExperienceChangedEvent(Entity, oldExperience, newExperience));
            Entity.Publish(new ExperienceChangedEvent(Entity, oldExperience, newExperience));
        }

        private void InvokeLevelChanged(int oldLevel, int newLevel)
        {
            LevelChanged?.Invoke(new LevelChangedEvent(Entity, oldLevel, newLevel));
            Entity.Publish(new LevelChangedEvent(Entity, oldLevel, newLevel));
        }

        private void InvokeChanged(long oldExperience, long newExperience, int oldLevel, int newLevel)
        {
            InvokeExperienceChanged(oldExperience, newExperience);
            InvokeLevelChanged(oldLevel, newLevel);
            Changed?.Invoke();
        }
    }

    public readonly struct LevelChangedEvent : IEntityEvent
    {
        public readonly int OldLevel;
        public readonly int NewLevel;

        public Entity Entity { get; }

        public LevelChangedEvent(Entity entity, int oldLevel, int newLevel)
        {
            Entity = entity;
            OldLevel = oldLevel;
            NewLevel = newLevel;
        }
    }

    public readonly struct ExperienceChangedEvent : IEntityEvent
    {
        public readonly long OldAmount;
        public readonly long NewAmount;

        public Entity Entity { get; }

        public ExperienceChangedEvent(Entity entity, long oldAmount, long newAmount)
        {
            Entity = entity;
            OldAmount = oldAmount;
            NewAmount = newAmount;
        }
    }

    public sealed class LevelingModuleAssembler
    {
        private readonly ExperienceTable _experienceTable;

        public LevelingModuleAssembler(LevelingConfig levelingConfig)
        {
            _experienceTable = levelingConfig.ExperienceTable;
        }

        public LevelingModule Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<LevelingModuleDefinition>(out var _))
            {
                return new LevelingModule(_experienceTable);
            }
            return null;
        }
    }
}
