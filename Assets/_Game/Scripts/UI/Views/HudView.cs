using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Views
{
    public class HudView : MonoBehaviour
    {
        [SerializeField]
        private Button _inventoryButton;
        [SerializeField]
        private Button _stashButton;

        public event Action InventoryButtonClicked;
        public event Action StashButtonClicked;

        [Inject]
        private void Construct()
        {
            _inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            _stashButton.onClick.AddListener(OnStashButtonClicked);
        }

        private void OnStashButtonClicked()
        {
            StashButtonClicked?.Invoke();
        }

        private void OnInventoryButtonClicked()
        {
            InventoryButtonClicked?.Invoke();
        }
    }
}
