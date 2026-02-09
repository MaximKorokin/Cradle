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
        [SerializeField]
        private Button _statsButton;
        [SerializeField]
        private Button _cheatsButton;

        public event Action InventoryButtonClicked;
        public event Action StashButtonClicked;
        public event Action StatsButtonClicked;
        public event Action CheatsButtonClicked;

        [Inject]
        private void Construct()
        {
            _inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            _stashButton.onClick.AddListener(OnStashButtonClicked);
            _statsButton.onClick.AddListener(OnStatsButtonClicked);
            _cheatsButton.onClick.AddListener(OnCheatsButtonClicked);
        }

        private void OnStashButtonClicked()
        {
            StashButtonClicked?.Invoke();
        }

        private void OnInventoryButtonClicked()
        {
            InventoryButtonClicked?.Invoke();
        }

        private void OnStatsButtonClicked()
        {
            StatsButtonClicked?.Invoke();
        }

        private void OnCheatsButtonClicked()
        {
            CheatsButtonClicked?.Invoke();
        }
    }
}
