using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Locations
{
    [CreateAssetMenu(fileName = "Location", menuName = "Game/LocationDefinition")]
    public sealed class LocationDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public string SceneName { get; private set; }

        [field: SerializeField]
        public LocationType Type { get; private set; }
    }

    public enum LocationType
    {
        None = 0,
        Town = 1,
        Wilderness = 2,
        Dungeon = 3,
    }
}
