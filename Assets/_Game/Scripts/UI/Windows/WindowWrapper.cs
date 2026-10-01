using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class WindowWrapper : WindowWrapperBase
    {
        [SerializeField]
        private RectTransform _headerTransform;
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private Button _closeButton;

        private void Awake()
        {
            _closeButton.onClick.AddListener(RequestClose);
        }

        public void SetupWrapperHeader(bool isActive, bool canClose, string title)
        {
            _headerTransform.gameObject.SetActive(isActive);
            _titleText.text = title;
            _closeButton.gameObject.SetActive(canClose);
        }
    }
}
