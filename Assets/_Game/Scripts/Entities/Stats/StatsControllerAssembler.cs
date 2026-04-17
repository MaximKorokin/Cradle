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
    }
}
