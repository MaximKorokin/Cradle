using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.StatusEffects
{
    [CreateAssetMenu(fileName = "StatusEffect", menuName = "ScriptableObjects/StatusEffectDefinition")]
    public sealed class StatusEffectDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public StatusEffectCategory Category { get; private set; }
        [field: SerializeField]
        public float Duration { get; private set; }
        [field: SerializeField]
        public bool IsEndless { get; private set; }
        [field: SerializeField]
        public StatModifier[] StatModifiers { get; private set; }
        [field: SerializeReference]
        [field: Header("Control Provider")]
        public ControlProviderData ControlProvider { get; private set; }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (ControlProvider is TimedControlProviderData timed)
            {
                timed.Duration = Duration;
            }
        }
    }

    public enum StatusEffectCategory
    {
        Buff,
        Debuff
    }
}
