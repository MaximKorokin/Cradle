using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Storage
{
    public abstract class RepositoryBase<T> where T : IEntry
    {
        readonly Dictionary<string, T> _byId = new();

        public IEnumerable<T> All => _byId.Values;

        public void Add(T e)
        {
            if (string.IsNullOrWhiteSpace(e.Id))
                e.GenerateId();
            _byId.Add(e.Id, e);
        }
        public bool Remove(string id) => _byId.Remove(id);

        public bool Contains(string id) => _byId.ContainsKey(id);
        public T Get(string id) => _byId[id];
        public bool TryGet(string id, out T e) => _byId.TryGetValue(id, out e);
    }
}
