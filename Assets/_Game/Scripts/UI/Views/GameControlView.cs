using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class GameControlView : MonoBehaviour
    {
        [SerializeField]
        private Button _resetPlayerQuestsButton;
        [SerializeField]
        private Button _resetPlayerLevelButton;

        public event Action ResetPlayerQuestsButtonClicked;
        public event Action ResetPlayerLevelButtonClicked;

        private void Awake()
        {
            _resetPlayerQuestsButton.onClick.AddListener(OnResetPlayerQuestsButtonClicked);
            _resetPlayerLevelButton.onClick.AddListener(OnResetPlayerLevelButtonClicked);
        }

        private void OnResetPlayerQuestsButtonClicked()
        {
            ResetPlayerQuestsButtonClicked?.Invoke();
        }

        private void OnResetPlayerLevelButtonClicked()
        {
            ResetPlayerLevelButtonClicked?.Invoke();
        }
    }
}
