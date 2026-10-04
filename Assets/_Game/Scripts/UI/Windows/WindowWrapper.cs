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

        public void SetupWrapperHeader(bool isActive, bool canClose, bool showTitle, string title)
        {
            _headerTransform.gameObject.SetActive(isActive);
            _titleText.gameObject.SetActive(showTitle);
            _titleText.text = title;
            _closeButton.gameObject.SetActive(canClose);
        }
    }
}
