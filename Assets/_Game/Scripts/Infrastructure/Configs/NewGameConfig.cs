using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Locations;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    [CreateAssetMenu(menuName = "Configs/NewGameConfig", fileName = "NewGameConfig")]
    public class NewGameConfig : ScriptableObject
    {
        [field: SerializeField]
        public EntityDefinition PlayerEntityDefinition { get; private set; }
        [field: SerializeField]
        public LocationTransitionData LocationTransitionData { get; private set; }
    }
}
