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

        public void EnsureUnit(string path, int relativeOrderInLayer)
        {
            EnsureUnitRequested?.Invoke(path, relativeOrderInLayer);
        }

        public void SetUnitSprite(string path, Sprite sprite)
        {
            SetUnitSpriteRequested?.Invoke(path, sprite);
        }

        public void RemoveUnit(string path)
        {
            RemoveUnitRequested?.Invoke(path);
        }

        public void SetAnimation(EntityAnimationClipName clipName, AnimationClip clip)
        {
            SetAnimationRequested?.Invoke(clipName, clip);
        }

        public void UpdateOrderInLayer()
        {
            UpdateOrderInLayerRequested?.Invoke();
        }

        public void SetDirection(bool right)
        {
            SetDirectionRequested?.Invoke(right);
        }
    }
}
