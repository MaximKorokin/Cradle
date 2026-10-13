using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public interface IEventBus
    {
        void Publish<T>(in T evt) where T : struct;
        IDisposable Subscribe<T>(Action<T> handler) where T : struct;
        void Clear();
    }

    public sealed class EventBusCore : IEventBus
    {
        private readonly Dictionary<Type, List<HandlerEntry>> _handlers = new();
        private long _nextSequence;

        public void Publish<T>(in T e) where T : struct
        {
            if (!_handlers.TryGetValue(typeof(T), out var list) || list.Count == 0)
                return;

            var snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
                ((Action<T>)snapshot[i].Handler).Invoke(e);
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
            => Subscribe(handler, 0);

        public IDisposable Subscribe<T>(Action<T> handler, int priority) where T : struct
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list))
                _handlers[type] = list = new List<HandlerEntry>(128);

            var entry = new HandlerEntry(handler, priority, _nextSequence++);
            list.Add(entry);
            list.Sort((a, b) =>
            {
                var priorityOrder = b.Priority.CompareTo(a.Priority);
                return priorityOrder != 0 ? priorityOrder : a.Sequence.CompareTo(b.Sequence);
            });

            return new Subscription(() =>
            {
                if (_handlers.TryGetValue(type, out var list))
                    list.Remove(entry);
            });
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                var index = list.FindIndex(entry => Equals(entry.Handler, handler));
                if (index >= 0)
                    list.RemoveAt(index);
            }
        }

        public IDisposable SubscribeOnce<T>(Action<T> handler) where T : struct
            => SubscribeOnce(handler, 0);

        public IDisposable SubscribeOnce<T>(Action<T> handler, int priority) where T : struct
        {
            IDisposable subscription = null;
            subscription = Subscribe<T>(e =>
            {
                try
                {
                    handler(e);
                }
                finally
                {
                    subscription.Dispose();
                }
            }, priority);
            return subscription;
        }

        public void Clear() => _handlers.Clear();

        private sealed class HandlerEntry
        {
            public Delegate Handler { get; }
            public int Priority { get; }
            public long Sequence { get; }

            public HandlerEntry(Delegate handler, int priority, long sequence)
            {
                Handler = handler;
                Priority = priority;
                Sequence = sequence;
            }
        }

        private sealed class Subscription : IDisposable
        {
            private Action _dispose;
            public Subscription(Action dispose) => _dispose = dispose;
            public void Dispose() { _dispose?.Invoke(); _dispose = null; }
        }
    }
}
