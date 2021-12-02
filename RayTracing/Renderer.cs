using System.Drawing;

namespace RayTracing
{
    public class Renderer
    {
        private const int SLEEP_MS = 1000;

        private String outputLocation;
        private Int2D numberOfChunks;
        private World world;

        public Renderer(Int2D divideInChunks, World world, String saveLocation)
        {
            this.world = world;
            this.numberOfChunks = divideInChunks;
            this.outputLocation = saveLocation;
        }

        public void Render(bool useMultithreading)
        {
            bool completed = false;

            StartRender((img, startTime) =>
            {
                DateTime writeTimeStart = DateTime.Now;
                Log.InfoLine("Render Complete in {0}ms. Writing to disk. Location: {1}", (DateTime.Now - startTime).TotalMilliseconds, outputLocation);
                Bitmap bmp = new Bitmap(img.GetLength(0), img.GetLength(1));

                for (int i = 0; i < bmp.Width; i++)
                    for (int j = 0; j < bmp.Height; j++)
                        bmp.SetPixel(i, j, img[i, j]);

                bmp.Save(outputLocation);
                Log.InfoLine("Image written to disk in {0}ms", (DateTime.Now - writeTimeStart).TotalMilliseconds);
                completed = true;
            }, useMultithreading);

            while (!completed)
                Thread.Sleep(SLEEP_MS);

        }

        // DateTime refers to the time render was started.
        private void StartRender(System.Action<System.Drawing.Color[,], DateTime> onComplete, bool multiThreadded)
        {
            DateTime startTime = DateTime.Now;

            ChunkRenderer[,] chunkRenderes = new ChunkRenderer[numberOfChunks.x, numberOfChunks.y];

            Int2D resolution = world.GetMainCamera().Resolution;
            Int2D pixelsPerChunk = new Int2D(resolution.x / numberOfChunks.x, resolution.y / numberOfChunks.y);

            Camera cam = world.GetMainCamera();

            Log.InfoLine("Starting Rendering. Resolution: {0}x{1}, Bouces: {2}, RaysPerPixel: {3}, Multithreading: {4}",
                resolution.x, resolution.y, cam.BounceLimit, cam.RaysPerPixel.x * cam.RaysPerPixel.y, multiThreadded);

            System.Drawing.Color[,] renderedImage = new System.Drawing.Color[resolution.x, resolution.y];

            bool[,] completedList = new bool[numberOfChunks.x, numberOfChunks.y];

            for (int i = 0; i < numberOfChunks.x; i++)
            {
                for (int j = 0; j < numberOfChunks.y; j++)
                {
                    Int2D boundsStart = new Int2D(pixelsPerChunk.x * i, pixelsPerChunk.y * j);
                    Int2D boundsEnd = new Int2D(pixelsPerChunk.x * (i + 1), pixelsPerChunk.y * (j + 1));
                    chunkRenderes[i, j] = new ChunkRenderer(world, boundsStart, boundsEnd);

                    // The thread does not start immediately on thread.Start, but after some time. So it will start AFTER i, j have already changed
                    // Hence this becomes necessary.
                    int x = i;
                    int y = j;

                    ThreadStart workerThreadFunction = () =>
                    {
                        Log.InfoLine("Starting Chunk ({0}, {1}) Render", x, y);
                        chunkRenderes[x, y].Render((clrs, chunkStartTime, worldRendered, bs, be) =>
                        {
                            Log.InfoLine("Chunk ({0}, {1}) Render complete in {2} ms", x, y, (DateTime.Now - chunkStartTime).TotalMilliseconds);
                            completedList[x, y] = true;

                            for (int l = 0; l < clrs.GetLength(0); l++)
                                for (int m = 0; m < clrs.GetLength(1); m++)
                                    renderedImage[bs.x + l, bs.y + m] = clrs[l, m].ToARGB();

                            for (int l = 0; l < completedList.GetLength(0); l++)
                                for (int m = 0; m < completedList.GetLength(1); m++)
                                    if (!completedList[l, m])
                                        return;

                            onComplete(renderedImage, startTime);
                        });
                    };

                    if (multiThreadded)
                    {
                        Thread renderThread = new Thread(workerThreadFunction);
                        renderThread.Start();
                    }
                    else
                        workerThreadFunction();
                }
            }

        }
    }
}