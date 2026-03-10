using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Interactions.Ability
{
    [CreateAssetMenu(menuName = "Definitions/Ability")]
    public sealed class AbilityDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Description { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public InteractionDefinition Interaction { get; private set; }
        [field: SerializeField]
        public float ManaCost { get; private set; }
        [field: SerializeField]
        public float CastTime { get; private set; }
        [field: SerializeField]
        public float ChannelTime { get; private set; }
        [field: SerializeField]
        public float Cooldown { get; private set; }
        [field: SerializeField]
        public float CastRange { get; private set; }
    }
}
