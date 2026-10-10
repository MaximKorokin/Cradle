using Assets._Game.Scripts.Quests.Objectives;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Quests
{
    public sealed class QuestState
    {
        public QuestDefinition Definition { get; }
        public ObjectiveProgress[] Objectives { get; set; }
        public bool AreObjectivesCompleted { get; private set; }
        public bool IsCompleted { get; private set; }

        public QuestStateSnapshot Snapshot => new(this);

        public event Action<QuestState> Updated;
        public event Action<QuestState> Completed;
        public event Action<QuestState> ObjectivesCompleted;

        public QuestState(QuestDefinition definition)
        {
            Definition = definition;

            Objectives = definition.Objectives.Select(o => o.CreateProgress()).ToArray();
            foreach (var objective in Objectives)
            {
                objective.Updated += OnObjectiveUpdated;
            }
        }

        public void SetCompleted(bool isCompleted, bool silent = false)
        {
            var previousState = IsCompleted;
            IsCompleted = isCompleted;
            if (isCompleted && !previousState && !silent)
            {
                Completed?.Invoke(this);
            }
        }

        private void OnObjectiveUpdated()
        {
            if (Objectives.All(o => o.IsCompleted))
            {
                AreObjectivesCompleted = true;
                ObjectivesCompleted?.Invoke(this);
            }
            else
            {
                AreObjectivesCompleted = false;
            }
            Updated?.Invoke(this);
        }
    }

    public struct QuestStateSnapshot
    {
        private readonly QuestState _questState;

        private ObjectiveProgressSnapshot[] _objectives;

        public readonly QuestDefinition Definition;
        public readonly bool IsCompleted;
        public readonly bool AreObjectivesCompleted;

        public QuestStateSnapshot(QuestState questState)
        {
            _questState = questState;
            _objectives = null;

            Definition = questState.Definition;
            IsCompleted = questState.IsCompleted;
            AreObjectivesCompleted = questState.AreObjectivesCompleted;
        }

        public QuestStateSnapshot(QuestDefinition definition)
        {
            _questState = null;
            _objectives = null;

            Definition = definition;
            IsCompleted = false;
            AreObjectivesCompleted = false;
        }

        public ObjectiveProgressSnapshot[] GetObjectives()
        {
            if (_objectives != null) 
            {
                return _objectives;
            }
            else if (_questState != null)
            {
                _objectives = _questState.Objectives.Select(o => new ObjectiveProgressSnapshot(o)).ToArray();
            }
            else
            {
                _objectives = Definition.Objectives.Select(o => new ObjectiveProgressSnapshot(o)).ToArray();
            }
            return _objectives;
        }
    }
}
