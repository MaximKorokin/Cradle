using System;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class SystemBase : ISystem, IDisposable
    {
        protected SystemBase()
        {
        }

        public virtual void Dispose()
        {
        }
    }

    public interface ISystem { }
}
