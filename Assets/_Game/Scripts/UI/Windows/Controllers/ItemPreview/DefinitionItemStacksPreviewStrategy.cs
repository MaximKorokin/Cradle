using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataFormatters;
using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class DefinitionItemStacksPreviewStrategy : IItemStacksPreviewStrategy
    {
        private readonly ItemDefinition _itemDefinition;
        private readonly ItemDefinitionFormatter _itemDefinitionFormatter;
        private readonly IPlayerProvider _playerProvider;
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;
        private readonly EquipmentSlotKey? _equipmentSlot;

        private EquipmentModel _equipmentModel;
        private ItemStacksPreviewWindow _window;

        public DefinitionItemStacksPreviewStrategy(
            ItemDefinition itemDefinition,
            ItemDefinitionFormatter itemDefinitionFormatter,
            IPlayerProvider playerProvider,
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter,
            EquipmentSlotKey? equipmentSlot)
        {
            _itemDefinition = itemDefinition;
            _itemDefinitionFormatter = itemDefinitionFormatter;
            _playerProvider = playerProvider;
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
            _equipmentSlot = equipmentSlot;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
            _window = window;
            _equipmentModel = _itemContainerResolver.ResolveEquipment(_playerProvider.Player);
            _equipmentModel.Changed += OnEquipmentChanged;
        }

        public void Cleanup(ItemStacksPreviewWindow window)
        {
            _equipmentModel.Changed -= OnEquipmentChanged;
        }

        private void OnEquipmentChanged()
        {
            Redraw(_window);
        }

        public void Redraw(ItemStacksPreviewWindow window)
        {
            var displayData = _itemDefinitionFormatter.FormatData(_itemDefinition);

            var equippedItem = _equipmentSlot != null ? _equipmentModel.Get(_equipmentSlot.Value) : null;

            if (equippedItem.HasValue)
                window.Render(displayData, _itemStackFormatter.FormatData(equippedItem.Value), Array.Empty<ItemStackAction>());
            else
                window.Render(displayData, Array.Empty<ItemStackAction>());
        }

        public void ProcessAction(ItemStackActionType actionType)
        {
        }
    }
}
