using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public abstract class DataAggregatorBase : IDataAggregator
    {
        public event Action Changed;
        public event Action Invalidated;

        protected void NotifyChanged()
        {
            Changed?.Invoke();
        }

        protected void NotifyInvalidated()
        {
            Invalidated?.Invoke();
        }

        public virtual void Dispose()
        {
        }
    }

    public interface IDataAggregator : IDisposable
    {
        event Action Changed;
        event Action Invalidated;
    }
}
