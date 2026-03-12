using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(menuName = "Configs/EntityUnitConfig")]
    public sealed class EntityUnitConfig : ScriptableObject
    {
        [field: SerializeField]
        public Sprite NotFoundVariantSprite { get; private set; }
    }
}
