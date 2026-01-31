using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class ItemContainerSaveRepository
    {
        private readonly ISaveStorage _storage;
        private readonly ISaveSerializer _serializer;

        public ItemContainerSaveRepository(ISaveStorage storage, ISaveSerializer serializer)
        {
            _storage = storage;
            _serializer = serializer;
        }

        public void SaveInventory(string key, InventorySave save) => _storage.SaveText(key, _serializer.Serialize(save));
        public InventorySave LoadInventory(string key) => _serializer.Deserialize<InventorySave>(_storage.LoadText(key));

        public void SaveEquipment(string key, EquipmentSave save) => _storage.SaveText(key, _serializer.Serialize(save));
        public EquipmentSave LoadEquipment(string key) => _serializer.Deserialize<EquipmentSave>(_storage.LoadText(key));
    }

    public class InventorySave
    {
        public (int, ItemStackSave)[] Items;
    }

    public class EquipmentSave
    {
        public (EquipmentSlotType, ItemStackSave)[] Items;
    }

    public class ItemStackSave
    {
        public string Id;
        public int Amount;
        public IItemInstanceData InstanceData;
    }
}
