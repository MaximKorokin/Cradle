using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ItemUseSettingsWindowController : WindowControllerBase<ItemUseSettingsWindow, ItemUseSettingsWindowControllerArguments>
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly EquipmentViewData _equipmentViewData;

        public ItemUseSettingsWindowController(
            IPlayerProvider playerProvider,
            EquipmentViewData equipmentViewData)
        {
            _playerProvider = playerProvider;
            _equipmentViewData = equipmentViewData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _equipmentViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override void OnBind()
        {
            base.OnBind();

            Window.Changed += OnChanged;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            Window.Changed -= OnChanged;
        }

        private void OnChanged(ItemUseSettings settings)
        {
            _playerProvider.Player.Publish(new ItemUseSettingsUpdateRequest(settings));
        }

        protected override void Redraw()
        {
            Window.Render(_equipmentViewData);
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
