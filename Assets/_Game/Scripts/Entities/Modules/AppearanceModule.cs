using Assets._Game.Scripts.Entities.Units;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AppearanceModule : EntityModuleBase
    {
        public event Action<string, int> EnsureUnitRequested;
        public event Action<string, Sprite> SetUnitSpriteRequested;
        public event Action<string> RemoveUnitRequested;

        public event Action<EntityAnimatorParameterName, ValueType> SetAnimatorValueRequested;
        public event Action<EntityAnimationClipName, AnimationClip> SetAnimationRequested;
        public event Action<TurnDirection> SetTurnDirectionRequested;
        public event Action UpdateOrderInLayerRequested;

        public EntityVisualModel EntityVisualModel { get; private set; }

        public AppearanceModule(EntityVisualModel entityVisualModel)
        {
            EntityVisualModel = entityVisualModel;
        }

        public void RequestEnsureUnit(string path, int relativeOrderInLayer)
        {
            EnsureUnitRequested?.Invoke(path, relativeOrderInLayer);
        }

        public void RequestSetUnitSprite(string path, Sprite sprite)
        {
            SetUnitSpriteRequested?.Invoke(path, sprite);
        }

        public void RequestRemoveUnit(string path)
        {
            RemoveUnitRequested?.Invoke(path);
        }

        public void RequestSetAnimatorValue(EntityAnimatorParameterName key, ValueType value)
        {
            SetAnimatorValueRequested?.Invoke(key, value);
        }

        public void RequestSetAnimation(EntityAnimationClipName clipName, AnimationClip clip)
        {
            SetAnimationRequested?.Invoke(clipName, clip);
        }

        public void RequestUpdateOrderInLayer()
        {
            UpdateOrderInLayerRequested?.Invoke();
        }

        public void RequestSetTurnDirection(TurnDirection turnDirection)
        {
            SetTurnDirectionRequested?.Invoke(turnDirection);
        }
    }
}
