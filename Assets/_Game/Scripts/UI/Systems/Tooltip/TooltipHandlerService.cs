using Assets._Game.Scripts.Shared;
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
        private readonly ItemStackPreviewView _itemStackPreviewView;
        private readonly ItemStackPreviewViewData _itemStackPreviewViewData;

        private IDisposable _currentController;

        public TooltipHandlerService(
            IObjectResolver resolver,
            ItemStackPreviewView itemStackPreviewViewPrefab,
            ItemStackPreviewViewData itemStackPreviewViewData)
        {
            _itemStackPreviewView = resolver.Instantiate(itemStackPreviewViewPrefab);
            _itemStackPreviewViewData = itemStackPreviewViewData;
            CleanState();
        }

        public (RectTransform Content, Action CleanAction) Handle(ITooltipSource tooltipSource)
        {
            if (tooltipSource is ContainerSlotWidget containerSlot && containerSlot.ContainsData)
            {
                _itemStackPreviewViewData.SetData(containerSlot.ContainerPath, containerSlot.SlotIndex, false);
                _itemStackPreviewViewData.SetEntityId(new ObservableData<string>(containerSlot.ContainerPath.EntityId));
                _currentController = new DataAggregatorController<ItemStackPreviewView, ItemStackPreviewViewData>(_itemStackPreviewView, _itemStackPreviewViewData);
                _itemStackPreviewView.gameObject.SetActive(true);

                return (_itemStackPreviewView.transform as RectTransform, CleanState);
            }

            return (null, null);
        }

        public void Dispose()
        {
            _itemStackPreviewViewData.Dispose();
        }

        private void CleanState()
        {
            _itemStackPreviewView.gameObject.SetActive(false);
            _currentController?.Dispose();
            _currentController = null;
        }
    }
}
