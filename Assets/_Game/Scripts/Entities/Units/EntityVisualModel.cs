using Assets._Game.Scripts.Items.Equipment;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    [CreateAssetMenu(fileName = "VisualModel", menuName = "ScriptableObjects/EntityVisualModel")]
    public class EntityVisualModel : ScriptableObject
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
        Shirt = 210,
        Armor = 220,

        Head = 300,
        Hair = 310,
        Helmet = 320,

        HandLeft = 400,
        GloveLeft = 410,
        HandheldLeft = 420,
        HandRight = 450,
        GloveRight = 460,
        HandheldRight = 480,

        FootLeft = 500,
        BootLeft = 520,
        FootRight = 550,
        BootRight = 570,

        FootFrontLeft = 1500,
        FootFrontRight = 1520,
        FootBackLeft = 1540,
        FootBackRight = 1560,
    }
}
