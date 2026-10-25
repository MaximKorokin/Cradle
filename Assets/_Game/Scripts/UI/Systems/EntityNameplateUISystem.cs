using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared.Utils;
using Assets._Game.Scripts.UI.Core;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Widgets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class EntityNameplateUISystem : UISystemBase
    {
        private IObjectResolver _resolver;
        private EntityRepository _entityRepository;

        private Camera _camera;
        private RectTransform _root;
        private EntityNameplateWidget _entityNameplateViewPrefab;
        private EntityViewService _entityViewService;

        private readonly Dictionary<EntityView, DataAggregatorController<EntityNameplateWidget, EntityNameplateWidgetData>> _items = new();

        [Inject]
        private void Construct(
            IObjectResolver resolver,
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            ICameraService cameraService,
            UIRootReferences roots,
            EntityNameplateWidget entityNameplateViewPrefab,
            EntityViewService entityViewService)
        {
            BaseConstruct(globalEventBus);

            _resolver = resolver;
            _entityRepository = entityRepository;

            _camera = cameraService.Camera;
            _root = roots.NameplatesRoot;
            _entityNameplateViewPrefab = entityNameplateViewPrefab;
            _entityViewService = entityViewService;

            TrackGlobalEvent<EntitySpawnedEvent>(OnEntitySpawned);
            TrackGlobalEvent<EntityDespawningEvent>(OnEntityDespawning);
        }

        private void OnEntitySpawned(EntitySpawnedEvent e)
        {
            if (!e.Entity.TryGetModule<CreatureModule>(out _)) return;

            var view = _entityViewService.GetEntityView(e.Entity);
            if (view != null)
            {
                CreateNameplate(e.Entity, view);
            }
        }

        private void OnEntityDespawning(EntityDespawningEvent e)
        {
            var view = _entityViewService.GetEntityView(e.Entity);
            if (view == null) return;

            DestroyNameplate(view);
        }

        private void LateUpdate()
        {
            foreach (var (target, controller) in _items)
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
                    controller.View.SetVisible(false);
                    continue;
                }

                controller.View.SetVisible(true);
                var screenPosition = _camera.WorldToScreenPoint(worldPosition);
                controller.View.SetPosition(screenPosition);
            }
        }

        private void CreateNameplate(Entity entity, EntityView entityView)
        {
            var view = _resolver.Instantiate(_entityNameplateViewPrefab, _root);
            var data = _resolver.Resolve<EntityNameplateWidgetData>();
            data.SetEntityId(_entityRepository.Observe(entity.Id));

            var controller = new DataAggregatorController<EntityNameplateWidget, EntityNameplateWidgetData>(view, data);

            _items[entityView] = controller;
        }

        private void DestroyNameplate(EntityView entityView)
        {
            if (_items.TryGetValue(entityView, out var controller))
            {
                controller.Dispose();

                if (controller.View != null && controller.View.gameObject != null)
                    Destroy(controller.View.gameObject);

                _items.Remove(entityView);
            }
        }

        public override void Dispose()
        {
            foreach (var key in _items.Keys.ToArray())
            {
                DestroyNameplate(key);
            }

            base.Dispose();
        }
    }
}
