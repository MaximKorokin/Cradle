using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using System;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CheatsWindowController : IDisposable
    {
        private readonly CheatsWindow _window;
        private readonly ItemDefinitionCatalog _itemDefinitionCatalog;
        private readonly ItemStackAssembler _itemStackAssembler;
        private readonly PlayerContext _playerContext;

        public CheatsWindowController(
            CheatsWindow window,
            ItemDefinitionCatalog itemDefinitionCatalog,
            ItemStackAssembler itemStackAssembler,
            PlayerContext playerContext)
        {
            _window = window;
            _itemDefinitionCatalog = itemDefinitionCatalog;
            _itemStackAssembler = itemStackAssembler;
            _playerContext = playerContext;

            window.ItemDefinitionClicked += OnItemDefinitionClicked;

            Redraw();
        }

        public void Redraw()
        {
            _window.Render(_itemDefinitionCatalog);
        }

        private void OnItemDefinitionClicked(ItemDefinition itemDefinition)
        {
            if (_playerContext.Player.TryGetModule<InventoryEquipmentModule>(out var ieModule))
            {
                ieModule.Inventory.Add(_itemStackAssembler.Create(itemDefinition.Id, itemDefinition.MaxAmount).Snapshot);
            }
        }

        public void Dispose()
        {
            _window.ItemDefinitionClicked -= OnItemDefinitionClicked;
        }
    }
}
