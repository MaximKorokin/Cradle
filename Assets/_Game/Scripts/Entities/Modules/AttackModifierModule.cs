namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AttackModifierModule : EntityModuleBase
    {

    }

    public enum AttackModifierType
    {
        None = 0,
        Vampiric = 1,
        Chilling = 2,
        Flaming = 3,
        Poisonous = 4,
    }

    public readonly struct AttackModifierEntry
    {
        public readonly AttackModifierType Type;
        public readonly float Value;
        public readonly float Chance;
    }
}
