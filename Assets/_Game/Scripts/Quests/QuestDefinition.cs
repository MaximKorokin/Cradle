using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Quests.Objectives;
using UnityEngine;

namespace Assets._Game.Scripts.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Definitions/QuestDefinition")]
    public sealed class QuestDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string Title { get; private set; }
        [field: SerializeField]
        public ObjectiveDefinition[] Objectives { get; private set; }
    }
}
