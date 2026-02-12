using Assets.CoreScripts;
using System;
using System.Linq;
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
            var itemDefinition = _specialItems.FirstOrDefault(s => s.SpecialItemId == specialItemId).ItemDefinition;
            if (itemDefinition == null)
            {
                SLog.Error($"Special item with id {specialItemId} is not registered.");
                return null;
            }
            return itemDefinition;
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
