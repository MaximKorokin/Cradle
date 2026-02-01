using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityRepository
    {
        readonly Dictionary<string, Entity> _byId = new();

        public IEnumerable<Entity> All => _byId.Values;

        public void Add(Entity e) => _byId.Add(e.Id, e);
        public bool Remove(string id) => _byId.Remove(id);

        public Entity Get(string id) => _byId[id];
        public bool TryGet(string id, out Entity e) => _byId.TryGetValue(id, out e);
    }
}
