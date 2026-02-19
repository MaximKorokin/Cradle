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
    }

    public enum StatusEffectCategory
    {
        Buff,
        Debuff
    }
}
