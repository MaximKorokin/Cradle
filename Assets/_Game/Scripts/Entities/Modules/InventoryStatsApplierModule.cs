using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Items.Inventory;
using Assets.CoreScripts;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class InventoryStatsApplierModule : EntityModuleBase
    {
        private readonly StatsModule _stats;

        public InventoryStatsApplierModule(StatsModule stats)
        {
            _stats = stats;
        }

        protected override void OnAttach()
        {
            Subscribe<InventoryChangedEvent>(OnInventoryChanged);
        }

        private void OnInventoryChanged(InventoryChangedEvent e)
        {
            // pure int as source is dangerous
            var slotSource = ("Inventory", e.Slot);

            _stats.RemoveModifiers(slotSource);

            if (e.Kind != InventoryChangeKind.Removed && e.Item != null)
            {
                _stats.AddModifiers(slotSource, new StatModifier(StatId.CarryWeight, StatStage.Add, StatOperation.Add, e.Item.Value.Definition.Weight).Yield());
            }
        }
    }
}
