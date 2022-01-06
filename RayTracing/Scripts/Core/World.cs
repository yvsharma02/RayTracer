using System.Collections.Generic;

namespace RayTracing
{
    public class World
    {
        private List<Shape> shapes;
        private List<LightSource> lightSources;

        private Camera mainCamera;

        public int ShapesCount
        {
            get
            {
                return shapes.Count;
            }
        }

        public int LightSourcesCount
        {
            get
            {
                return lightSources.Count;
            }
        }
        public World(Camera mainCamera, GlobalLight globalLightSrc)
        {
            this.shapes = new List<Shape>();
            this.lightSources = new List<LightSource>();

            SetMainCamera(mainCamera);
        }

        public Shape ClosestShapeHit(Ray ray, out Vector3D pointOfContact)
        {
            float minDistSq = float.MaxValue;
            Shape closestHitShape = null;
            pointOfContact = new Vector3D();
            Vector3D poc = new Vector3D();

            for (int i = 0; i < shapes.Count; i++)
            {
                Shape currentHitSubshape;

                if (shapes[i].HitsRay(ray, out poc, out currentHitSubshape))
                {
                    float distSq = ray.Origin.DistanceFromSq(poc);
                    if (distSq < minDistSq)
                    {
                        minDistSq = distSq;
                        closestHitShape = (currentHitSubshape == null) ? shapes[i] : currentHitSubshape;
                        pointOfContact = poc;
                    }
                }
            }

            return closestHitShape;
        }

        public void AddShape(Shape s)
        {
            shapes.Add(s);
        }

        public void RemoveShape(Shape s)
        {
            shapes.Remove(s);
        }

        public void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }

        public void AddLightSource(LightSource source)
        {
            lightSources.Add(source);
        }

        public void RemoveLightSource(LightSource source)
        {
            lightSources.Remove(source);
        }

        public LightSource GetLightSource(int index)
        {
            return lightSources[index];
        }

        public Shape GetShape(int index)
        {
            return shapes[index];
        }

        public Camera GetMainCamera()
        {
            return mainCamera;
        }
    }
}