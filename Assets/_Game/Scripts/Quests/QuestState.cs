using Assets._Game.Scripts.Quests.Objectives;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Quests
{
    public sealed class QuestState
    {
        public string Title { get; }
        public ObjectiveProgress[] Objectives { get; set; }
        public bool IsCompleted { get; private set; }

        public event Action<QuestState> Updated;
        public event Action<QuestState> Completed;

        public QuestState(QuestDefinition definition)
        {
            Title = definition.Title;

            Objectives = definition.Objectives.Select(o => o.CreateProgress()).ToArray();
            foreach (var objective in Objectives)
            {
                objective.Updated += OnObjectiveUpdated;
            }
        }

        private void OnObjectiveUpdated()
        {
            Updated?.Invoke(this);
            if (Objectives.All(o => o.IsCompleted))
            {
                IsCompleted = true;
                Completed?.Invoke(this);
            }
        }
    }
}
