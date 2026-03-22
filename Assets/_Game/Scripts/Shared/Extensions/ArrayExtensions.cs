namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class ArrayExtensions
    {
        public static T GetRandomElement<T>(this T[] array, T defaultValue = default)
        {
            if (array == null || array.Length == 0)
                return defaultValue;
            var randomIndex = UnityEngine.Random.Range(0, array.Length);
            return array[randomIndex];
        }
    }
}
