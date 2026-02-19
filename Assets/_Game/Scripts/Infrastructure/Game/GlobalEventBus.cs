using System;

public interface IGlobalEvent { }

public interface IGlobalEventBus
{
    void Publish<T>(in T evt) where T : struct, IGlobalEvent;
    IDisposable Subscribe<T>(Action<T> handler) where T : struct, IGlobalEvent;
    void Clear();
}

public sealed class GlobalEventBus : IGlobalEventBus
{
    private readonly EventBusCore _core = new();

    public void Publish<T>(in T evt) where T : struct, IGlobalEvent
        => _core.Publish(evt);

    public IDisposable Subscribe<T>(Action<T> handler) where T : struct, IGlobalEvent
        => _core.Subscribe(handler);

    public void Clear() => _core.Clear();
}
