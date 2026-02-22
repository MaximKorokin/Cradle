using Assets._Game.Scripts.Entities.Interaction.Calculators;
using Assets._Game.Scripts.Entities.Interaction.Steps;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Entities.Interaction
{
    [CreateAssetMenu(fileName = "Interaction", menuName = "ScriptableObjects/InteractionDefinition")]
    public class InteractionDefinition : ScriptableObject
    {
        [SerializeReference] public StepData Root;

        public IInteractionStep BuildRuntime(IObjectResolver resolver)
        {
            return Root?.Build(resolver) ?? new EmptyStep();
        }

        [Serializable]
        public abstract class StepData
        {
            public abstract IInteractionStep Build(IObjectResolver resolver);
        }

        [Serializable]
        public sealed class SequenceData : StepData
        {
            [SerializeReference] public List<StepData> Steps = new();
            public override IInteractionStep Build(IObjectResolver resolver)
            {
                var built = new IInteractionStep[Steps.Count];
                for (int i = 0; i < Steps.Count; i++)
                {
                    built[i] = Steps[i].Build(resolver);
                }
                return new SequenceStep(built);
            }
        }

        [Serializable]
        public sealed class ParallelData : StepData
        {
            [SerializeReference] public List<StepData> Steps = new();
            public override IInteractionStep Build(IObjectResolver resolver)
            {
                var built = new IInteractionStep[Steps.Count];
                for (int i = 0; i < Steps.Count; i++)
                {
                    built[i] = Steps[i].Build(resolver);
                }
                return new ParallelStep(built);
            }
        }

        [Serializable]
        public sealed class DamageData : StepData
        {
            public DamageSpec Spec;
            public override IInteractionStep Build(IObjectResolver resolver) => new DealDamageStep(Spec, resolver.Resolve<IDamageCalculator>());
        }

        [Serializable]
        public sealed class HealData : StepData
        {
            public int Amount = 10;
            public override IInteractionStep Build(IObjectResolver resolver) => new HealStep(Amount);
        }

        [Serializable]
        public sealed class WaitData : StepData
        {
            public float Seconds = 0.25f;
            public override IInteractionStep Build(IObjectResolver resolver) => new WaitStep(Seconds);
        }

        [Serializable]
        public sealed class VfxData : StepData
        {
            public string VfxId = "HitSpark";
            public override IInteractionStep Build(IObjectResolver resolver) => new SpawnVfxStep(VfxId);
        }
    }
}
