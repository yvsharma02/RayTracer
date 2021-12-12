namespace RayTracing
{
    public class TestScene
    {
        private const string PLANE_TEXTURE = @"D:\Projects\VisualStudio\RayTracing\Assets\TestTexture.png";

        private const bool MULTI_THREADING_ENABLED = true;

        private const string SAVE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Generated";

        private const int CHUNKS_X = 16;
        private const int CHUNKS_Y = 16;

        private const int RES_X = 1024;
        private const int RES_Y = 1024;

        private const int RAYS_PER_PIXEL_X = 4;
        private const int RAYS_PER_PIXEL_Y = 4;

        private const int BOUCES = 3;

        private World world;

        /*
        private RTColor CalculateColor(ColoredRay[][] clrs, float iDiv, float rDiv, float gDiv, float bDiv)
        {
            
        }
        */

        public TestScene()
        {
            Vector3D lightPos = new Vector3D(20, 0.5f, 0);
            Vector3D spherePos = new Vector3D(21, 3.3f, 5);
            world = new World(null, null);

            Vector3D PlaneAxis1 = new Vector3D(0, 0, 50);
            Vector3D PlaneAxis2 = new Vector3D(50, 0, 0);

            Camera camera = new Camera(
                new Vector3D(1, 0, 0),
                new Vector3D(0, 1f, 0f),
                new Vector3D(-15, -15, 0),
                new Vector3D(25, 25, -5),
                75,
                75,
                0,
                new Int2D(RES_X, RES_Y),
                new Int2D(RAYS_PER_PIXEL_X, RAYS_PER_PIXEL_Y),
                BOUCES,
                new RTColor(RTColor.MAX_INTENSITY / 3, 123, 123, 123));

            world.SetMainCamera(camera);

            DefaultShapeShader planeShader = new DefaultShapeShader(TextureLoader.Load(PLANE_TEXTURE));
            world.AddShape(new PlaneShape(new Vector3D(), PlaneAxis1, PlaneAxis2, planeShader));

            DefaultShapeShader sphereShader = new DefaultShapeShader();
            world.AddShape(new SphereShape(spherePos, 2.5f, sphereShader));

            RTColor sunColor = new RTColor(RTColor.MAX_INTENSITY, 255, 250, 242);
            Vector3D sunAxis1 = new Vector3D(0.01f, 0, 0);
            Vector3D sunAxis2 = new Vector3D(0, -0.01f, 0.01f);

            world.AddLightSource(new GlobalLight(new Vector3D(0, 100, 0), sunAxis1, sunAxis2, new Int2D(2, 2), sunColor));
            world.AddLightSource(new PointLight(new RTColor(RTColor.MAX_INTENSITY, 255f, 255f, 0f), lightPos));
        }

        public void Render()
        {
            string fileName = DateTime.Now.ToString().Replace(":", "-") + ".png";

            Renderer render = new Renderer(new Int2D(CHUNKS_X, CHUNKS_Y), world, Path.Combine(SAVE_LOCATION, fileName));
            render.Render(MULTI_THREADING_ENABLED);
        }
    }
}