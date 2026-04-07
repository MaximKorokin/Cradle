using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;

namespace Assets._Game.Scripts.Infrastructure.Calculators
{
    public class DerivedStatsCalculator
    {
        private readonly StatId[] _primaryStats = new[] { StatId.SizeScale, StatId.Strength, StatId.Agility };

        public void RecalculateDerivedStats(Entity entity)
        {
            var stats = entity.GetModule<StatModule>();

            var source = StatModifierSource.Derived;

            stats.RemoveModifiers(source);

            var sizeScale = stats.Stats.Get(StatId.SizeScale);
            var strength = stats.Stats.Get(StatId.Strength);
            var agility = stats.Stats.Get(StatId.Agility);

            // todo: list formulas at least in an array
            stats.AddModifiers(source, new[]
            {
                new StatModifier(StatId.PhysicalRangeMultiplier, StatStage.Multiply, StatOperation.Multiply, sizeScale - 1),
                new StatModifier(StatId.PhysicalAttack, StatStage.PreAdd, StatOperation.Add, strength * 2f),
                new StatModifier(StatId.PhysicalAttackSpeed, StatStage.PreAdd, StatOperation.Add, agility * 0.01f),
            });
        }

        public void RecalculateDerivedStats(Entity entity, StatChangedEvent e)
        {
            for (int i = 0; i < _primaryStats.Length; i++)
            {
                if (e.StatId == _primaryStats[i])
                {
                    // todo: recalculate only changed stats
                    RecalculateDerivedStats(entity);
                    // return here to avoid unnecessary recalculations if multiple primary stats are changed at once
                    return;
                }
            }
        }
    }
}
