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

        public event Action<EntityAnimationClipName, AnimationClip> SetAnimationRequested;
        public event Action<bool> SetDirectionRequested;
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

        public void RequestSetAnimation(EntityAnimationClipName clipName, AnimationClip clip)
        {
            SetAnimationRequested?.Invoke(clipName, clip);
        }

        public void RequestUpdateOrderInLayer()
        {
            UpdateOrderInLayerRequested?.Invoke();
        }

        public void RequestSetDirection(bool right)
        {
            SetDirectionRequested?.Invoke(right);
        }
    }
}
