using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Utils;
using Assets._Game.Scripts.UI.Core;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class EntityNameplateUISystem : UISystemBase
    {
        private Camera _camera;
        private RectTransform _root;
        private EntityNameplateView _entityNameplateViewPrefab;
        private EntityViewService _entityViewService;

        private readonly Dictionary<EntityView, EntityNameplateView> _items = new();

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            ICameraService cameraService,
            UIRootReferences roots,
            EntityNameplateView entityNameplateViewPrefab,
            EntityViewService entityViewService)
        {
            BaseConstruct(globalEventBus);

            _camera = cameraService.Camera;
            _root = roots.NameplatesRoot;
            _entityNameplateViewPrefab = entityNameplateViewPrefab;
            _entityViewService = entityViewService;

            TrackGlobalEvent<EntitySpawnedEvent>(OnEntitySpawned);
            TrackGlobalEvent<EntityDespawningEvent>(OnEntityDespawning);
        }

        private void OnEntitySpawned(EntitySpawnedEvent e)
        {
            if (!e.Entity.TryGetModule<StatModule>(out _)) return;

            var view = _entityViewService.GetEntityView(e.Entity);
            if (view != null)
            {
                Register(view, Create(e.Entity, view));
            }
        }

        private void OnEntityDespawning(EntityDespawningEvent e)
        {
            var view = _entityViewService.GetEntityView(e.Entity);
            if (view == null) return;

            var nameplate = _items.GetValueOrDefault(view);
            if (nameplate == null) return;

            Unregister(view);
            Destroy(nameplate.gameObject);
        }

        private void Register(EntityView entityView, EntityNameplateView view)
        {
            _items[entityView] = view;
        }

        private void Unregister(EntityView entityView)
        {
            _items.Remove(entityView);
        }

        private void LateUpdate()
        {
            foreach (var (target, view) in _items)
            {
                if (target == null)
                    continue;

                var bounds = EntityViewUtils.GetRenderersBounds(target);
                var worldPosition = new Vector3(bounds.center.x, bounds.max.y + 0.1f);
                var vp = _camera.WorldToViewportPoint(worldPosition);

                bool visible =
                    vp.z > 0 &&
                    vp.x >= 0 && vp.x <= 1 &&
                    vp.y >= 0 && vp.y <= 1;

                if (!visible)
                {
                    view.SetVisible(false);
                    continue;
                }

                view.SetVisible(true);

                var screenPosition = _camera.WorldToScreenPoint(worldPosition);
                view.SetPosition(screenPosition);
            }
        }

        private EntityNameplateView Create(Entity entity, EntityView entityView)
        {
            var view = Instantiate(_entityNameplateViewPrefab, _root);
            view.Bind(new EntityNameplateViewData(entity));
            Register(entityView, view);
            return view;
        }
    }
}
