using Assets._Game.Scripts.Entities.Control.Intents;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class IntentModule : EntityModuleBase
    {
        public MoveIntent Move { get; private set; }
        public AimIntent Aim { get; private set; }
        public UseAbilityIntent UseAbility { get; private set; }

        public void SetMove(MoveIntent moveIntent)
        {
            Move = moveIntent;
        }

        public void SetAim(AimIntent aimIntent)
        {
            Aim = aimIntent;
        }

        public void SetUseAbility(UseAbilityIntent interactIntent)
        {
            UseAbility = interactIntent;
        }

        public bool TryConsumeMove(out MoveIntent intent)
        {
            intent = Move;
            Move = MoveIntent.None;
            return !intent.Equals(MoveIntent.None);
        }

        public bool TryConsumeUseAbility(out UseAbilityIntent intent)
        {
            intent = UseAbility;
            UseAbility = UseAbilityIntent.None;
            return !intent.Equals(UseAbilityIntent.None);
        }
    }
}
