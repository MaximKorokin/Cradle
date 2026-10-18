using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ItemEnchantingView : UIViewBase<ItemEnchantingViewData>
    {
        [field: SerializeField]
        public ItemStackWidget EnchantableItemWidget { get; private set; }
        [field: SerializeField]
        public ItemStackWidget EnchantingItemWidget { get; private set; }
        [SerializeField]
        private TMP_Text _probabilityText;
        [SerializeField]
        private TMP_Text _enchantResultText;
        [SerializeField]
        private Button _enchantButton;
        [SerializeField]
        private Button _cancelButton;
        [SerializeField]
        private RectTransform _enchantEffectsParent;
        [SerializeField]
        private TMP_Text _enchantEffectTemplate;
        [SerializeField]
        private TMP_Text _disabledEnchantEffectTemplate;

        private readonly List<GameObject> _instantiatedTemplates = new();

        private string _initialProbabilityText;
        private string _initialEnchantResultText;

        public event Action EnchantButtonClicked;
        public event Action CancelButtonClicked;

        protected override void Awake()
        {
            base.Awake();

            _enchantEffectTemplate.gameObject.SetActive(false);
            _disabledEnchantEffectTemplate.gameObject.SetActive(false);

            _initialProbabilityText = _probabilityText.text;
            _initialEnchantResultText = _enchantResultText.text;

            EnchantableItemWidget.BindChanged += OnBindChanged;
            EnchantingItemWidget.BindChanged += OnBindChanged;

            _enchantButton.onClick.AddListener(OnEnchantButtonCicked);
            _cancelButton.onClick.AddListener(OnCancelButtonCicked);
        }

        private void OnBindChanged()
        {
            Data.SetEnchantableItem(EnchantableItemWidget.ItemStackPath);
            Data.SetEnchantingItem(EnchantingItemWidget.ItemStackPath);
        }

        private void OnEnchantButtonCicked()
        {
            EnchantButtonClicked?.Invoke();
        }

        private void OnCancelButtonCicked()
        {
            CancelButtonClicked?.Invoke();
        }

        protected override void Render(ItemEnchantingViewData data)
        {
            Clear();

            EnchantableItemWidget.Bind(data.EnchantableItemPath);
            EnchantingItemWidget.Bind(data.EnchantingItemPath);
            EnchantableItemWidget.Render(data.GetEnchantableItem());
            EnchantingItemWidget.Render(data.GetEnchantingItem());

            var displayData = Data.GetEnchantDisplayData();
            if (!displayData.HasData) return;

            foreach (var bonus in displayData.LevelBonuses)
            {
                var template = bonus.IsEnabled ? _enchantEffectTemplate : _disabledEnchantEffectTemplate;
                var obj = template.TryCreateFromFormattedTemplate(_enchantEffectsParent, $"+{bonus.Level}", bonus.BonusDescription);
                _instantiatedTemplates.Add(obj);
            }

            if (Data.CanEnchant())
            {
                _enchantButton.interactable = true;
                var enchantingItemData = data.GetEnchantItemDisplayData(displayData);
                _probabilityText.text = _initialProbabilityText.FormatSafe(new string[] {
                    "Success rate",
                    $"{enchantingItemData.ProbabilityPerlevel[displayData.CurrentLevel] * 100}%" });
                _enchantResultText.text = _initialEnchantResultText.FormatSafe(new string[] {
                    "In case of failure: ",
                    enchantingItemData.FailurePerLevel[displayData.CurrentLevel].ToString() });
            }
        }

        private void Clear()
        {
            for (int i = 0; i < _instantiatedTemplates.Count; i++)
            {
                Destroy(_instantiatedTemplates[i]);
            }
            _instantiatedTemplates.Clear();

            _probabilityText.text = "";
            _enchantResultText.text = "";

            _enchantButton.interactable = false;
        }
    }
}
