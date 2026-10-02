using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.UI.Systems.Tooltip
{
    public sealed class TooltipHandlerService : IDisposable
    {
        private readonly IObjectResolver _resolver;
        private readonly UITooltipConfig _config;
        private readonly ItemStackPreviewViewData _itemStackPreviewViewData;

        private ItemStackPreviewView _itemStackPreviewView;
        private TextViewWidget _textView;

        private IDisposable _currentController;

        public TooltipHandlerService(
            IObjectResolver resolver,
            UITooltipConfig config,
            ItemStackPreviewViewData itemStackPreviewViewData)
        {
            _resolver = resolver;
            _config = config;
            _itemStackPreviewViewData = itemStackPreviewViewData;
        }

        public (RectTransform Content, Action CleanAction) Handle(ITooltipSource tooltipSource)
        {
            if (tooltipSource is ContainerSlotWidget containerSlot && containerSlot.ContainsData)
            {
                InstantiateFromPrefab(_config.ItemStackPreviewViewPrefab, ref _itemStackPreviewView);

                _itemStackPreviewViewData.SetData(containerSlot.ContainerPath, containerSlot.SlotIndex, false);
                _itemStackPreviewViewData.SetEntityId(new ObservableData<string>(containerSlot.ContainerPath.EntityId));
                _currentController = new DataAggregatorController<ItemStackPreviewView, ItemStackPreviewViewData>(_itemStackPreviewView, _itemStackPreviewViewData);
                _itemStackPreviewView.gameObject.SetActive(true);

                return (_itemStackPreviewView.transform as RectTransform, CleanState);
            }
            else if (tooltipSource is WindowOpenTrigger windowOpenTrigger)
            {
                InstantiateFromPrefab(_config.TextViewPrefab, ref _textView);

                _textView.Render($"Open {windowOpenTrigger.WindowId}");
                _textView.gameObject.SetActive(true);

                return (_textView.transform as RectTransform, CleanState);
            }

            return (null, null);
        }

        public void Dispose()
        {
            _itemStackPreviewViewData.Dispose();
        }

        private void InstantiateFromPrefab<T>(T prefab, ref T instance) where T : Component
        {
            if (instance == null)
            {
                instance = _resolver.Instantiate(prefab);
                instance.gameObject.SetActive(false);
            }
        }

        private void CleanState()
        {
            if (_itemStackPreviewView != null && _itemStackPreviewView.gameObject.activeSelf)
                _itemStackPreviewView.Clear();

            if (_textView != null && _textView.gameObject.activeSelf)
                _textView.Clear();

            _currentController?.Dispose();
            _currentController = null;
        }
    }
}
