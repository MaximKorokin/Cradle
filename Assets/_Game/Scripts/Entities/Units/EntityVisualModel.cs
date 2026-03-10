using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Items.Equipment;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    [CreateAssetMenu(fileName = "VisualModel", menuName = "Visual/EntityVisualModel")]
    public class EntityVisualModel : GuidScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public EntityView BasePrefab { get; private set; }
        [field: SerializeField]
        public AnimatorOverrideController Animator { get; private set; }
        [field: SerializeField]
        public EntityUnitVisualModel[] Units { get; private set; }
    }

    [Serializable]
    public class EntityUnitVisualModel
    {
        [field: SerializeField]
        public EntityVisualModelUnitPath Path { get; private set; }
        [field: SerializeField]
        public EquipmentSlotType[] EquipmentSlots { get; private set; }
        [field: SerializeField]
        public int RelativeOrderInLayer { get; private set; }
    }

    public enum EntityVisualModelUnitPath
    {
        Body = 200,

        Head = 300,
        Hair = 310,

        HandLeft = 400,
        HandRight = 450,

        FootLeft = 500,
        FootRight = 550,

        FootFrontLeft = 1500,
        FootFrontRight = 1520,
        FootBackLeft = 1540,
        FootBackRight = 1560,
    }
}
