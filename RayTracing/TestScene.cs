namespace RayTracing
{
    public class TestScene
    {
        private const bool MULTI_THREADING_ENABLED = true;

        private const string SAVE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Generated";

        private const int CHUNKS_X = 32;
        private const int CHUNKS_Y = 32;

        private const int RES_X = 4096;
        private const int RES_Y = 4096;

        private const int RAYS_PER_PIXEL_X = 64;
        private const int RAYS_PER_PIXEL_Y = 64;

        private const int BOUCES = 64;

        private World world;

        public TestScene()
        {
            world = new World(null, null);

            Camera camera = new Camera(
                new Vector3D(1, 0, 0),
                new Vector3D(0, 1f, .2f),
                new Vector3D(-10, -10, -5),
                new Vector3D(0, 5, -15),
                10,
                10,
                0,
                new Int2D(RES_X, RES_Y),
                new Int2D(RAYS_PER_PIXEL_X, RAYS_PER_PIXEL_Y),
                BOUCES);

            world.SetMainCamera(camera);

            GlobalLight globalLight = new GlobalLight(new Vector3D(0f, 100f, 0f), new Vector3D(0.05f, -1f, -0.5f), new RTColor(255, 255, 251, 245));

            world.SetGlobalLightSource(globalLight);

            world.AddShape(new SphereShape(new Vector3D(0, 0, 0), 1f, (incidentClr, incidentRay, pointOfContact) =>
            {
                return new RTColor(incidentClr.Intensity / 1.2f, incidentClr.R / 2, incidentClr.G, incidentClr.B);
            }));
            world.AddShape(new SphereShape(new Vector3D(2.1f, 0, 0), 1f, (incidentClr, incidentRay, pointOfContact) =>
            {
                return new RTColor(incidentClr.Intensity / 1.2f, incidentClr.R, incidentClr.G, incidentClr.B);
            }));

            world.AddShape(new SphereShape(new Vector3D(1.05f, 0, 2.1f), 1f, (incidentClr, incidentRay, pointOfContact) =>
            {
                return new RTColor(incidentClr.Intensity / 1.4f, incidentClr.R, incidentClr.G / 10, incidentClr.B);
            }));

            world.AddShape(new SphereShape(new Vector3D(1f, 1.75f, 1f), 1f, (incidentClr, incidentRay, pointOfContact) =>
            {
                return incidentClr;
            }));

            /*
            world.AddShape(new SphereShape(new Vector3D(0, 2, 0), 1f, (incidentClr, incidentRay, pointOfContact) =>
            {
                return new RTColor(incidentClr.Intensity / 2, incidentClr.R, incidentClr.G / 5, incidentClr.B);
            }));

            world.AddShape(new SphereShape(new Vector3D(0, 4, 0), 1f, (incidentClr, incidentRay, pointOfContact) =>
            {
                return new RTColor(incidentClr.Intensity / 2, incidentClr.R, incidentClr.G, incidentClr.B / 5);
            }));
            */

            /*
            world.AddShape(new PlaneShape (new Vector3D(0, -10, 0), new Vector3D(0, 0, 1) * 100, new Vector3D(-1, 1) * 100, (clr, ray, poc) =>
            {
                //                return new RTColor(clr.Intensity, clr.R / 2, clr.G / 3, clr.B / 4);
                return new RTColor(clr.Intensity / 1.2f, clr.R, clr.G, clr.B);
            }));
            */

            world.AddShape(new PlaneShape(new Vector3D(-10, -1.1f, -10), new Vector3D(0, 0, 1) * 25, new Vector3D(1, 0, 0) * 25, (clr, ray, poc) =>
            {
                return new RTColor(clr.Intensity / 1.5f, clr.R, clr.G, clr.B);
            }));

            /*
            world.AddShape(new PlaneShape(new Vector3D(0, 0, 5.5f), new Vector3D(1, 0, 0) * 6, new Vector3D(0, 1, 0) * 2, (clr, ray, poc) =>
            {
                return clr;
            }));
            */

            /*
            world.AddShape(new PlaneShape(new Vector3D(0, -10, 0), new Vector3D(0, 0, 250), new Vector3D(250, 0, 0), (incidentClr, incidentRay, pointOfContact) =>
            {
                return new RTColor(incidentClr.Intensity / 2, incidentClr.R, incidentClr.G, incidentClr.B);
            }));
            */
        }

        public void Render()
        {
            string fileName = DateTime.Now.ToString().Replace(":", "-") + ".png";

            Renderer render = new Renderer(new Int2D(CHUNKS_X, CHUNKS_Y), world, Path.Combine(SAVE_LOCATION, fileName));
            render.Render(true);
        }
    }
}