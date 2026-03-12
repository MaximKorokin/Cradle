using Assets._Game.Scripts.Entities.Control.Intents;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class IntentModule : EntityModuleBase
    {
        private MoveIntent _move;
        private AimIntent _aim;
        private UseAbilityIntent _useAbility;
        private PickupItemIntent _pickupItem;

        public void SetMove(MoveIntent moveIntent)
        {
            _move = moveIntent;
        }

        public void SetAim(AimIntent aimIntent)
        {
            _aim = aimIntent;
        }

        public void SetUseAbility(UseAbilityIntent interactIntent)
        {
            _useAbility = interactIntent;
        }

        public void SetPickupItem(PickupItemIntent pickupItem)
        {
            _pickupItem = pickupItem;
        }

        public bool TryConsumeMove(out MoveIntent intent)
        {
            return TryConsumeIntent(ref _move, out intent);
        }

        public bool TryConsumeAim(out AimIntent intent)
        {
            return TryConsumeIntent(ref _aim, out intent);
        }

        public bool TryConsumeUseAbility(out UseAbilityIntent intent)
        {
            return TryConsumeIntent(ref _useAbility, out intent);
        }

        public bool TryConsumePickupItem(out PickupItemIntent intent)
        {
            return TryConsumeIntent(ref _pickupItem, out intent);
        }

        private static bool TryConsumeIntent<T>(ref T intentProperty, out T intent) where T : IIntent
        {
            intent = intentProperty;
            intentProperty = (T)intentProperty.None;
            return !intent.Equals(intentProperty.None);
        }
    }
}
