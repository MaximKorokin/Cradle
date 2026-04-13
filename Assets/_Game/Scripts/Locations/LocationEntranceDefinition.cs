using Assets._Game.Scripts.Infrastructure.Storage;
using UnityEngine;

namespace Assets._Game.Scripts.Locations
{
    public sealed class LocationEntranceDefinition : GuidScriptableObject
    { 
        [field: SerializeField]
        public string DisplayName { get; private set; }

    }
}
