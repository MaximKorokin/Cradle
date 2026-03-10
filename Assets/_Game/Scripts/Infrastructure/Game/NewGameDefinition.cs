using Assets._Game.Scripts.Entities;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    [CreateAssetMenu(menuName = "Configs/NewGameDefinition")]
    public class NewGameDefinition : ScriptableObject
    {
        [field: SerializeField]
        public EntityDefinition PlayerEntityDefinition { get; private set; }
    }
}
