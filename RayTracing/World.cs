using System.Collections.Generic;

namespace RayTracing
{
    public class World
    {
        private List<LightSource> lightSources;
        private List<Shape> shapes;
        private Camera mainCamera;

        public int ShapeCount
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

        public World()
        {
            this.shapes = new List<Shape>();
            this.lightSources = new List<LightSource>();
        }

        public void AddShape(Shape s)
        {
            shapes.Add(s);
        }

        public void RemoveShape(Shape s)
        {
            shapes.Remove(s);
        }

        public void SetCamera(Camera camera)
        {
            mainCamera = camera;
        }

        public void AddLightSource(LightSource source)
        {
            lightSources.Add(source);
        }

        public void Remove(LightSource source)
        {
            lightSources.Remove(source);
        }

        public Shape GetShape(int index)
        {
            return this.shapes[index];
        }

        public LightSource GetLightSources(int index)
        {
            return lightSources[index];
        }

        public Camera GetMainCamera()
        {
            return mainCamera;
        }
    }
}