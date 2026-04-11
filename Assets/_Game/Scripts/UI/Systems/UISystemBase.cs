using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Systems
{
    public abstract class UISystemBase : MonoBehaviour, IDisposable
    {
        public virtual void Dispose()
        {

        }
    }
}
