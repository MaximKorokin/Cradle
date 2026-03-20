using Assets._Game.Scripts.Entities.Control.Intents;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class IntentModule : EntityModuleBase
    {
        private MoveIntent _move;
        private AimIntent _aim;
        private ActionIntent _act;

        public void SetMove(MoveIntent moveIntent)
        {
            _move = moveIntent;
        }

        public void SetAim(AimIntent aimIntent)
        {
            _aim = aimIntent;
        }

        public void SetAct(ActionIntent actIntent)
        {
            _act = actIntent;
        }

        public bool TryConsumeMove(out MoveIntent intent)
        {
            return TryConsumeIntent(ref _move, out intent);
        }

        public bool TryConsumeAim(out AimIntent intent)
        {
            return TryConsumeIntent(ref _aim, out intent);
        }

        public bool TryConsumeAction(out ActionIntent intent)
        {
            return TryConsumeIntent(ref _act, out intent);
        }

        // This method retrieves the current intent and resets it to default, effectively consuming it.
        private static bool TryConsumeIntent<T>(ref T intentProperty, out T intent) where T : IIntent
        {
            intent = intentProperty;
            intentProperty = default;
            return intent.HasIntent;
        }
    }

    public sealed class IntentModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            return new IntentModule();
        }
    }
}
