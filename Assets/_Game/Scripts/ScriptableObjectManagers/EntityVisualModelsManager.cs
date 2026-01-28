using System;
using UnityEngine;

namespace Assets._Game.Scripts.ScriptableObjectManagers
{
    [CreateAssetMenu(fileName = "Entities", menuName = "ScriptableObjects/Entities")]
    public class EntityVisualModelsManager : ScriptableObject
    {
        [field: SerializeField]
        public EntityVisualModel[] Views { get; private set; }
    }

    [Serializable]
    public class EntityVisualModel
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Variant { get; private set; }
        [field: SerializeField]
        public AnimatorOverrideController Animator { get; private set; }
        [field: SerializeField]
        public EntityUnitVisualModel[] Units { get; private set; }
    }

    [Serializable]
    public class EntityUnitVisualModel
    {
        [field: SerializeField]
        public string Path { get; private set; }
        [field: SerializeField]
        public int RelativeOrderInLayer { get; private set; }
    }
}
