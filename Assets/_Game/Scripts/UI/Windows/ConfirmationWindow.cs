using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ConfirmationWindow : UIWindowBase
    {
        [SerializeField]
        private TMP_Text TitleText;
        [SerializeField]
        private TMP_Text MessageText;
        [SerializeField]
        private Button ConfirmButton;
        [SerializeField]
        private Button CancelButton;

        public event Action<bool> ConfirmationResult;

        public override void OnShow()
        {
            base.OnShow();

            ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            CancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        public override void OnHide()
        {
            base.OnHide();

            ConfirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            CancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }

        public void Render(string title, string message)
        {
            TitleText.text = title;
            MessageText.text = message;
        }

        private void OnConfirmButtonClicked()
        {
            ConfirmationResult?.Invoke(true);
        }

        private void OnCancelButtonClicked()
        {
            ConfirmationResult?.Invoke(false);
        }
    }
}
