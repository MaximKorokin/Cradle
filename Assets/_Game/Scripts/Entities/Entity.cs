using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Storage;
using System;
using System.Collections.Generic;

public interface IEntityEvent { }

public sealed class Entity : IEntry, IDisposable
{
    string IEntry.Id { get; set; }

    private readonly Dictionary<Type, IEntityModule> _modules = new();
    private readonly EventBusCore _bus = new();

    public EntityDefinition Definition { get; private set; }

    public Entity(EntityDefinition definition)
    {
        Definition = definition;
    }

    public void AddModule<T>(T module) where T : class, IEntityModule
    {
        _modules[typeof(T)] = module;
        module.Attach(this);
    }

    public bool TryGetModule<T>(out T module) where T : class, IEntityModule
    {
        if (_modules.TryGetValue(typeof(T), out var m)) { module = (T)m; return true; }
        module = null!;
        return false;
    }

    public void Publish<TEvent>(in TEvent evt) where TEvent : struct, IEntityEvent
        => _bus.Publish(evt);

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IEntityEvent
        => _bus.Subscribe(handler);

    public void Dispose()
    {
        _bus.Clear();
        foreach (var module in _modules.Values)
            module.Dispose();
        _modules.Clear();
    }
}
