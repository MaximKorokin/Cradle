using Assets._Game.Scripts.Infrastructure.Game;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(menuName = "Configs/ConfigReferences")]
    public sealed class ConfigReferences : ScriptableObject
    {
        [field: SerializeField]
        public NewGameDefinition NewGameDefinition { get; private set; }
        [field: SerializeField]
        public SaveConfig SaveConfig { get; private set; }
        [field: SerializeField]
        public ItemsConfig ItemsConfig { get; private set; }
        [field: SerializeField]
        public StatsConfig StatsConfig { get; private set; }
        [field: SerializeField]
        public StatusEffectsConfig StatusEffectsConfig { get; private set; }
        [field: SerializeField]
        public EntityUnitConfig EntityUnitConfig { get; private set; }
        [field: SerializeField]
        public DespawnConfig DespawnConfig { get; private set; }
        [field: SerializeField]
        public LevelingConfig LevelingConfig { get; private set; }
        [field: SerializeField]
        public LootConfig LootConfig { get; private set; }
    }
}
