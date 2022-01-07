using System.Drawing;

namespace RayTracing
{
    public static class RayTracer
    {
        public static Task StartRender(Color[,] image, World world, Camera camera, Int2D startingPixelIndex, Int2D count, bool startImmediately)
        {
            Task task = new Task(() => Render(image, world, camera, startingPixelIndex, count));

            if (startImmediately)
                task.Start();

            return task;
        }

        public static void Render(Color[,] image, World world, Camera camera, Int2D startingPixelIndex, Int2D pixelCount)
        {
            if (world == null)
                throw new NullReferenceException();

            if (camera == null)
                throw new NullReferenceException();

            Int2D res = camera.Resolution;

            if (startingPixelIndex.x < 0 || startingPixelIndex.y < 0)
                throw new IndexOutOfRangeException();

            if (startingPixelIndex.x + pixelCount.x > res.x || startingPixelIndex.y + pixelCount.y > res.y)
                throw new IndexOutOfRangeException();

            for (int i = 0; i < pixelCount.x; i++)
            {
                for (int j = 0; j < pixelCount.y; j++)
                {
                    Camera cam = world.GetMainCamera();
                    Ray[] emmitedRays = cam.Shader.GetEmmitedRays(cam, new Int2D(startingPixelIndex.x + i, startingPixelIndex.y + j));

                    RTColor[] subPixelColors = new RTColor[emmitedRays.Length];

                    for (int k = 0; k < subPixelColors.Length; k++)
                        subPixelColors[k] = StartTrace(emmitedRays[k], world, cam);

                    image[startingPixelIndex.x + i, startingPixelIndex.y + j] = cam.Shader.CalculateFinalPixelColor(cam, new Int2D(startingPixelIndex.x + i, startingPixelIndex.y + j), subPixelColors);
                }
            }
        }

        private static RTColor StartTrace(Ray reverseRay, World world, Camera camera)
        {
            return Trace(world, camera, camera.BounceLimit, reverseRay, out Vector3D _);
        }

        private static RTColor Trace(World world, Camera renderCamera, int bouncesRemaining, Ray tracingRay, out Vector3D pointOfContact)
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