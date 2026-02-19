using System;
using System.Collections.Generic;

public interface IEventBus
{
    void Publish<T>(in T evt) where T : struct;
    IDisposable Subscribe<T>(Action<T> handler) where T : struct;
    void Clear();
}

public sealed class EventBusCore : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Publish<T>(in T evt) where T : struct
    {
        if (!_handlers.TryGetValue(typeof(T), out var list) || list.Count == 0)
            return;

        var snapshot = list.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
            ((Action<T>)snapshot[i]).Invoke(evt);
    }

    public IDisposable Subscribe<T>(Action<T> handler) where T : struct
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var list))
            _handlers[type] = list = new List<Delegate>(8);

        list.Add(handler);
        return new Subscription(() =>
        {
            if (_handlers.TryGetValue(type, out var l))
                l.Remove(handler);
        });
    }

    public void Clear() => _handlers.Clear();

    private sealed class Subscription : IDisposable
    {
        private Action _dispose;
        public Subscription(Action dispose) => _dispose = dispose;
        public void Dispose() { _dispose?.Invoke(); _dispose = null; }
    }
}
