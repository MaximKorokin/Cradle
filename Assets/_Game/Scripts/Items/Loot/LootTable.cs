using Assets._Game.Scripts.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Loot
{
    [CreateAssetMenu(menuName = "Game/LootTable")]
    public sealed class LootTable : GuidScriptableObject
    {
        [SerializeField]
        private LootEntry[] _lootEntries;
        public IReadOnlyList<LootEntry> LootEntries => _lootEntries;

        //protected override void OnValidate()
        //{
        //    base.OnValidate();

        //    if (_lootEntries == null) return;
        //    for (int i = 0; i < _lootEntries.Length; i++)
        //    {
        //        var lootEntry = _lootEntries[i];
        //        if (lootEntry.MaxAmount < lootEntry.MinAmount)
        //        {
        //            Debug.LogWarning($"{nameof(lootEntry.MinAmount)} is greater than {nameof(lootEntry.MaxAmount)} in {lootEntry.ItemDefinition} in {nameof(LootTable)} {Id}");
        //        }
        //    }
        //}
    }

    [Serializable]
    public struct LootEntry
    {
        public ItemDefinition ItemDefinition;
        public int MinAmount;
        public int MaxAmount;
        [Range(0f, 1f)]
        public float Chance;
    }
}
