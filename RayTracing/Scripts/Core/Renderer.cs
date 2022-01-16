using System.Drawing;

namespace RayTracing
{
    public static class Renderer
    {
        private static void SaveToDisk(Color[,] image, string location)
        {
            Log.InfoLine("Writing to disk: {0}", location);
            DateTime startTime = DateTime.Now;

            Bitmap bmp = new Bitmap(image.GetLength(0), image.GetLength(1));

            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    bmp.SetPixel(i, j, image[i, j]);

            bmp.Save(location);

            Log.InfoLine("Written to disk in : {0} ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        public static void RenderAndWriteToDisk(World world, Camera camera, Int2D chunks, bool multiThreaded, string location)
        {
            Color[,] image = Render(world, camera, chunks, multiThreaded);
            SaveToDisk(image, location);
        }

        public static void StartRenderAndWriteToDisk(World world, Camera camera, Int2D chunks, bool multiThreaded, string location)
        {
            StartRender(world, camera, chunks, multiThreaded, (img) => SaveToDisk(img, location));
        }

        public static Color[,] Render(World world, Camera camera, Int2D chunks, bool multiThreaded)
        {
            if (world == null)
                throw new NullReferenceException();

            if (camera == null)
                throw new NullReferenceException();

            Color[,] image = new Color[camera.Resolution.x, camera.Resolution.y];

            if ((camera.Resolution.x % chunks.x != 0) && (camera.Resolution.y % chunks.y != 0))
                throw new ArgumentException("Chunks should be a divisor or resolution");

            Log.InfoLine("Starting Render:\nResolution: {0}\n{1}", camera.Resolution, multiThreaded ? "Multi-Threaded" : "Single Threaded");

            Int2D pixelsPerChunk = new Int2D(camera.Resolution.x / chunks.x, camera.Resolution.y / chunks.y);
            DateTime startTime = DateTime.Now;

            if (multiThreaded)
            {
                int completeCount = 0;

                DateTime[] startingTimes = new DateTime[chunks.x * chunks.y];
                Task[] tasks = new Task[chunks.x * chunks.y];

                for (int i = 0; i < chunks.x; i++)
                {
                    for (int j = 0; j < chunks.y; j++)
                    {
                        Int2D chunkStart = new Int2D(i * pixelsPerChunk.x, j * pixelsPerChunk.y);

                        int index = chunks.x * i + j;
                        startingTimes[index] = DateTime.Now;

                        Log.InfoLine("Starting Chunk {0};", index);
                        Task task = RayTracer.StartRender(image, world, camera, chunkStart, pixelsPerChunk, true);
                        tasks[index] = task;

                        task.ContinueWith((t) =>
                        {
                            completeCount += 1;
                            Log.InfoLine("Completed Chunk {0} in {1} ms; {2}/{3} Done;", index, (DateTime.Now - startingTimes[index]).TotalMilliseconds, completeCount, chunks.x * chunks.y);
                        });
                    }
                }

                Task.WaitAll(tasks);

            }
            else
            {
                int completeCount = 0;
                for (int i = 0; i < chunks.x; i++)
                {
                    for (int j = 0; j < chunks.y; j++)
                    {
                        int index = i * chunks.x + j;
                        Log.InfoLine("Starting Chunk {0};", i * chunks.x + j);
                        DateTime chunkStartTime = DateTime.Now;

                        Int2D chunkStart = new Int2D(i * pixelsPerChunk.x, j * pixelsPerChunk.y);
                        RayTracer.Render(image, world, camera, chunkStart, pixelsPerChunk);

                        completeCount += 1;
                        Log.InfoLine("Completed Chunk {0} in {1} ms; {2}/{3} Done;", index, (DateTime.Now - chunkStartTime).TotalMilliseconds, completeCount, chunks.x * chunks.y);
                    }
                }
            }

            Log.InfoLine("Render complete in: {0} ms", (DateTime.Now - startTime).TotalMilliseconds);
            return image;
        }

        public static void StartRender(World world, Camera camera, Int2D chunks, bool multithreaded, Action<Color[,]> onComplete)
        {
            Color[,] image = null;
            Task task = new Task(() => image = Render(world, camera, chunks, multithreaded));
            onComplete(image);
        }
    }
}