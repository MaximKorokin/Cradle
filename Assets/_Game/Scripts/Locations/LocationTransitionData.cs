using Assets._Game.Scripts.Items;
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
        [field: SerializeField]
        public LocationTransitionRequirementsData RequirementsData { get; private set; }

        public LocationTransitionData(
            LocationDefinition locationDefinition,
            LocationEntranceDefinition entranceDefinition = null,
            LocationTransitionRequirementsData requirementsData = default)
        {
            LocationDefinition = locationDefinition;
            EntranceDefinition = entranceDefinition;
            RequirementsData = requirementsData;
        }
    }

    [Serializable]
    public struct LocationTransitionRequirementsData
    {
        public int Level;
        public ItemRequirementData[] Items;

        [Serializable]
        public struct ItemRequirementData
        {
            public ItemDefinition Definition;
            public int Amount;
        }
    }
}
