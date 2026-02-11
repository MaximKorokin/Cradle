using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Windows.Controllers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemStacksPreviewWindow : UIWindowBase
    {
        [SerializeField]
        private ItemStacksPreviewView _primaryItemPreviewView;
        [SerializeField]
        private ItemStacksPreviewView _secondaryItemPreviewView;
        [SerializeField]
        private RectTransform _actionButtonParent;
        [SerializeField]
        private Button _actionButtonTemplate;

        private readonly List<Button> _actionButtons = new();

        public event Action<ItemStackActionType> ActionButtonClicked;

        public override void OnShow()
        {
            _actionButtonTemplate.gameObject.SetActive(false);
        }

        public override void OnHide()
        {

        }

        public void Render(ItemStackSnapshot? primaryItemStack, ItemStackSnapshot? secondaryItemStack, IEnumerable<ItemStackAction> actions)
        {
            Clear();
            if (primaryItemStack != null)
            {
                _primaryItemPreviewView.Render(primaryItemStack.Value);
            }
            if (secondaryItemStack != null)
            {
                _secondaryItemPreviewView.Render(secondaryItemStack.Value);
            }

            foreach (var action in actions)
            {
                var button = Instantiate(_actionButtonTemplate, _actionButtonParent);
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TMP_Text>().text = action.Title;
                button.onClick.AddListener(() =>
                {
                    ActionButtonClicked?.Invoke(action.Type);
                });
                _actionButtons.Add(button);
            }
        }

        public void Clear()
        {
            _primaryItemPreviewView.Clear();
            _secondaryItemPreviewView.Clear();

            foreach (var button in _actionButtons)
            {
                Destroy(button.gameObject);
            }
            _actionButtons.Clear();
        }
    }
}
