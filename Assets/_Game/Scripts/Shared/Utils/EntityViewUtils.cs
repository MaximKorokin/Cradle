using Assets._Game.Scripts.Entities;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Utils
{
    public sealed class EntityViewUtils
    {
        public static Bounds GetRenderersBounds(EntityView entityView)
        {
            var renderers = entityView.UnitsController.Units.Select(u => u.SpriteRenderer);

            Bounds? bounds = null;

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                    continue;

                if (bounds == null)
                {
                    bounds = renderer.bounds;
                }
                else
                {
                    var b = bounds.Value;
                    b.Encapsulate(renderer.bounds);
                    bounds = b;
                }
            }

            return bounds.GetValueOrDefault();
        }
    }
}
