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

        public override bool Equals(object obj)
        {
            return obj is ObservableData<T> observableData && Value.Equals(observableData.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(ObservableData<T> left, ObservableData<T> right) => left.Equals(right);
        public static bool operator !=(ObservableData<T> left, ObservableData<T> right) => !left.Equals(right);
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
