using System;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public interface IControlProvider
    {
        ControlPriority Priority { get; }
        ControlMask Mask { get; }
        /// <summary>
        /// When false, the provider is removed from the <see cref="ControlModule"/> on the next tick.
        /// Use this for providers that expire naturally (e.g. timed stun/fear).
        /// Providers that should remain registered but temporarily yield control should keep this true and return <see cref="ControlMask.None"/> instead.
        /// </summary>
        bool IsPersisted { get; }
        void Initialize(Entity entity);
        void Tick(float delta);
        void Reset();
    }

    public enum ControlPriority
    {
        BaseAI = 0,
        OverrideAI = 10,  // loot pick up
        BasePlayer = 20,

        OverrideLow = 100,  // fear
        OverrideHigh = 200, // stun

        System = 1000,      // used for things like cutscenes, where we want to be sure that no other control provider can override it
    }

    [Flags]
    public enum ControlMask
    {
        None = 0,
        Move = 1 << 0,
        Aim = 1 << 1,
        Interact = 1 << 2,
        All = Move | Aim | Interact
    }

    [Serializable]
    public abstract class ControlProviderData
    {
        public abstract IControlProvider CreateInstance(IObjectResolver resolver);
    }

    [Serializable]
    public abstract class TimedControlProviderData : ControlProviderData
    {
        public float Duration;
    }
}
