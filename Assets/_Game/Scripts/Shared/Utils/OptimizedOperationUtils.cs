namespace Assets._Game.Scripts.Shared.Utils
{
    public static class OptimizedOperationUtils
    {
        public static void CleanTillNull<T>(T[] buffer) where T : class
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == null) return;
                buffer[i] = null;
            }
        }

        public static int CollectUnique<T>(T[] source, T[] buffer) where T : class
        {
            int count = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == null) break;

                bool exists = false;
                for (int j = 0; j < count; j++)
                {
                    if (buffer[j].Equals(source[i]))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    buffer[count++] = source[i];
                }
            }
            return count;
        }
    }
}
