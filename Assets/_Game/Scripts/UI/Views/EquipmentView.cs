using Assets._Game.Scripts.Items.Equipment;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class EquipmentView : MonoBehaviour
    {
        [SerializeField]
        private EquipmentSlotView[] _slots;

        public void Render(EquipmentModel equipmentModel)
        {
            foreach (var (index, stack) in equipmentModel.Enumerate())
            {
                var slotView = System.Array.Find(_slots, s => s.SlotType == index);
                if (slotView != null)
                {
                    slotView.Render(stack);
                }
            }
        }
    }
}
