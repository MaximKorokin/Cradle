using Assets.CoreScripts;
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
        private GameObject[] _tabElements;

        private Color _initialColor;

        private void Awake()
        {
            _initialColor = _image.color;
        }

        public override void SetSelection(bool selection, bool silent)
        {
            base.SetSelection(selection, silent);
            _image.color = selection ? _selectedColor : _initialColor;
            _tabElements.ForEach(x => x.SetActive(selection));
        }
    }
}
