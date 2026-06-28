using Assets.CoreScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    public class SelectableTab : SelectableElement
    {
        [SerializeField]
        private Color _selectedColor = Color.white;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _title;
        [SerializeField]
        private RectTransform[] _tabElements;

        private Color _initialColor;

        public string Id { get; private set; }

        private void Awake()
        {
            _initialColor = _image.color;
        }

        public override void SetSelection(bool selection, bool silent)
        {
            base.SetSelection(selection, silent);
            _image.color = selection ? _selectedColor : _initialColor;
            _tabElements.ForEach(x => x.gameObject.SetActive(selection));
        }

        public void Initialize(string id, string title, params RectTransform[] tabElements)
        {
            Id = id;

            _title.text = title;

            _tabElements = tabElements;
            _tabElements.ForEach(x => x.gameObject.SetActive(false));
        }
    }
}
