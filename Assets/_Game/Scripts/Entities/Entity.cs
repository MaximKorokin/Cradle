using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Storage;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities
{
    public sealed class Entity : IEntry, IDisposable
    {
        string IEntry.Id { get; set; }

        public string Id => ((IEntry)this).Id;

        private readonly Dictionary<Type, IEntityModule> _modules = new();
        private readonly EventBusCore _bus = new();

        public EntityDefinition Definition { get; private set; }
        public IReadOnlyCollection<IEntityModule> Modules => _modules.Values;

        public Entity(EntityDefinition definition)
        {
            Definition = definition;
        }

        public void AddModule<T>(T module) where T : class, IEntityModule
        {
            if (module == null) return;

            _modules[module.GetType()] = module;
            module.Attach(this);
        }

        public bool TryGetModule<T>(out T module) where T : class, IEntityModule
        {
            if (_modules.TryGetValue(typeof(T), out var m))
            {
                module = (T)m;
                return true;
            }

            module = null!;
            return false;
        }

        public T GetModule<T>() where T : class, IEntityModule
        {
            if (TryGetModule(out T module))
            {
                return module;
            }
            throw new InvalidOperationException($"Entity does not have a module of type {typeof(T).Name}");
        }

        public bool HasModule<T>() where T : class, IEntityModule
            => _modules.ContainsKey(typeof(T));

        public bool HasModule(Type moduleType)
        {
            return _modules.ContainsKey(moduleType);
        }

        public void Publish<TEvent>(in TEvent evt) where TEvent : struct, IEntityEvent
            => _bus.Publish(evt);

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IEntityEvent
            => _bus.Subscribe(handler);

        public IDisposable SubscribeOnce<TEvent>(Action<TEvent> handler) where TEvent : struct, IEntityEvent
            => _bus.SubscribeOnce(handler);

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IEntityEvent
            => _bus.Unsubscribe(handler);

        public void Dispose()
        {
            _bus.Clear();
            foreach (var module in _modules.Values)
                module.Dispose();
            _modules.Clear();
        }
    }

    public interface IEntityEvent
    {
    }
}
