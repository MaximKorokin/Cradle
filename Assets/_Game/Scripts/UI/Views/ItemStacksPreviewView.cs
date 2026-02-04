using Assets._Game.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class ItemStacksPreviewView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _title;
        [SerializeField]
        private Image _iconView;

        public void Render(ItemStack itemStack)
        {
            _title.text = itemStack.Definition.Name;
            _iconView.sprite = itemStack.Definition.Icon;
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            _title.text = string.Empty;
            _iconView.sprite = null;
        }
    }
}
