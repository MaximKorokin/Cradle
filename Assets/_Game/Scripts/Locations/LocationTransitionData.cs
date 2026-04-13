using System;
using UnityEngine;

namespace Assets._Game.Scripts.Locations
{
    [Serializable]
    public sealed class LocationTransitionData
    {
        [field: SerializeField]
        public LocationDefinition LocationDefinition { get; private set; }
        [field: SerializeField]
        public LocationEntranceDefinition EntranceDefinition { get; private set; }

        public LocationTransitionData(LocationDefinition locationDefinition, LocationEntranceDefinition entranceDefinition)
        {
            LocationDefinition = locationDefinition;
            EntranceDefinition = entranceDefinition;
        }
    }
}
