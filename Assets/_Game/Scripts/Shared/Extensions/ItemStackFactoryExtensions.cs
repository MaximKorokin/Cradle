using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ItemStackFactoryExtensions
    {
        public static ItemStack CreateAndApply(this ItemStackFactory factory, string definitionId, ItemStackSave save)
        {
            var item = factory.Create(definitionId, save.Amount);
            factory.Apply(item, save);
            return item;
        }
    }
}
