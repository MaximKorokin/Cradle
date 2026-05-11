using Assets._Game.Scripts.Quests.Objectives;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Quests
{
    public sealed class QuestState
    {
        public QuestDefinition Definition { get; }
        public string Title { get; }
        public ObjectiveProgress[] Objectives { get; set; }
        /// <summary> Gets a value indicating whether all objectives have been completed. </summary>
        public bool AreObjectivesCompleted { get; private set; }
        /// <summary> Indicates whether the quest has been completed. Value is controlled by the quest system and should not be set manually. </summary>
        public bool IsCompleted { get; private set; }

        public event Action<QuestState> Updated;
        public event Action<QuestState> Completed;
        public event Action<QuestState> ObjectivesCompleted;

        public QuestState(QuestDefinition definition)
        {
            Definition = definition;
            Title = definition.Title;

            Objectives = definition.Objectives.Select(o => o.CreateProgress()).ToArray();
            foreach (var objective in Objectives)
            {
                objective.Updated += OnObjectiveUpdated;
            }
        }

        public void SetCompleted(bool isCompleted, bool raiseEvents = true)
        {
            var previousState = IsCompleted;
            IsCompleted = isCompleted;
            if (isCompleted && !previousState && raiseEvents)
            {
                Completed?.Invoke(this);
            }
        }

        private void OnObjectiveUpdated()
        {
            Updated?.Invoke(this);
            if (Objectives.All(o => o.IsCompleted))
            {
                AreObjectivesCompleted = true;
                ObjectivesCompleted?.Invoke(this);
            }
        }
    }
}
