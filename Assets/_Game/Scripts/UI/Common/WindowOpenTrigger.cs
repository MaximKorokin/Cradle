using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Windows;
using Assets._Game.Scripts.UI.Windows.Controllers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Common
{
    [RequireComponent(typeof(Button))]
    public sealed class WindowOpenTrigger : MonoBehaviour
    {
        [SerializeField] private WindowId _windowId;

        private WindowManager _windowManager;
        private IPlayerProvider _playerProvider;

        [Inject]
        private void Construct(
            WindowManager windowManager,
            IPlayerProvider playerProvider)
        {
            _windowManager = windowManager;
            _playerProvider = playerProvider;
        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(InstantiateWindow);
        }

        private void InstantiateWindow()
        {
            var playerEntityId = _playerProvider.Player.Id;
            switch (_windowId)
            {
                case WindowId.InventoryEquipment:
                    _windowManager.InstantiateWindow<InventoryEquipmentWindow, InventoryEquipmentWindowControllerArguments>(new(playerEntityId, playerEntityId));
                    break;
                case WindowId.ItemUseSettings:
                    _windowManager.InstantiateWindow<ItemUseSettingsWindow, ItemUseSettingsWindowControllerArguments>(new(playerEntityId));
                    break;
                case WindowId.Cheats:
                    _windowManager.InstantiateWindow<CheatsWindow, CheatsWindowControllerArguments>(new(playerEntityId, playerEntityId));
                    break;
                case WindowId.Quests:
                    _windowManager.InstantiateWindow<QuestsWindow, QuestsWindowControllerArguments>(new(playerEntityId));
                    break;
                default:
                    _windowManager.InstantiateWindow(_windowId);
                    return;
            }
        }
    }
}
