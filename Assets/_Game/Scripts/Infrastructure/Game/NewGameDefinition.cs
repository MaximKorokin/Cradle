using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Locations;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    [CreateAssetMenu(menuName = "Configs/NewGameDefinition")]
    public class NewGameDefinition : ScriptableObject
    {
        [field: SerializeField]
        public EntityDefinition PlayerEntityDefinition { get; private set; }
        [field: SerializeField]
        public LocationDefinition Location { get; private set; }
        [field: SerializeField]
        public LocationEntranceDefinition LocationEntrance { get; private set; }
    }
}
