using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems.Tooltip
{
    public sealed class TooltipHandlerService : IDisposable
    {
        private readonly IObjectResolver _resolver;
        private readonly UITooltipConfig _config;
        private readonly ItemStackPreviewViewData _itemStackPreviewViewData;
        private readonly StatusEffectViewData _statusEffectViewData;
        private readonly EntityRepository _entityRepository;

        private ItemStackPreviewView _itemStackPreviewView;
        private TextViewWidget _textView;
        private StatusEffectDescriptionView _statusEffectDescriptionView;

        private IDisposable _currentController;

        public TooltipHandlerService(
            IObjectResolver resolver,
            UITooltipConfig config,
            ItemStackPreviewViewData itemStackPreviewViewData,
            StatusEffectViewData statusEffectViewData,
            EntityRepository entityRepository)
        {
            _resolver = resolver;
            _config = config;
            _itemStackPreviewViewData = itemStackPreviewViewData;
            _statusEffectViewData = statusEffectViewData;
            _entityRepository = entityRepository;
        }

        public (RectTransform Content, Action CleanAction) Handle(ITooltipSource tooltipSource)
        {
            if (tooltipSource is ItemStackWidget containerSlot && containerSlot.ContainsData)
                return HandleContainerSlot(containerSlot);
            else if (tooltipSource is WindowOpenTrigger windowOpenTrigger)
                return HandleWindowOpenTrigger(windowOpenTrigger);
            else if (tooltipSource is StatusEffectView statusEffectWidget)
                return HandleStatusEffect(statusEffectWidget);

            return (null, null);
        }

        private (RectTransform Content, Action CleanAction) HandleContainerSlot(ItemStackWidget itemStackWidget)
        {
            _resolver.InstantiateFromPrefab(_config.ItemStackPreviewViewPrefab, ref _itemStackPreviewView);

            _itemStackPreviewViewData.SetData(itemStackWidget.ContainerPath, itemStackWidget.GetItemSlotIndex(), false);
            _itemStackPreviewViewData.SetEntityId(_entityRepository.Observe(itemStackWidget.ContainerPath.EntityId));
            _currentController = new DataAggregatorController<ItemStackPreviewView, ItemStackPreviewViewData>(_itemStackPreviewView, _itemStackPreviewViewData);
            _itemStackPreviewView.gameObject.SetActive(true);

            return (_itemStackPreviewView.transform as RectTransform, CleanState);
        }

        private (RectTransform Content, Action CleanAction) HandleWindowOpenTrigger(WindowOpenTrigger windowOpenTrigger)
        {
            _resolver.InstantiateFromPrefab(_config.TextViewPrefab, ref _textView);

            _textView.Render($"Open {windowOpenTrigger.WindowId}");
            _textView.gameObject.SetActive(true);

            return (_textView.transform as RectTransform, CleanState);
        }

        private (RectTransform Content, Action CleanAction) HandleStatusEffect(StatusEffectView statusEffectWidget)
        {
            _resolver.InstantiateFromPrefab(_config.StatusEffectDescriptionViewPrefab, ref _statusEffectDescriptionView);

            _statusEffectViewData.SetEntityId(statusEffectWidget.Data.ObservableEntityId);
            _statusEffectViewData.SetStatusEffect(statusEffectWidget.StatusEffectSnapshot.Definition);
            _currentController = new DataAggregatorController<StatusEffectDescriptionView, StatusEffectViewData>(_statusEffectDescriptionView, _statusEffectViewData);
            _statusEffectDescriptionView.gameObject.SetActive(true);

            return (_statusEffectDescriptionView.transform as RectTransform, CleanState);
        }

        public void Dispose()
        {
            _itemStackPreviewViewData.Dispose();
        }

        private void CleanState()
        {
            if (_itemStackPreviewView != null && _itemStackPreviewView.gameObject.activeSelf)
            {
                _itemStackPreviewView.Clear();
                _itemStackPreviewView.gameObject.SetActive(false);
            }

            if (_textView != null && _textView.gameObject.activeSelf)
                _textView.Clear();

            if (_statusEffectDescriptionView != null && _statusEffectDescriptionView.gameObject.activeSelf)
            {
                _statusEffectDescriptionView.Clear();
                _statusEffectDescriptionView.gameObject.SetActive(false);
            }

            _currentController?.Dispose();
            _currentController = null;
        }
    }
}
