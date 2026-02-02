using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ItemStackAssemblerExtensions
    {
        public static ItemStack CreateAndApply(this ItemStackAssembler assembler, string definitionId, ItemStackSave save)
        {
            var item = assembler.Create(definitionId, save.Amount);
            assembler.Apply(item, save);
            return item;
        }
    }
}
