using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class TransformExtensions
    {
        public static void AddChild(this Transform transform, Transform other, string path)
        {
            var pathParts = path.Split('/');
            if (pathParts[^1] == other.name)
            {
                pathParts = pathParts.Take(pathParts.Length - 1).ToArray();
            }

            var parent = transform.Find(string.Join('/', pathParts));
            if (parent == null) parent = transform;

            other.parent = parent;
        }
    }
}