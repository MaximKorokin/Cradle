using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Game;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Quests.Objectives
{
    [Serializable]
    public abstract class ObjectiveDefinition
    {
        [field: SerializeField]
        public int RequiredAmount { get; private set; }

        public abstract ObjectiveProgress CreateProgress();
    }

    public abstract class ObjectiveProgress
    {
        public ObjectiveDefinition ObjectiveDefinition { get; private set; }
        public virtual int CurrentAmount { get; private set; }
        public virtual int RequiredAmount { get; private set; }

        public bool IsCompleted => CurrentAmount >= RequiredAmount;

        public event Action Updated;

        public ObjectiveProgress(ObjectiveDefinition definition)
        {
            ObjectiveDefinition = definition;
            CurrentAmount = 0;
            RequiredAmount = definition.RequiredAmount;
        }

        public virtual void Initialize(Entity entity) { }

        protected void NotifyUpdated()
        {
            Updated?.Invoke();
        }

        protected void SetProgress(int amount)
        {
            CurrentAmount = amount;
            if (CurrentAmount > RequiredAmount)
            {
                CurrentAmount = RequiredAmount;
            }
            NotifyUpdated();
        }

        public abstract void HandleEvent(IEvent e);
    }

    public abstract class ObjectiveProgress<T> : ObjectiveProgress where T : ObjectiveDefinition
    {
        public T Definition { get; }

        protected ObjectiveProgress(T definition) : base(definition)
        {
            Definition = definition;
        }
    }

    public interface ILoadableObjectiveProgress
    {
        bool TryLoad(object state);
    }

    public interface ISaveableObjectiveProgress
    {
        object Save();
    }

    public readonly struct ObjectiveProgressSnapshot
    {
        public readonly ObjectiveDefinition ObjectiveDefinition;
        public readonly int CurrentAmount;
        public readonly int RequiredAmount;

        public ObjectiveProgressSnapshot(ObjectiveProgress progress)
        {
            ObjectiveDefinition = progress.ObjectiveDefinition;
            CurrentAmount = progress.CurrentAmount;
            RequiredAmount = progress.RequiredAmount;
        }

        public ObjectiveProgressSnapshot(ObjectiveDefinition definition)
        {
            ObjectiveDefinition = definition;
            CurrentAmount = 0;
            RequiredAmount = definition.RequiredAmount;
        }
    }
}
