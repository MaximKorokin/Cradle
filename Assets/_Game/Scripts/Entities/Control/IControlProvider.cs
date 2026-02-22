namespace Assets._Game.Scripts.Entities.Control
{
    public enum ControlPriority
    {
        BaseAI = 0,
        BasePlayer = 10,

        OverrideLow = 100,  // fear
        OverrideHigh = 200, // stun

        System = 1000,      // used for things like cutscenes, where we want to be sure that no other control provider can override it
    }

    public interface IControlProvider
    {
        ControlPriority Priority { get; }
        bool IsActive { get; }
        void Tick(Entity entity, float delta);
    }
}
