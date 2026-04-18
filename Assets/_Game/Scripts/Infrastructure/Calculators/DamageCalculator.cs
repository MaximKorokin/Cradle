using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Calculators
{
    public enum DamageType
    {
        None = 0,
        Physical = 10,
        Magical = 20,
        True = 30
    }

    [Serializable]
    public struct DamageSpec
    {
        public DamageSourceType Source;
        public DamageType Type;
        public float Flat;
        public float AttackScale;
        public bool CanCrit;

        DamageSpec(DamageSourceType source, DamageType type, float flat, float attackScale, bool canCrit)
        {
            Source = source;
            Type = type;
            Flat = flat;
            AttackScale = attackScale;
            CanCrit = canCrit;
        }
    }

    public interface IDamageCalculator
    {
        int Calculate(in DamageSpec spec, Entity source, Entity target, out bool isCritical);
    }

    public sealed class DamageCalculator : IDamageCalculator
    {
        public int Calculate(in DamageSpec spec, Entity source, Entity target, out bool isCritical)
        {
            isCritical = false;
            if (!source.TryGetModule(out StatModule sourceStats)) return 0;
            if (!target.TryGetModule(out StatModule targetStats)) return 0;

            var attack = sourceStats.Stats.Get(StatId.PhysicalAttack);
            var armor = targetStats.Stats.Get(StatId.PhysicalArmor);

            var rawDamage = spec.Flat + attack * spec.AttackScale;

            // Hyperbolic Reduction
            // 100 - mid-game armor (50% reduction)
            // example of physical reduction: raw * 100/(100+armor)
            var reduced = spec.Type == DamageType.Physical
                ? rawDamage * (100f / (100f + armor))
                : rawDamage;

            if (spec.CanCrit)
            {
                var critChance = sourceStats.Stats.Get(StatId.PhysicalCritChance);
                var critMultiplier = sourceStats.Stats.Get(StatId.PhysicalCritMultiplier);
                if (Roll(critChance))
                {
                    reduced *= critMultiplier;
                    isCritical = true;
                }
            }

            var damage = Mathf.FloorToInt(reduced);
            return rawDamage > 0f ? Mathf.Max(1, damage) : 0;
        }

        private static bool Roll(float chance01) => UnityEngine.Random.value < Mathf.Clamp01(chance01);
    }
}
