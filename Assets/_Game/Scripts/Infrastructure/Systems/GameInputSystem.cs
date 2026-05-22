using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class GameInputSystem : SystemBase
    {
        private readonly InputAction _clickAction;

        public GameInputSystem(IGlobalEventBus globalEventBus) : base(globalEventBus)
        {
            _clickAction = InputSystem.actions.FindAction("Click");

            _clickAction.performed += OnClick;
            _clickAction.Enable();
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (!context.ReadValueAsButton()) return;

            var screenPosition = Pointer.current?.position.ReadValue() ?? Vector2.zero;
            GlobalEventBus.Publish(new ClickEffectRequest(screenPosition));
        }

        public override void Dispose()
        {
            _clickAction.performed -= OnClick;
            _clickAction.Disable();

            base.Dispose();
        }
    }
}
