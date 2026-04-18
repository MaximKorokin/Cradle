using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.UI.Core;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class FloatingTextUISystem : UISystemBase
    {
        private ICameraService _cameraService;
        private RectTransform _root;
        private FloatingTextConfig _floatingTextConfig;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            ICameraService cameraService,
            UIRootReferences roots,
            FloatingTextConfig floatingTextConfig)
        {
            BaseConstruct(globalEventBus);

            _cameraService = cameraService;
            _root = roots.FloatingTextRoot;
            _floatingTextConfig = floatingTextConfig;

            TrackGlobalEvent<FloatingTextRequest>(OnFloatingTextRequested);
        }

        private void OnFloatingTextRequested(FloatingTextRequest e)
        {
            var view = Instantiate(_floatingTextConfig.FloatingTextPrefab, _root);
            var position = (Vector2)e.WorldPosition + _floatingTextConfig.Offset + Random.insideUnitCircle * _floatingTextConfig.RandomOffset;
            var screenPosition = _cameraService.Camera.WorldToScreenPoint(position);
            view.transform.position = screenPosition;
            view.Bind(e.Text, e.Style);
        }
    }
}
