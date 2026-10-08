using System;
using Assets._Game.Scripts.Shared;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Storage
{
    public abstract class RepositoryBase<T> where T : IEntry
    {
        private static readonly EntryHandle UnboundHandle = new(default);

        readonly Dictionary<string, T> _byId = new();
        readonly Dictionary<string, EntryHandle> _handles = new();

        public IReadOnlyCollection<T> All => _byId.Values;

        public event Action<T> Added;
        public event Action<T> Removed;

        public void Add(T e)
        {
            if (string.IsNullOrWhiteSpace(e.Id))
                e.GenerateId();
            _byId.Add(e.Id, e);
            Added?.Invoke(e);
        }

        public bool Remove(string id)
        {
            if (_byId.TryGetValue(id, out var e))
            {
                _byId.Remove(id);
                if (_handles.TryGetValue(id, out var handle))
                {
                    _handles.Remove(id);
                    handle.Kill();
                }
                Removed?.Invoke(e);
                return true;
            }
            return false;
        }

        public int CopyAllTo(T[] buffer)
        {
            _byId.Values.CopyTo(buffer, 0);
            return _byId.Values.Count;
        }

        public bool Contains(string id) => _byId.ContainsKey(id);
        public T Get(string id) => _byId[id];
        public bool TryGet(string id, out T e) => _byId.TryGetValue(id, out e);

        /// <summary>Throws if the reference is unbound or the entry is removed.</summary>
        public T Get(EntryRef entryRef)
        {
            if (!entryRef.Exists)
                throw new KeyNotFoundException($"Entry '{entryRef.Id}' does not exist.");
            return _byId[entryRef.Id];
        }

        public bool TryGet(EntryRef entryRef, out T entry)
        {
            entry = default;
            return entryRef.Exists && _byId.TryGetValue(entryRef.Id, out entry);
        }

        public IReadOnlyObservableData<EntryRef> Observe(string id)
        {
            if (id == null)
                return UnboundHandle;

            if (!_byId.ContainsKey(id))
                return new EntryHandle(new EntryRef(id, false));

            if (!_handles.TryGetValue(id, out var handle))
            {
                handle = new EntryHandle(new EntryRef(id, true));
                _handles.Add(id, handle);
            }
            return handle;
        }

        private sealed class EntryHandle : IReadOnlyObservableData<EntryRef>
        {
            public EntryRef Value { get; private set; }

            public event Action<EntryRef> ValueChanged;

            public EntryHandle(EntryRef value)
            {
                Value = value;
            }

            public void Kill()
            {
                if (!Value.Exists) return;

                Value = new EntryRef(Value.Id, false);
                ValueChanged?.Invoke(Value);
            }
        }
    }
}
