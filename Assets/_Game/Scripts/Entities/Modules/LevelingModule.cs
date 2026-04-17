using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Persistence;
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

        public LevelingModule(int level, ExperienceTable experienceTable)
        {
            Level = level;

            _experienceTable = experienceTable;
        }

        /// <summary>
        /// Returns a value between 0 and 1 representing the progress towards the next level.
        /// </summary>
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

        // todo: add silent flag
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
            var e = new ExperienceChangedEvent(oldExperience, newExperience);
            ExperienceChanged?.Invoke(e);
            Entity.Publish(e);
        }

        private void InvokeLevelChanged(int oldLevel, int newLevel)
        {
            var e = new LevelChangedEvent(oldLevel, newLevel);
            LevelChanged?.Invoke(e);
            Entity.Publish(e);
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

        public LevelChangedEvent(int oldLevel, int newLevel)
        {
            OldLevel = oldLevel;
            NewLevel = newLevel;
        }
    }

    public readonly struct ExperienceChangedEvent : IEntityEvent
    {
        public readonly long OldAmount;
        public readonly long NewAmount;

        public ExperienceChangedEvent(long oldAmount, long newAmount)
        {
            OldAmount = oldAmount;
            NewAmount = newAmount;
        }
    }

    public sealed class LevelingModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        private readonly ExperienceTable _experienceTable;

        public LevelingModuleFactory(LevelingConfig levelingConfig)
        {
            _experienceTable = levelingConfig.ExperienceTable;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<LevelingModuleDefinition>(out var levelingModuleDefinition))
            {
                return new LevelingModule(levelingModuleDefinition.Level, _experienceTable);
            }
            return null;
        }

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<LevelingModule>(out var levelingModule) || entitySave.LevelingSave == null) return;

            levelingModule.SetLevel(entitySave.LevelingSave.Level);
            levelingModule.AddExperience(entitySave.LevelingSave.Experience);
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<LevelingModule>(out var levelingModule)) return;

            entitySave.LevelingSave = new()
            {
                Level = levelingModule.Level,
                Experience = levelingModule.Experience,
            };
        }
    }
}
