using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Stats
{
    public sealed class Stat
    {
        private const float MinMultiplier = 0f;
        private const float MaxMultiplier = 10f;

        private readonly List<(object Source, StatModifier Modifier)> _modifiers = new();

        public Stat(float baseValue)
        {
            BaseValue = baseValue;
        }

        public float BaseValue { get; private set; }

        public void SetBase(float value) => BaseValue = value;

        public void AddModifier(object source, StatModifier modifier)
        {
            _modifiers.Add((source, modifier));
        }

        public int RemoveBySource(object source)
        {
            return _modifiers.RemoveAll(e => Equals(e.Source, source));
        }

        public float Calculate()
        {
            float value = BaseValue;

            // Sort by Stage then Priority (Priority used within same stage)
            var ordered = _modifiers
                .OrderBy(m => m.Modifier.Stage)
                .ThenBy(m => m.Modifier.Priority);

            float addAccumulated = 0f;
            float multiplyAccumulated = 0f;
            float postMultiplyAccumulated = 0f;

            float? overrideValue = null;
            float? clampMin = null;
            float? clampMax = null;

            foreach (var (_, modifier) in ordered)
            {
                // You can switch on stage explicitly; easiest is switch by Op + Stage.
                switch (modifier.Stage)
                {
                    case StatStage.PreAdd:
                    case StatStage.Add:
                        if (modifier.Operation != StatOperation.Add) throw new InvalidOperationException($"Stage {StatStage.Add} expects operation {StatOperation.Add}");
                        addAccumulated += modifier.Value;
                        break;

                    case StatStage.Multiply:
                        if (modifier.Operation != StatOperation.Multiply) throw new InvalidOperationException($"Stage {StatStage.Multiply} expects operation {StatOperation.Multiply}");
                        multiplyAccumulated += modifier.Value;
                        break;

                    case StatStage.PostMultiply:
                        if (modifier.Operation != StatOperation.Multiply) throw new InvalidOperationException($"Stage {StatStage.PostMultiply} expects operation {StatOperation.Multiply}");
                        postMultiplyAccumulated += modifier.Value;
                        break;

                    case StatStage.Override:
                        if (modifier.Operation != StatOperation.Override) throw new InvalidOperationException($"Stage {StatStage.Override} expects operation {StatOperation.Override}");
                        overrideValue = modifier.Value; // last wins by priority
                        break;

                    case StatStage.Clamp:
                        if (modifier.Operation == StatOperation.ClampMin)
                            clampMin = clampMin.HasValue ? Math.Max(clampMin.Value, modifier.Value) : modifier.Value;
                        else if (modifier.Operation == StatOperation.ClampMax)
                            clampMax = clampMax.HasValue ? Math.Min(clampMax.Value, modifier.Value) : modifier.Value;
                        else
                            throw new InvalidOperationException($"Stage {StatStage.Clamp} expects operation {StatOperation.ClampMin} or {StatOperation.ClampMax}");
                        break;
                }
            }

            // Apply pipeline
            value += addAccumulated;
            var multiplier = 1f + multiplyAccumulated;
            value *= Mathf.Clamp(multiplier, MinMultiplier, MaxMultiplier);
            value *= (1f + postMultiplyAccumulated);

            if (overrideValue.HasValue)
                value = overrideValue.Value;

            if (clampMin.HasValue) value = Math.Max(value, clampMin.Value);
            if (clampMax.HasValue) value = Math.Min(value, clampMax.Value);

            return value;
        }
    }

    public enum StatId
    {
        Level = 100,
        Exp = 110,

        HpMax = 200,
        HpCurrent = 210,

        Damage = 300,
        MoveSpeed = 310,
        AttackSpeed = 320,

        Strength = 400,
        Agility = 410,

        CarryWeight = 500,
        CarryWeightMax = 510,
    }

    public enum StatStage
    {
        // base -> (preAdd) -> (add) -> (mul) -> (postMul) -> (override) -> clamp
        PreAdd = 10,        // +X to Base
        Add = 20,           // +X
        Multiply = 30,      // +(X) to multiplier, e.g. 0.1 = +10%
        PostMultiply = 40,  // global multiplier
        Override = 50,      // set to X
        Clamp = 60          // restriction: min/max/softcap
    }

    public enum StatOperation
    {
        Add = 10,        // +X
        Multiply = 20,   // +(X) to multiplier, e.g. 0.1 = +10%
        Override = 30,   // set to X (last wins by priority)
        ClampMin = 40,
        ClampMax = 50
    }

    [Serializable]
    public struct StatModifier
    {
        [field: SerializeField]
        public StatId Stat { get; private set; }
        [field: SerializeField]
        public StatStage Stage { get; private set; }
        [field: SerializeField]
        public StatOperation Operation { get; private set; }
        [field: SerializeField]
        public float Value { get; private set; }
        [field: SerializeField]
        public int Priority { get; private set; }

        public StatModifier(StatId stat, StatStage stage, StatOperation op, float value, int priority = 0)
        {
            Stat = stat;
            Stage = stage;
            Operation = op;
            Value = value;
            Priority = priority;
        }
    }
}
