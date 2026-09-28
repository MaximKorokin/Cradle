using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public abstract class DataAggregatorBase : IDataAggregator
    {
        public event Action Changed;

        protected void NotifyChanged()
        {
            Changed?.Invoke();
        }

        public virtual void Dispose()
        {
        }
    }

    public interface IDataAggregator : IDisposable
    {
        event Action Changed;
    }
}
