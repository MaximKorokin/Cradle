using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Shared
{
    public sealed class ObservableData<T> : IObservableData<T>
    {
        public T Value { get; private set; }

        public event Action<T> ValueChanged;

        public ObservableData(T initialValue)
        {
            Value = initialValue;
        }

        public void SetData(T value)
        {
            if (EqualityComparer<T>.Default.Equals(Value, value))
                return;

            Value = value;
            ValueChanged?.Invoke(value);
        }
    }

    public interface IObservableData<T> : IReadOnlyObservableData<T>
    {
        void SetData(T value);
    }

    public interface IReadOnlyObservableData<T>
    {
        T Value { get; }
        event Action<T> ValueChanged;
    }
}
