using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;

namespace Assets._Game.Scripts.Infrastructure.Calculators
{
    public class DerivedStatsCalculator
    {
        public void RecalculateDerivedStats(Entity entity)
        {
            var stats = entity.GetModule<StatModule>();

            var source = StatModifierSource.Derived;

            stats.RemoveModifiers(source);

            var strength = stats.Stats.GetBase(StatId.Strength);
            var agility = stats.Stats.GetBase(StatId.Agility);

            // todo: list formulas at least in an array
            stats.AddModifiers(source, new[]
            {
                new StatModifier(StatId.PhysicalAttack, StatStage.PreAdd, StatOperation.Add, strength * 2f),
                new StatModifier(StatId.PhysicalAttackSpeed, StatStage.PreAdd, StatOperation.Add, agility * 0.01f),
            });
        }

        public void RecalculateDerivedStats(StatChangedEvent e)
        {
            // todo: recalculate only changed stats
            // todo: list primary stats at least in an array
            if (e.StatId != StatId.Strength && e.StatId != StatId.Agility) return;

            RecalculateDerivedStats(e.Entity);
        }
    }
}
