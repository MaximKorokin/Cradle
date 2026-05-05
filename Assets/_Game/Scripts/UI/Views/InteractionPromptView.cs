using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _viewGameObject;
        [SerializeField]
        private TMP_Text _promptText;
        [SerializeField]
        private TMP_Text _buttonText;
        [SerializeField]
        private Button _button;

        private Action _callback;

        public void Show(string promptText, string buttonText, Action callback)
        {
            _callback = callback;

            _promptText.text = promptText;
            _buttonText.text = buttonText;
            _viewGameObject.SetActive(true);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void Hide()
        {
            _viewGameObject.SetActive(false);

            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _callback?.Invoke();
        }
    }
}
