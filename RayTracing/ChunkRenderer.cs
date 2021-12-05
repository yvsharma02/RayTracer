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
        public void Render(bool advanceTrace, Action<RTColor[,], DateTime, World, Int2D, Int2D> onComplete)
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
                            currentPixelColors[x, y] = StartTrace(cam.PixelIndexToRay(new Int2D(pixelBoundsStart.x +i, pixelBoundsStart.y + j), new Int2D(x, y)), world, cam, advanceTrace);

                    colors[i, j] = CalculateFinalColor(currentPixelColors);
                }
            }

            onComplete(colors, startTime, world, pixelBoundsStart, pixelBoundsEnd);
        }

        protected virtual RTColor CalculateFinalColor(RTColor[,] rawClrs)
        {
            float I = 0f;
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

        private Vector3D CalculateReflectedRayDirection(Vector3D incidentRay, Vector3D normal)
        {
            normal = normal.Normalize();
            incidentRay = incidentRay.Normalize();

            return incidentRay - normal * 2f * Vector3D.Dot(incidentRay, normal);
        }

        private RTColor StartTrace(Ray reverseRay, World world, Camera camera, bool advanceTrace)
        {
            if (!advanceTrace)
                return Trace(reverseRay.Origin, reverseRay.Direction, world, camera.BounceLimit, out Vector3D _, camera.NoHitColor, camera.BounceLimitColor);
            else
                return TraceAdvance(world, camera, camera.BounceLimit, reverseRay, out Vector3D _);
        }

        private RTColor TraceAdvance(World world, Camera renderCamera, int bouncesRemaining, Ray originalReverseRay, out Vector3D pointOfContact)
        {
            if (bouncesRemaining < 0)
            {
                pointOfContact = originalReverseRay.Origin + originalReverseRay.Direction * float.PositiveInfinity;
                return renderCamera.BounceLimitColor;
            }

            Vector3D closestShapePOC = new Vector3D();
            Shape closestShape = world.ClosestShapeHit(originalReverseRay, out closestShapePOC);
            
            if (closestShape != null)
            {
                Vector3D normal = closestShape.CalculateNormal(closestShapePOC);
                Vector3D reflectedRayDir = CalculateReflectedRayDirection(originalReverseRay.Direction, normal);
                pointOfContact = closestShapePOC + (normal * Vector3D.EPSILON);

                RTRay[][] hittingRays = new RTRay[world.LightSourcesCount + 1][];

                Vector3D actualSrc;
                RTColor reflectedRayClr = TraceAdvance(world, renderCamera, bouncesRemaining - 1, new Ray(pointOfContact, reflectedRayDir), out actualSrc);

                for (int i = 0; i < world.LightSourcesCount; i++)
                    hittingRays[i] = world.GetLightSource(i).ReachingRays(world, pointOfContact);

                hittingRays[world.LightSourcesCount] = new RTRay[] { new RTRay(actualSrc, reflectedRayDir * -1f, reflectedRayClr, pointOfContact) };

                return closestShape.CalculateBouncedRayColor(hittingRays, originalReverseRay.Direction * -1f);
            }
            else
            {
                pointOfContact = originalReverseRay.Origin + originalReverseRay.Direction * float.PositiveInfinity;
                return renderCamera.NoHitColor;
            }
        }

        private RTColor Trace(Vector3D reverseRayOrigin, Vector3D reverseRayDirection, World world, int bouncesRemaining, out Vector3D actualRayOrigin, RTColor noHitColor, RTColor bounceLimitColor)
        {
            if (bouncesRemaining < 0)
            {
                actualRayOrigin = reverseRayOrigin + reverseRayDirection * float.PositiveInfinity;
                return bounceLimitColor;
            }

            reverseRayDirection = reverseRayDirection.Normalize();

            Ray reverseRay = new Ray(reverseRayOrigin, reverseRayDirection);

            RTColor resultantColor = RTColor.Black;

            WorldObject closestHitObject = null;

            Vector3D closestHitObjPOC = new Vector3D();
            float minDistance = float.PositiveInfinity;

            for (int i = 0; i < world.ObjectCount; i++)
            {
                WorldObject obj = world.GetWorlObject(i);

                Vector3D poc;
                
                if (obj.HitsRay(reverseRay, out poc))
                {
                    float dist = poc.DistanceFrom(reverseRayOrigin);

                    if (dist < minDistance)
                    {
                        closestHitObject = obj;
                        closestHitObjPOC = poc;
                        minDistance = dist;
                    }
                }
            }

            if (closestHitObject != null)
            {
                actualRayOrigin = closestHitObjPOC;

                if (!closestHitObject.IsLightSource)
                {
                    Shape shape = (Shape)closestHitObject;

                    Vector3D normal = shape.CalculateNormal(closestHitObjPOC);
                    Vector3D reflectedRayDir = CalculateReflectedRayDirection(reverseRay.Direction, normal);

                    Vector3D actualRaySrc;
                    RTColor incidentClr = Trace(closestHitObjPOC, reflectedRayDir, world, bouncesRemaining - 1, out actualRaySrc, noHitColor, bounceLimitColor);

                    RTRay incidentRay = new RTRay(actualRaySrc, reflectedRayDir * -1f, incidentClr, closestHitObjPOC);

                    resultantColor = shape.CalculateBouncedRayColor(new RTRay[][] { new RTRay[] { incidentRay } }, reflectedRayDir);
                }
                else
                {
                    LightSource ls = (LightSource)closestHitObject;
                    resultantColor = ls.LightColor * ls.CalculateMultiplier(reverseRay);
                }
            }
            else
            {
                LightSource ls = world.GetGlobalLightSource();

                if (ls != null)
                {
                    resultantColor = ls.LightColor * ls.CalculateMultiplier(reverseRay);
                    actualRayOrigin = (closestHitObjPOC - ls.Position) * float.PositiveInfinity;
                }
                else
                {
                    resultantColor = noHitColor;
                    actualRayOrigin = reverseRayOrigin + reverseRayDirection * float.PositiveInfinity;
                }
            }

            return resultantColor;
        }
    }
}