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

        public event Action<string> InfoButtonClicked;
        public event Action<string> ActionButtonClicked;

        public void Render(SimpleListItemData definition)
        {
            _identifier = definition.Identifier;
            _image.sprite = definition.Sprite;
            _text.text = definition.Text;
            _infoButton.onClick.AddListener(OnInfoButtonClicked);
            _actionButton.onClick.AddListener(OnActionButtonClicked);
        }

        public void Clear()
        {
            _identifier = null;
            _infoButton.onClick.RemoveListener(OnInfoButtonClicked);
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
    }
}
