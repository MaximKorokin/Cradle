using Assets._Game.Scripts.Entities;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    [CreateAssetMenu(fileName = "NewGameDefinition", menuName = "ScriptableObjects/NewGameDefinition")]
    public class NewGameDefinition : ScriptableObject
    {
        [field: SerializeField]
        public EntityDefinition PlayerEntityDefinition { get; set; }
    }
}
