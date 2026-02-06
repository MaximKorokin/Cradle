using Assets._Game.Scripts.Entities.Modules;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities
{
    public class Entity : IDisposable
    {
        private readonly Dictionary<Type, IEntityModule> _modules = new();
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public Entity(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

        public void AddModule<T>(T module) where T : class, IEntityModule
        {
            _modules[typeof(T)] = module;
            module.Attach(this);
        }

        public bool TryGetModule<T>(out T module) where T : class, IEntityModule
        {
            if (_modules.TryGetValue(typeof(T), out var p)) { module = (T)p; return true; }
            module = null!;
            return false;
        }

        public void Publish<TEvent>(TEvent evt) where TEvent : struct, IEntityEvent
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var list))
                for (int i = 0; i < list.Count; i++)
                    ((Action<TEvent>)list[i]).Invoke(evt);
        }

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IEntityEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var list))
                _handlers[typeof(TEvent)] = list = new List<Delegate>();

            list.Add(handler);
            return new Subscription(() => list.Remove(handler));
        }

        public void Dispose()
        {
            foreach (var module in _modules.Values)
            {
                module.Dispose();
            }
        }

        private sealed class Subscription : IDisposable
        {
            private Action _dispose;
            public Subscription(Action dispose) => _dispose = dispose;
            public void Dispose() { _dispose?.Invoke(); _dispose = null; }
        }
    }

    public interface IEntityEvent { }
}
