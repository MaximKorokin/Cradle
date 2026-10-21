using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.UI.DataAggregators;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class StatusEffectListViewController : ViewControllerBase<StatusEffectListView, StatusEffectListViewData>
    {
        private readonly IObjectResolver _objectResolver;

        private readonly List<GameObject> _instantiatedGameObjects = new();
        private readonly List<DataAggregatorController<StatusEffectView, StatusEffectViewData>> _viewControllers = new();

        public StatusEffectListViewController(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        protected override void OnRender()
        {
            Clear();

            foreach (var buff in Data.Buffs)
            {
                CreateView(View.BuffTemplate, View.BuffsContainer, buff);
            }
            foreach (var debuff in Data.Debuffs)
            {
                CreateView(View.DebuffTemplate, View.DebuffsContainer, debuff);
            }

            base.OnRender();
        }

        private void CreateView(StatusEffectView template, Transform parent, StatusEffectSnapshot statusEffect)
        {
            var view = _objectResolver.Instantiate(template, parent);
            view.gameObject.SetActive(true);
            _instantiatedGameObjects.Add(view.gameObject);

            var data = _objectResolver.Resolve<StatusEffectViewData>();
            data.SetStatusEffect(statusEffect.Definition);
            data.SetEntityId(Data.ObservableEntityId);

            var controller = new DataAggregatorController<StatusEffectView, StatusEffectViewData>(view, data);
            _viewControllers.Add(controller);
        }

        private void Clear()
        {
            foreach (var controller in _viewControllers)
            {
                controller.Dispose();
            }
            _viewControllers.Clear();

            foreach (var obj in _instantiatedGameObjects)
            {
                Object.Destroy(obj);
            }
            _instantiatedGameObjects.Clear();
        }
    }
}
