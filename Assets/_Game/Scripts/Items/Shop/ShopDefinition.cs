using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Shop
{
    [CreateAssetMenu(menuName = "Definitions/ShopDefinition")]
    public sealed class ShopDefinition : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Display name of the shop")]
        public string ShopName { get; private set; } = "Shop";

        [field: SerializeField]
        [field: Range(0f, 5f)]
        [field: Tooltip("Multiplier applied to base price when buying from shop")]
        public float BuyCoefficient { get; private set; } = 1.0f;

        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("Multiplier applied to base price when selling to shop")]
        public float SellCoefficient { get; private set; } = 0.5f;

        [field: SerializeField]
        [field: Tooltip("Items initially stocked in the shop")]
        public ShopItemEntry[] InitialItems { get; private set; }
    }

    [Serializable]
    public sealed class ShopItemEntry
    {
        [field: SerializeField]
        public ItemDefinition ItemDefinition { get; private set; }

        [field: SerializeField]
        [field: Range(1, 999)]
        public int Amount { get; private set; } = 1;
    }
}
