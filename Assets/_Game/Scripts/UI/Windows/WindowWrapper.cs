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
        private Image _contentBackgroundImage;
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private Button _closeButton;

        private void Awake()
        {
            _closeButton.onClick.AddListener(RequestClose);
        }

        public void SetupWrapperHeader(WindowSettings settings)
        {
            _headerTransform.gameObject.SetActive(settings.ShowHeader);
            _titleText.gameObject.SetActive(settings.ShowHeaderText);
            _titleText.text = settings.HeaderText;
            _closeButton.gameObject.SetActive(settings.ShowCloseButton);

            _contentBackgroundImage.enabled = settings.ShowContentBackground;
        }
    }
}
