using System;
using VContainer.Unity;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public abstract class SystemBase : IDisposable, IStartable
    {
        public virtual void Start()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}
