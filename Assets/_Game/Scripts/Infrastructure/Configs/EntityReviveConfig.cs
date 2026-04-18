using Assets._Game.Scripts.Locations;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "EntityReviveConfig", menuName = "Configs/EntityReviveConfig")]
    public sealed class EntityReviveConfig : ScriptableObject
    {
        [field: SerializeField]
        [field: Range(0f, 1f)]
        public float HealthRestorePercentage { get; private set; }
        [field: SerializeField]
        public LocationTransitionData ReviveLocation { get; private set; }
    }
}
