using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Windows;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : WindowControllerBase<CheatsWindow, EmptyWindowControllerArguments>
    {
        private CheatsWindow _window;

        private readonly ItemDefinitionCatalog _itemDefinitionCatalog;
        private readonly ItemStackAssembler _itemStackAssembler;
        private readonly PlayerContext _playerContext;

        public CheatsWindowController(
            ItemDefinitionCatalog itemDefinitionCatalog,
            ItemStackAssembler itemStackAssembler,
            PlayerContext playerContext)
        {
            _itemDefinitionCatalog = itemDefinitionCatalog;
            _itemStackAssembler = itemStackAssembler;
            _playerContext = playerContext;
        }

        public override void Bind(CheatsWindow window)
        {
            _window = window;
            window.ItemDefinitionClicked += OnItemDefinitionClicked;
            _window.Render(_itemDefinitionCatalog);
        }

        public override void Dispose()
        {
            _window.ItemDefinitionClicked -= OnItemDefinitionClicked;
        }

        private void OnItemDefinitionClicked(ItemDefinition itemDefinition)
        {
            if (_playerContext.Player.TryGetModule<InventoryEquipmentModule>(out var ieModule))
            {
                ieModule.Inventory.Add(_itemStackAssembler.Create(itemDefinition.Id, itemDefinition.MaxAmount).Snapshot);
            }
        }
    }
}
