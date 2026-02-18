using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class CheatsWindowController : WindowControllerBase<CheatsWindow, EmptyWindowControllerArguments>
    {
        private CheatsWindow _window;

        private readonly CheatsHudData _cheatsHudData;
        private readonly PlayerContext _playerContext;
        private readonly ItemStackAssembler _itemStackAssembler;

        public CheatsWindowController(
            CheatsHudData cheatsHudData,
            PlayerContext playerContext,
            ItemStackAssembler itemStackAssembler)
        {
            _cheatsHudData = cheatsHudData;
            _playerContext = playerContext;
            _itemStackAssembler = itemStackAssembler;
        }

        public override void Bind(CheatsWindow window)
        {
            _window = window;
            window.ItemDefinitionClicked += OnItemDefinitionClicked;
            window.StatusEffectDefinitionClicked += OnStatusEffectDefinitionClicked;
            _window.Render(_cheatsHudData);
        }

        public override void Dispose()
        {
            _window.ItemDefinitionClicked -= OnItemDefinitionClicked;
        }

        private void OnStatusEffectDefinitionClicked(StatusEffectDefinition statusEffectDefinition)
        {
            if (_playerContext.Player.TryGetModule<StatusEffectModule>(out var statusEffectModule))
            {
                var statusEffect = new StatusEffect(statusEffectDefinition);
                statusEffectModule.StatusEffectsController.AddStatusEffect(statusEffect);
            }
        }

        private void OnItemDefinitionClicked(ItemDefinition itemDefinition)
        {
            if (_playerContext.Player.TryGetModule<InventoryEquipmentModule>(out var ieModule))
            {
                ieModule.Inventory.Add(_itemStackAssembler.Create(itemDefinition.Id, 1).Snapshot);
            }
        }
    }
}
