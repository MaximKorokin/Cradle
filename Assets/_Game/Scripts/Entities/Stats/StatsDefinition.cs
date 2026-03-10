using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Stats
{
    [CreateAssetMenu(menuName = "Definitions/Stats")]
    public sealed class StatsDefinition : ScriptableObject
    {
        [field: SerializeField]
        public StatDefinition[] Stats { get; private set; }
    }

    [Serializable]
    public sealed class StatDefinition
    {
        [field: SerializeField]
        public StatId Id { get; private set; }

        [field: SerializeField]
        public float DefaultBase { get; private set; }
    }
}
