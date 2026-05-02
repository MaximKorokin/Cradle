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

        private string _identifier;

        public event Action<string> Button1Clicked;

        public void Render(SimpleListItemData definition)
        {
            _identifier = definition.Identifier;
            _image.sprite = definition.Sprite;
            _text.text = definition.Text;
            _button1.onClick.AddListener(OnButton1Clicked);
        }

        public void Clear()
        {
            _identifier = null;
            _button1.onClick.RemoveListener(OnButton1Clicked);
        }

        private void OnButton1Clicked()
        {
            Button1Clicked?.Invoke(_identifier);
        }
    }

    public struct SimpleListItemData
    {
        public string Identifier;
        public Sprite Sprite;
        public string Text;
    }
}
