using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public class GameSaveRepository
    {
        private readonly ISaveStorage _storage;
        private readonly ISaveSerializer _serializer;

        public GameSaveRepository(ISaveStorage storage, ISaveSerializer serializer)
        {
            _storage = storage;
            _serializer = serializer;
        }

        public void Save(string key, GameSave save) => _storage.SaveText(key, _serializer.Serialize(save));
        public GameSave Load(string key) => _serializer.Deserialize<GameSave>(_storage.LoadText(key));
    }

    public class GameSave
    {
        public EntitySave PlayerSave;
    }

    public class EntitySave
    {
        public string EntityId;
        public string DefinitionId;
        public InventorySave InventorySave;
        public EquipmentSave EquipmentSave;
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
        public string ItemDefinitionId;
        public int Amount;
        public IItemInstanceData InstanceData;
    }
}
