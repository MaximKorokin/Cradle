using Assets._Game.Scripts.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Stats
{
    public sealed class StatsControllerAssembler
    {
        public StatsController Create(StatsDefinition statsDefinition)
        {
            return new(statsDefinition.Stats.Select(s => (s.Id, s.DefaultBase)), new());
        }

        public void Apply(StatsController statsController, StatsSave save)
        {
            foreach (var statSave in save.BaseValues)
            {
                statsController.SetBase(statSave.Id, statSave.Value);
            }
        }

        public StatsSave Save(StatsController statsController)
        {
            var baseStats = new List<StatBaseSave>();
            foreach (var statId in Enum.GetValues(typeof(StatId)).Cast<StatId>())
            {
                if (statsController.Has(statId))
                    baseStats.Add(new StatBaseSave() { Id = statId, Value = statsController.GetBase(statId) });
            }
            return new() { BaseValues = baseStats };
        }
    }
}
