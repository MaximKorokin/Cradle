using System;

namespace Assets._Game.Scripts.Items
{
    public class ItemStack
    {
        public ItemStack(ItemDefinition definition, IItemInstanceData instanceData, int amount)
        {
            Definition = definition;
            Instance = instanceData;
            Amount = amount;
        }

        private ItemDefinition _definition;
        public ItemDefinition Definition
        {
            get => _definition; set
            {
                if (_definition != value)
                {
                    _definition = value;
                    Changed?.Invoke();
                }
            }
        }

        private IItemInstanceData _instance;
        public IItemInstanceData Instance
        {
            get => _instance; set
            {
                if (_instance != value)
                {
                    _instance = value;
                    Changed?.Invoke();
                }
            }
        }

        private int _amount;
        public int Amount
        {
            get => _amount; set
            {
                if (_amount != value)
                {
                    _amount = value;
                    Changed?.Invoke();
                }
            }
        }

        public event Action Changed;
    }
}
