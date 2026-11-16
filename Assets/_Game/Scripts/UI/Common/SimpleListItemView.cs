using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    public class SimpleListItemView : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private Button _infoButton;
        [SerializeField]
        private Button _actionButton;

        private string _identifier;

        private string _initialText;
        private string InitialText => _initialText ??= _text.text;

        public event Action<string> InfoButtonClicked;
        public event Action<string> ActionButtonClicked;

        public void Render(SimpleListItemData definition)
        {
            _identifier = definition.Identifier;

            if (_image != null)
            {
                _image.gameObject.SetActive(definition.Sprite != null);
                _image.sprite = definition.Sprite;
            }

            if (_text != null)
            {
                if (definition.TextFactory != null)
                    _text.text = definition.TextFactory(InitialText);
                else
                    _text.text = definition.Text;
            }

            if (_infoButton != null)
                _infoButton.onClick.AddListener(OnInfoButtonClicked);

            if (_actionButton != null)
                _actionButton.onClick.AddListener(OnActionButtonClicked);

            definition.ItemCreationCallback?.Invoke(this);
        }

        public void Clear()
        {
            _identifier = null;

            if (_infoButton != null)
                _infoButton.onClick.RemoveListener(OnInfoButtonClicked);

            if (_actionButton != null)
                _actionButton.onClick.RemoveListener(OnActionButtonClicked);
        }

        private void OnInfoButtonClicked()
        {
            InfoButtonClicked?.Invoke(_identifier);
        }

        private void OnActionButtonClicked()
        {
            ActionButtonClicked?.Invoke(_identifier);
        }
    }

    public struct SimpleListItemData
    {
        public string Identifier;
        public Sprite Sprite;
        public string Text;
        /// <summary> A factory function to generate the text dynamically based on the template text.Will be used instead of the Text field if provided. </summary>
        public Func<string, string> TextFactory;
        public Action<SimpleListItemView> ItemCreationCallback;
    }
}
