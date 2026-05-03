using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Items.Equipment;
using System.Collections.Generic;

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

        public void Save(string key, GameSave save)
        {
            var json = _serializer.Serialize(save);
            _storage.SaveText(key, json);
        }

        public GameSave Load(string key)
        {
            var json = _storage.LoadText(key);
            return _serializer.Deserialize<GameSave>(json);
        }
    }

    public sealed class GameSave
    {
        public int Version;
        public long SavedAtUtc;
        public EntitySave PlayerSave;
        public LocationSave PlayerLocationSave;
    }

    public sealed class LocationSave
    {
        public string LocationId;
        public string EntranceId;
        public float PositionX;
        public float PositionY;
    }

    public sealed class EntitySave
    {
        public string DefinitionId;
        public InventorySave StorageSave;
        public InventorySave InventorySave;
        public EquipmentSave EquipmentSave;
        public LevelingSave LevelingSave;
    }

    public sealed class LevelingSave
    {
        public int Level;
        public long Experience;
    }

    public sealed class StatBaseSave
    {
        public StatId Id;
        public float Value;
    }

    public sealed class InventorySlotSave
    {
        public int Slot;
        public ItemStackSave Stack;
    }

    public sealed class EquipmentSlotSave
    {
        public EquipmentSlotType Type;
        public int Index;
        public ItemStackSave Stack;
    }

    public sealed class InventorySave
    {
        public InventorySlotSave[] Items;
    }

    public sealed class EquipmentSave
    {
        public EquipmentSlotSave[] Items;
        public ItemUseSettings AutoItemUseSettings;
    }

    public class ItemStackSave
    {
        public string ItemDefinitionId;
        public int Amount;
        public EncodedSaveData InstanceData;
    }

    public class EncodedSaveData
    {
        public string Type;
        public string Json;
    }
}
