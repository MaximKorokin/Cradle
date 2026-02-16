using Assets._Game.Scripts.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Stats
{
    public sealed class StatRegulator
    {
        private readonly HashSet<StatId> _statsWithForbiddenModifiers = new();
        private readonly HashSet<StatId> _statsWithNonNegativeRestriction = new();

        private readonly Dictionary<StatId, StatId> _statsRestrictedToBeLessThan = new();
        private readonly Dictionary<StatId, StatId> _statsThatRestrictMaxValue = new();

        public StatRegulator()
        {
            // Restrictions precomputation
            foreach (var statId in Enum.GetValues(typeof(StatId)).Cast<StatId>())
            {
                foreach (var restriction in statId.GetAttributes<StatRestrictionAttribute>())
                {
                    if (restriction.Restriction == StatRestrictionType.ForbidModifiers)
                    {
                        _statsWithForbiddenModifiers.Add(statId);
                    }
                    else if (restriction.Restriction == StatRestrictionType.NotBiggerThan)
                    {
                        if (restriction.Parameter is StatId statIdParameter)
                        {
                            _statsRestrictedToBeLessThan.Add(statId, statIdParameter);
                            _statsThatRestrictMaxValue.Add(statIdParameter, statId);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Invalid parameter for {nameof(StatRestrictionType.NotBiggerThan)} restriction on stat {statId}");
                        }
                    }
                    else if (restriction.Restriction == StatRestrictionType.NonNegative)
                    {
                        _statsWithNonNegativeRestriction.Add(statId);
                    }
                }
            }
        }

        public void RegulateBeforeChange(StatsController statsController, StatId statId, ref float value)
        {
            if (_statsWithNonNegativeRestriction.Contains(statId))
            {
                value = Math.Max(0, value);
            }

            if (_statsRestrictedToBeLessThan.TryGetValue(statId, out var maxValueStatId))
            {
                var maxValue = statsController.Get(maxValueStatId);
                value = Math.Min(value, maxValue);
            }
        }

        public void RegulateAfterChange(StatsController statsController, StatId statId)
        {
            if (_statsRestrictedToBeLessThan.TryGetValue(statId, out var maxValueStatId))
            {
                var value = statsController.GetBase(statId);
                var maxValue = statsController.Get(maxValueStatId);
                if (value > maxValue)
                {
                    statsController.SetBase(statId, maxValue);
                }
            }

            if (_statsThatRestrictMaxValue.TryGetValue(statId, out var restrictedStatId))
            {
                var maxValue = statsController.Get(statId);
                var value = statsController.GetBase(restrictedStatId);
                if (value > maxValue)
                {
                    statsController.SetBase(restrictedStatId, maxValue);
                }
            }
        }

        public void RegulateModifier(StatsController statsController, StatId statId, StatModifier modifier)
        {
            if (_statsWithForbiddenModifiers.Contains(statId))
            {
                throw new InvalidOperationException($"Stat {statId} cannot have modifiers");
            }
        }
    }
}
