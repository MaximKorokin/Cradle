using UnityEngine;

namespace Assets._Game.Scripts.Shared.Utils
{
    public static class ScreenUtils
    {
        public static Vector2 ScreenCenter => new(Screen.width / 2f, Screen.height / 2f);

        public static Vector2 DefaultWindowPosition => new(Screen.width / 2, Screen.height - 100);
    }
}
