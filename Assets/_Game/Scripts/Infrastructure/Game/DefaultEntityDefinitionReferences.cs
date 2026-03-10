using Assets._Game.Scripts.Entities;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    [CreateAssetMenu(menuName = "Configs/DefaultEntityDefinitionReferences")]
    public sealed class DefaultEntityDefinitionReferences : ScriptableObject
    {
        [field: SerializeField]
        public EntityDefinition LootItem { get; private set; }
        [field: SerializeField]
        public EntityDefinition Projectile { get; private set; }
    }
}
