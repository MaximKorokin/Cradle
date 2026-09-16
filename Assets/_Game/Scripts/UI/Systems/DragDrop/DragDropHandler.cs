using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.Items.Commands;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public class DragDropHandler
    {
        private readonly IGlobalEventBus _globalEventBus;

        public DragDropHandler(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public void Handle(IDragDropSource source, IDragDropTarget target)
        {
            if (source is InventorySlotView inventorySlot1 && target is InventorySlotView inventorySlot2)
            {
                SLog.Log(inventorySlot1.SlotIndex, inventorySlot2.SlotIndex);

                PublishCommand(new TransferToContainerSlotCommand(
                    inventorySlot1.ContainerPath,
                    inventorySlot1.SlotIndex,
                    inventorySlot2.ContainerPath,
                    inventorySlot2.SlotIndex,
                    1));
            }
        }

        private void PublishCommand(IItemCommand command)
        {
            _globalEventBus.Publish(new ItemCommandRequest(command));
        }
    }
}
