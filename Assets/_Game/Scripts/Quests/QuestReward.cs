using Assets._Game.Scripts.Items;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Quests
{
    [Serializable]
    public sealed class QuestReward
    {
        [SerializeField]
        private long _experience;
        [SerializeField]
        private ItemDefinition[] _itemRewards;

        public long Experience => _experience;
        public ItemDefinition[] ItemRewards => _itemRewards;

        public QuestReward(long experience, ItemDefinition[] itemRewards)
        {
            _experience = experience;
            _itemRewards = itemRewards;
        }
    }
}
