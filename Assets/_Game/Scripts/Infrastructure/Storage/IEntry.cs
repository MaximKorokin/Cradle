using System;

namespace Assets._Game.Scripts.Infrastructure.Storage
{
    public interface IEntry
    {
        public string Id { get; protected set; }

        void GenerateId()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }
}
