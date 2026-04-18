using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class PlayerReviveView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _viewGameObject;
        [SerializeField]
        private Button _reviveButton;

        public event Action ReviveButtonClicked;

        private void Awake()
        {
            _reviveButton.onClick.AddListener(() => ReviveButtonClicked?.Invoke());
        }

        public void Show()
        {
            _viewGameObject.SetActive(true);
        }

        public void Hide()
        {
            _viewGameObject.SetActive(false);
        }
    }
}
