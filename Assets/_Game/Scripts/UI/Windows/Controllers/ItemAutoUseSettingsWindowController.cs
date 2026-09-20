using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ItemUseSettingsWindowController : WindowControllerBase<ItemUseSettingsWindow, ItemUseSettingsWindowControllerArguments>
    {
        private ItemUseSettingsWindow _window;

        private readonly IPlayerProvider _playerProvider;
        private readonly EquipmentHudData _equipmentHudData;

        public ItemUseSettingsWindowController(
            IPlayerProvider playerProvider,
            EquipmentHudData equipmentHudData)
        {
            _playerProvider = playerProvider;
            _equipmentHudData = equipmentHudData;
        }

        public override void Initialize(ItemUseSettingsWindowControllerArguments arguments)
        {
            base.Initialize(arguments);

            _equipmentHudData.SetEntityId(arguments.EquipmentEntityId);
        }

        public override void Bind(ItemUseSettingsWindow window)
        {
            _window = window;
            _window.Changed += OnChanged;
            Redraw();
        }

        private void OnChanged(ItemUseSettings settings)
        {
            _playerProvider.Player.Publish(new ItemUseSettingsUpdateRequest(settings));
        }

        private void Redraw()
        {
            _window.Render(_equipmentHudData);
        }

        public override void Unbind()
        {
            _window.Changed -= OnChanged;
        }
    }

    public readonly struct ItemUseSettingsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public ItemUseSettingsWindowControllerArguments(IReadOnlyObservableData<string> equipmentEntityId)
        {
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
