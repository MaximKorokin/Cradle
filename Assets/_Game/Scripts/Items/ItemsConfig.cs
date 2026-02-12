using Assets.CoreScripts;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "ScriptableObjects/ItemsConfig")]
    public sealed class ItemsConfig : ScriptableObject
    {
        [SerializeField]
        private SpecialItemDefinition[] _specialItems;

        public ItemDefinition GetSpecialItemDefinition(SpecialItemId specialItemId)
        {
            var index = Array.IndexOf(_specialItems, specialItemId);
            if (index == -1)
            {
                SLog.Error($"Special item with id {specialItemId} is not registered.");
                return null;
            }
            return _specialItems[index].ItemDefinition;
        }
    }

    [Serializable]
    public struct SpecialItemDefinition
    {
        public SpecialItemId SpecialItemId;
        public ItemDefinition ItemDefinition;
    }

    public enum SpecialItemId
    {
        Gold = 10
    }
}
