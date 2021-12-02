using System.Collections.Generic;

namespace RayTracing
{
    public class World
    {
        private List<WorldObject> worldObjects;

        private LightSource globalLightSource;

        private Vector3D globalLightDirection;
        private RTColor globalLightColor;

        private Camera mainCamera;

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
            SetMainCamera(mainCamera);
            SetGlobalLightSource(globalLightSrc);

            for (int i = 0; i < worldObjectsToAdd.Length; i++)
                worldObjects.Add(worldObjectsToAdd[i]);
        }

        public void AddShape(Shape s)
        {
            worldObjects.Add(s);
        }

        public void RemoveShape(Shape s)
        {
            worldObjects.Remove(s);
        }

        public void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }

        public void AddLightSource(LightSource source)
        {
            worldObjects.Add(source);
        }

        public void Remove(LightSource source)
        {
            worldObjects.Remove(source);
        }

        public WorldObject GetWorlObject(int index)
        {
            return worldObjects[index];
        }

        public LightSource GetGlobalLightSource()
        {
            return globalLightSource;
        }

        public void SetGlobalLightSource(LightSource src)
        {
            globalLightSource = src;
        }

        public Camera GetMainCamera()
        {
            return mainCamera;
        }
    }
}