using UnityEngine;

namespace Assets._Game.Scripts.Entities.Stats
{
    [CreateAssetMenu(menuName = "Configs/StatsConfig")]
    public sealed class StatsConfig : ScriptableObject
    {
        /// <summary>
        /// Number of update ticks per second.
        /// </summary>
        [field: SerializeField]
        public float TickRate { get; private set; }
    }
}
