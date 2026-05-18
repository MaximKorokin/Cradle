using Assets._Game.Scripts.Shared;
using UnityEngine;

namespace Assets._Game.Scripts.Locations
{
    [CreateAssetMenu(fileName = "LocationEntranceDefinition", menuName = "Game/LocationEntranceDefinition")]
    public sealed class LocationEntranceDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string DisplayName { get; private set; }
    }
}
