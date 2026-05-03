using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.DataFormatters;
using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers.ItemPreview
{
    public sealed class DefinitionItemStacksPreviewStrategy : IItemStacksPreviewStrategy
    {
        private readonly ItemDefinition _itemDefinition;
        private readonly ItemDefinitionFormatter _itemDefinitionFormatter;

        public DefinitionItemStacksPreviewStrategy(
            ItemDefinition itemDefinition,
            ItemDefinitionFormatter itemDefinitionFormatter)
        {
            _itemDefinition = itemDefinition;
            _itemDefinitionFormatter = itemDefinitionFormatter;
        }

        public void Initialize(ItemStacksPreviewWindow window)
        {
        }

        public void Cleanup(ItemStacksPreviewWindow window)
        {
        }

        public void Redraw(ItemStacksPreviewWindow window)
        {
            var displayData = _itemDefinitionFormatter.FormatData(_itemDefinition);
            window.Render(displayData, Array.Empty<ItemStackAction>());
        }

        public void ProcessAction(ItemStackActionType actionType)
        {
        }
    }
}
