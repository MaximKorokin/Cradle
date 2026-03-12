using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(menuName = "Configs/DespawnConfig")]
    public sealed class DespawnConfig : ScriptableObject
    {
        [field: SerializeField]
        public float MinDespawnDelay { get; private set; }
        [field: SerializeField]
        public float MaxDespawnDelay { get; private set; }
    }
}
