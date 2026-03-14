using Assets._Game.Scripts.Entities.Faction;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Interactions.Action
{
    [CreateAssetMenu(menuName = "Definitions/Action")]
    public sealed class ActionDefinition : GuidScriptableObject
    {
        [field: Header("Visual")]
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Description { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: Space]
        [field: Header("Logic")]
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
        [field: Space]
        [field: Header("Evaluation")]
        [field: SerializeField]
        public EntityQueryData EntityQueryData { get; private set; }
        [field: SerializeField]
        public FactionRelation FactionRelation { get; private set; }
        [field: SerializeField]
        public float BaseScore { get; private set; }
    }
}
