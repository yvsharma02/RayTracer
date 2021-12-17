namespace RayTracing
{
    public static class ExtensionMethods
    {
        public static T[] Clone<T>(this T[] original)
        {
            T[] cloned = new T[original.Length];

            for (int i = 0; i < original.Length; i++)
                cloned[i] = original[i];

            return cloned;
        }
    }
}
