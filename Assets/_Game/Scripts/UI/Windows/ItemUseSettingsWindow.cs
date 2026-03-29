using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Windows
{
	public sealed class ItemUseSettingsWindow : UIWindowBase
	{
		[SerializeField]
		private TMP_Text _hpPercentText;
		[SerializeField]
		private Slider _hpPercentSlider;
		[SerializeField]
		private Toggle _overrideStatusEffectsToggle;

		public Action<ItemUseSettings> Changed;

		private void Awake()
		{
			_hpPercentSlider.onValueChanged.AddListener(OnHpPercentSliderValueChanged);
			_overrideStatusEffectsToggle.onValueChanged.AddListener(OnOverrideStatusEffectsToggleValueChanged);
		}

		public void Render(EquipmentHudData equipmentHudData)
		{
			_hpPercentSlider.SetValueWithoutNotify(equipmentHudData.ItemUseSettings.HpPercent);
			_hpPercentText.text = $"{equipmentHudData.ItemUseSettings.HpPercent}%";
			_overrideStatusEffectsToggle.SetIsOnWithoutNotify(equipmentHudData.ItemUseSettings.OverrideStatusEffects);
		}

		private void OnHpPercentSliderValueChanged(float value)
		{
			var intValue = (int)_hpPercentSlider.value;
			_hpPercentText.text = $"{intValue}%";
			InvokeChanged();
		}

		private void OnOverrideStatusEffectsToggleValueChanged(bool value)
		{
			InvokeChanged();
		}

		private void InvokeChanged()
		{
			Changed?.Invoke(new ItemUseSettings((int)_hpPercentSlider.value, _overrideStatusEffectsToggle.isOn));
		}
	}
}
