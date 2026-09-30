using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ItemUseSettingsView : UIViewBase<ItemUseSettingsViewData>
    {
        [SerializeField]
        private TMP_Text _hpPercentText;
        [SerializeField]
        private Slider _hpPercentSlider;
        [SerializeField]
        private Toggle _overrideStatusEffectsToggle;

        public event Action<ItemUseSettings> Changed;

        protected override void Render(ItemUseSettingsViewData itemUseSettingsViewData)
        {
            _hpPercentSlider.SetValueWithoutNotify(itemUseSettingsViewData.ItemUseSettings.HpPercent);
            _hpPercentText.text = $"{itemUseSettingsViewData.ItemUseSettings.HpPercent}%";
            _overrideStatusEffectsToggle.SetIsOnWithoutNotify(itemUseSettingsViewData.ItemUseSettings.OverrideStatusEffects);

            _hpPercentSlider.onValueChanged.AddListener(OnHpPercentSliderValueChanged);
            _overrideStatusEffectsToggle.onValueChanged.AddListener(OnOverrideStatusEffectsToggleValueChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _hpPercentSlider.onValueChanged.RemoveListener(OnHpPercentSliderValueChanged);
            _overrideStatusEffectsToggle.onValueChanged.RemoveListener(OnOverrideStatusEffectsToggleValueChanged);
        }

        private void OnHpPercentSliderValueChanged(float value)
        {
            var intValue = (int)_hpPercentSlider.value;
            _hpPercentText.text = $"{intValue}%";
            NotifyChanged();
        }

        private void OnOverrideStatusEffectsToggleValueChanged(bool value)
        {
            NotifyChanged();
        }

        private void NotifyChanged()
        {
            Changed?.Invoke(new ItemUseSettings((int)_hpPercentSlider.value, _overrideStatusEffectsToggle.isOn));
        }
    }
}
