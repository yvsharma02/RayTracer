using System.Collections.Generic;

namespace RayTracing
{
    public class World
    {
        private List<Shape> shapes;
        private List<LightSource> lightSources;

        private List<WorldObject> worldObjects;

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

        public int ObjectCount
        {
            get
            {
                return worldObjects.Count;
            }
        }

        public World(Camera mainCamera, GlobalLight globalLightSrc, params WorldObject[] worldObjectsToAdd)
        {
            this.worldObjects = new List<WorldObject>();
            this.shapes = new List<Shape>();
            this.lightSources = new List<LightSource>();

            SetMainCamera(mainCamera);

            for (int i = 0; i < worldObjectsToAdd.Length; i++)
                worldObjects.Add(worldObjectsToAdd[i]);
        }

        public Shape ClosestShapeHit(Ray ray, out Vector3D pointOfContact)
        {
            float minDist = float.MaxValue;
            Shape closestHitShape = null;
            pointOfContact = new Vector3D();
            Vector3D poc = new Vector3D();

            for (int i = 0; i < shapes.Count; i++)
            {
                WorldObject currentHitSubshape;

                if (shapes[i].HitsRay(ray, out poc, out currentHitSubshape))
                {
                    float dist = ray.Origin.DistanceFrom(poc);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestHitShape = (Shape) ((currentHitSubshape == null) ? shapes[i] : currentHitSubshape);
                        pointOfContact = poc;
                    }
                }
            }

            return closestHitShape;
        }

        public void AddShape(Shape s)
        {
            shapes.Add(s);
            worldObjects.Add(s);
        }

        public void RemoveShape(Shape s)
        {
            shapes.Remove(s);
            worldObjects.Remove(s);
        }

        public void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }

        public void AddLightSource(LightSource source)
        {
            lightSources.Add(source);
            worldObjects.Add(source);
        }

        public void RemoveLightSource(LightSource source)
        {
            lightSources.Remove(source);
            worldObjects.Remove(source);
        }

        public LightSource GetLightSource(int index)
        {
            return lightSources[index];
        }

        public Shape GetShape(int index)
        {
            return shapes[index];
        }

        public WorldObject GetWorlObject(int index)
        {
            return worldObjects[index];
        }

        public Camera GetMainCamera()
        {
            return mainCamera;
        }
    }
}