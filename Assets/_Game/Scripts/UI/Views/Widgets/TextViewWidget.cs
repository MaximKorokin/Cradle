using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class TextViewWidget : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        public void Render(string text)
        {
            _text.text = text;
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            _text.text = string.Empty;
        }
    }
}
