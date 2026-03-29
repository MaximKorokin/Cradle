using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Interactions.Steps;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Calculators;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.Entities.Interactions
{
    [CreateAssetMenu(menuName = "Definitions/Interaction")]
    public class InteractionDefinition : ScriptableObject
    {
        [SerializeReference] public StepData Root;

        public InteractionInstance BuildRuntime(InteractionContext context, IObjectResolver resolver)
        {
            var root = Root?.Build(resolver) ?? new EmptyStep();
            var instance = new InteractionInstance(root, context);
            return instance;
        }

        [Serializable]
        public abstract class StepData
        {
            public abstract IInteractionStep Build(IObjectResolver resolver);
        }

        public abstract class StepsStepData : StepData
        {
            [SerializeReference] public List<StepData> Steps = new();
        }

        [Serializable]
        public sealed class SequenceData : StepsStepData
        {
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
        public sealed class ParallelData : StepsStepData
        {
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
            public override IInteractionStep Build(IObjectResolver resolver) => new DealDamageStep(Spec, resolver.Resolve<IDamageCalculator>(), resolver.Resolve<IGlobalEventBus>());
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

        [Serializable]
        public sealed class OverrideControlStepData : StepData
        {
            [SerializeReference]
            public ControlProviderData ControlProvider;

            public override IInteractionStep Build(IObjectResolver resolver)
                => new OverrideControlStep(ControlProvider.CreateInstance(resolver));
        }

        [Serializable]
        public sealed class SetAnimatorValueStepData : StepData
        {
            public ApplyToTarget ApplyTo;
            public EntityAnimatorParameterName Parameter;
            public int Value;

            public override IInteractionStep Build(IObjectResolver resolver)
                => new SetAnimatorValueStep(Parameter, Value, ApplyTo);

            public enum ApplyToTarget { Source, Target }
        }

        [Serializable]
        public sealed class LootItemPickupStepData : StepData
        {
            public override IInteractionStep Build(IObjectResolver resolver) => new LootItemPickupStep(resolver.Resolve<IGlobalEventBus>(), resolver.Resolve<EntityRepository>(), resolver.Resolve<ItemInstanceDataFactory>());
        }
    }
}
