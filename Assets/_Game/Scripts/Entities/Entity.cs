using Assets._Game.Scripts.Entities.Modules;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities
{
    public class Entity
    {
        private readonly Dictionary<Type, IEntityModule> _modules = new();

        public Entity(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

        public void AddModule<T>(T part) where T : class, IEntityModule => _modules[typeof(T)] = part;
        public bool TryGetModule<T>(out T part) where T : class, IEntityModule
        {
            if (_modules.TryGetValue(typeof(T), out var p)) { part = (T)p; return true; }
            part = null!;
            return false;
        }
    }
}
