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
        public void Render(Action<System.Drawing.Color[,], DateTime, World, Int2D, Int2D> onComplete)
        {
            DateTime startTime = DateTime.Now;

            int sizeX = pixelBoundsEnd.x - pixelBoundsStart.x;
            int sizeY = pixelBoundsEnd.y - pixelBoundsStart.y;
            System.Drawing.Color[,] colors = new System.Drawing.Color[sizeX, sizeY];

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    Camera cam = world.GetMainCamera();
                    Ray[] emmitedRays = cam.Shader.GetEmmitedRays(cam, new Int2D(pixelBoundsStart.x + i, pixelBoundsStart.y + j));

                    RTColor[] subPixelColors = new RTColor[emmitedRays.Length];

                    for (int k = 0; k < subPixelColors.Length; k++)
                        subPixelColors[k] = StartTrace(emmitedRays[k], world, cam);

                    colors[i, j] = cam.Shader.CalculateFinalPixelColor(cam, subPixelColors);
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
            return Trace(world, camera, camera.BounceLimit, reverseRay, out Vector3D _);
        }

        private RTColor Trace(World world, Camera renderCamera, int bouncesRemaining, Ray tracingRay, out Vector3D pointOfContact)
        {
            Vector3D truePOC = default(Vector3D);
            Shape closestShape = world.ClosestShapeHit(tracingRay, out truePOC);
            
            if (closestShape != null)
            {
                Vector3D normal = closestShape.Shader.CalculateNormal(closestShape, truePOC);
                pointOfContact = truePOC + (normal * Vector3D.EPSILON);

                int c = 0;
                ColoredRay[][] incomingRays = null;

                if (bouncesRemaining > 0)
                {
                    Ray[] outgoingRays = closestShape.Shader.GetOutgoingRays(closestShape, tracingRay, truePOC);
                    if (outgoingRays != null)
                    {
                        incomingRays = new ColoredRay[world.LightSourcesCount + outgoingRays.Length][];
                        for (int i = 0; i < outgoingRays.Length; i++)
                        {
                            Vector3D incomingRaySource;
                            RTColor reflectedRayColor = Trace(world, renderCamera, bouncesRemaining - 1, new Ray(pointOfContact, outgoingRays[i].Direction), out incomingRaySource);
                            incomingRays[c++] = new ColoredRay[] { new ColoredRay(incomingRaySource, outgoingRays[i].Direction * -1f, reflectedRayColor, pointOfContact) };
                        }
                    }
                }

                if (incomingRays == null)
                    incomingRays = new ColoredRay[world.LightSourcesCount][];

                for (int i = 0; i < world.LightSourcesCount; i++)
                    incomingRays[c++] = world.GetLightSource(i).ReachingRays(world, pointOfContact);

                return closestShape.Shader.CalculateBounceColor(closestShape, incomingRays, truePOC, tracingRay.Direction * -1f);
            }
            else
            {
                pointOfContact = tracingRay.Origin + tracingRay.Direction * float.PositiveInfinity;
                return renderCamera.NoHitColor;
            }
        }
    }
}