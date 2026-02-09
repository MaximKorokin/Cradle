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
        private Button _button1;

        private object _identifierObject;

        public event Action<object> Button1Clicked;

        public void Render(SimpleListItemDefinition definition)
        {
            _identifierObject = definition.Identifier;
            _image.sprite = definition.Sprite;
            _text.text = definition.Text;
            _button1.onClick.AddListener(OnButton1Clicked);
        }

        public void Clear()
        {
            _identifierObject = null;
            _button1.onClick.RemoveListener(OnButton1Clicked);
        }

        private void OnButton1Clicked()
        {
            Button1Clicked?.Invoke(_identifierObject);
        }
    }

    public struct SimpleListItemDefinition
    {
        public object Identifier;
        public Sprite Sprite;
        public string Text;
    }
}
