using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class GameInputUISystem : UISystemBase
    {
        [Inject]
        private void Construct(IGlobalEventBus globalEventBus)
        {
            BaseConstruct(globalEventBus);

            TrackGlobalEvent<PointerDownEvent>(OnPointerDown);
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (e.Context.IsOverUI)
            {
                GlobalEventBus.Publish(new ClickEffectRequest(e.Context.ScreenPosition));
            }
        }
    }
}
