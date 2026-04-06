using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(menuName = "Configs/LootConfig")]
    public class LootConfig : ScriptableObject
    {
        [field: SerializeField]
        public float SpawnRadius { get; private set; }
    }
}
