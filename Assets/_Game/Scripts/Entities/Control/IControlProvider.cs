using System;
using VContainer;

namespace Assets._Game.Scripts.Entities.Control
{
    public interface IControlProvider
    {
        ControlPriority Priority { get; }
        ControlMask Mask { get; }
        bool IsActive { get; }
        void Initialize(Entity entity);
        void Tick(float delta);
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
