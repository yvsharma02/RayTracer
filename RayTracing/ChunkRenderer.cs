namespace RayTracing
{
    public class ChunkRenderer
    {
        private World world;

        private Int2D pixelBoundsStart;
        private Int2D pixelBoundsEnd;

        public World WorldToRender { get => world; }

        public ChunkRenderer(World world, Int2D pbs, Int2D pbe)
        {
            this.world = world;
            this.pixelBoundsStart = pbs;
            this.pixelBoundsEnd = pbe;
        }

        public void SetPixelBounds(Int2D pbs, Int2D pbe)
        {
            Int2D worldStart = new Int2D(0, 0);
            Int2D worldEnd = world.GetMainCamera().Resolution;

            if (pbs.x < worldStart.x || pbs.y < worldStart.y || pbs.x > worldEnd.x || pbs.y > worldEnd.y)
                throw new ArgumentOutOfRangeException("Pixel Start Index");
            else if (pbe.x < worldStart.x || pbe.y < worldStart.y || pbe.x > worldEnd.x || pbe.y > worldEnd.y)
                throw new ArgumentOutOfRangeException("Pixel End Index");

            if (pbe.x < pbs.x || pbe.y < pbs.y)
                throw new ArgumentException("Pixel Bounds");

            this.pixelBoundsStart = pbe;
            this.pixelBoundsEnd = pbe;
        }

        // Ouput, Time render was started, World to render, pixelStartIndex, PixelEndIndex
        public void Render(Action<RTColor[,], DateTime, World, Int2D, Int2D> onComplete)
        {
            DateTime startTime = DateTime.Now;

            int sizeX = pixelBoundsEnd.x - pixelBoundsStart.x;
            int sizeY = pixelBoundsEnd.y - pixelBoundsStart.y;
            RTColor[,] colors = new RTColor[sizeX, sizeY];

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    Camera cam = world.GetMainCamera();
                    Int2D rpp = cam.RaysPerPixel;
                    RTColor[,] currentPixelColors = new RTColor[rpp.x, rpp.y];

                    for (int x = 0; x < rpp.x; x++)
                        for (int y = 0; y < rpp.y; y++)
                            currentPixelColors[x, y] = StartTrace(cam.PixelIndexToRay(new Int2D(pixelBoundsStart.x + i, pixelBoundsStart.y + j), new Int2D(x, y)), world, cam);

                    colors[i, j] = CalculateFinalColor(currentPixelColors);
                }
            }
            onComplete(colors, startTime, world, pixelBoundsStart, pixelBoundsEnd);
        }

        protected virtual RTColor CalculateFinalColor(RTColor[,] rawClrs)
        {
            double I = 0f;
            float R = 0f;
            float G = 0f;
            float B = 0f;

            for (int i = 0; i < rawClrs.GetLength(0); i++)
            {
                for (int j = 0; j < rawClrs.GetLength(1); j++)
                {
                    float divider = rawClrs.GetLength(0) * rawClrs.GetLength(1);

                    I += rawClrs[i, j].Intensity / divider;
                    R += rawClrs[i, j].R / divider;
                    G += rawClrs[i, j].G / divider;
                    B += rawClrs[i, j].B / divider;
                }
            }

            return new RTColor(I, R, G, B);
        }

        private RTColor StartTrace(Ray reverseRay, World world, Camera camera)
        {
            return TraceAdvance(world, camera, camera.BounceLimit, reverseRay, out Vector3D _);
        }

        private RTColor TraceAdvance(World world, Camera renderCamera, int bouncesRemaining, Ray originalReverseRay, out Vector3D pointOfContact)
        {
            Vector3D closestShapePOC = new Vector3D();
            Shape closestShape = world.ClosestShapeHit(originalReverseRay, out closestShapePOC);
            
            if (closestShape != null)
            {
                Vector3D normal = closestShape.CalculateNormal(closestShapePOC);
                pointOfContact = closestShapePOC + (normal * Vector3D.EPSILON);
                ColoredRay[][] hittingRays = null;// = new ColoredRay[world.LightSourcesCount + (bouncesRemaining >= 0 ? 1 : 0)][];
                int c = 0;

                if (bouncesRemaining >= 0)
                {
                    Ray[] reverseEmmitedRays = closestShape.Shader.ReverseEmmitedRays(closestShape, originalReverseRay, closestShapePOC);
                    if (reverseEmmitedRays != null)
                    {
                        hittingRays = new ColoredRay[world.LightSourcesCount + reverseEmmitedRays.Length][];
                        for (int i = 0; i < reverseEmmitedRays.Length; i++)
                        {
                            Vector3D actualSrc;
                            RTColor reflectedRayColor = TraceAdvance(world, renderCamera, bouncesRemaining - 1, new Ray(pointOfContact, reverseEmmitedRays[i].Direction), out actualSrc);
                            hittingRays[c++] = new ColoredRay[] { new ColoredRay(actualSrc, reverseEmmitedRays[i].Direction * -1f, reflectedRayColor, pointOfContact) };
                        }
                    }
                }

                if (hittingRays == null)
                    hittingRays = new ColoredRay[world.LightSourcesCount][];

                for (int i = 0; i < world.LightSourcesCount; i++)
                    hittingRays[c++] = world.GetLightSource(i).ReachingRays(world, pointOfContact);

                return closestShape.Shader.CalculateBounceColor(closestShape, hittingRays, pointOfContact, originalReverseRay.Direction * -1f);
            }
            else
            {
                pointOfContact = originalReverseRay.Origin + originalReverseRay.Direction * float.PositiveInfinity;
                return renderCamera.NoHitColor;
            }
        }
    }
}