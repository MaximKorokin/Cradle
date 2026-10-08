using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class EntityAiToggleWindow : UIWindowBase
    {
        [field: SerializeField]
        public EntityAiToggleView EntityAiToggleView { get; private set; }
    }
}
