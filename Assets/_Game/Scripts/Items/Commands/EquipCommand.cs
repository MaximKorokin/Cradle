namespace Assets._Game.Scripts.Items.Commands
{
    public readonly struct EquipCommand : IItemCommand
    {
        public readonly ItemDefinition ItemDefinition;

        public EquipCommand(ItemDefinition itemDefinition)
        {
            ItemDefinition = itemDefinition;
        }
    }
}
