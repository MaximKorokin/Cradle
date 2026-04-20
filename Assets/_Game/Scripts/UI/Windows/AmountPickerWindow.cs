using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class AmountPickerWindow : UIWindowBase
    {
        [SerializeField]
        private TMP_Text AmountText;
        [SerializeField]
        private Slider AmountSlider;
        [SerializeField]
        private Button ConfirmButton;
        [SerializeField]
        private Button IncrementButton;
        [SerializeField]
        private Button DecrementButton;
        [SerializeField]
        private Button HalfAmountButton;
        [SerializeField]
        private Button MaxAmountButton;

        private int _minAmount;
        private int _maxAmount;

        private int Amount
        {
            get => (int)AmountSlider.value;
            set
            {
                if (value < _minAmount || value > _maxAmount) return;

                AmountSlider.value = value;
            }
        }

        public event Action<int> AmountSelected;

        public override void OnShow()
        {
            base.OnShow();

            AmountSlider.onValueChanged.AddListener(OnAmountSliderValueChanged);
            ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            IncrementButton.onClick.AddListener(OnIncrementButtonClicked);
            DecrementButton.onClick.AddListener(OnDecrementButtonClicked);
            HalfAmountButton.onClick.AddListener(OnHalfAmountButtonClicked);
            MaxAmountButton.onClick.AddListener(OnMaxAmountButtonClicked);
        }

        public override void OnHide()
        {
            base.OnHide();

            AmountSlider.onValueChanged.RemoveListener(OnAmountSliderValueChanged);
            ConfirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            IncrementButton.onClick.RemoveListener(OnIncrementButtonClicked);
            DecrementButton.onClick.RemoveListener(OnDecrementButtonClicked);
            HalfAmountButton.onClick.RemoveListener(OnHalfAmountButtonClicked);
            MaxAmountButton.onClick.RemoveListener(OnMaxAmountButtonClicked);
        }

        public void Render(int minAmount, int maxAmount)
        {
            _minAmount = minAmount;
            _maxAmount = maxAmount;

            AmountSlider.minValue = _minAmount;
            AmountSlider.maxValue = _maxAmount;
            AmountSlider.wholeNumbers = true;
            Amount = _minAmount;

            OnAmountSliderValueChanged(Amount);
        }

        private void OnAmountSliderValueChanged(float value)
        {
            AmountText.text = $"{(int)value} / {_maxAmount}";
        }

        private void OnConfirmButtonClicked()
        {
            AmountSelected?.Invoke(Amount);
        }

        private void OnIncrementButtonClicked()
        {
            Amount++;
        }

        private void OnDecrementButtonClicked()
        {
            Amount--;
        }

        private void OnHalfAmountButtonClicked()
        {
            Amount = Mathf.FloorToInt((_maxAmount - _minAmount) / 2f) + _minAmount;
        }

        private void OnMaxAmountButtonClicked()
        {
            Amount = _maxAmount;
        }
    }
}
