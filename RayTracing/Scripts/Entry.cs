namespace RayTracing
{
    public class Entry
    {
        private const string LOG_FILE_PATH = @"D:\Projects\VisualStudio\RayTracing\Generated\Log.txt";

        public static void Main(String[] args)
        {
            Log.Initialise(LOG_FILE_PATH, true, true, true, true);

            TestScene scene = new TestScene();
        }
    }
}