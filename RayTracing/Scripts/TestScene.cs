namespace RayTracing
{
    public class TestScene
    {
        private const string TEST_MESH_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Assets\TextureSphere.obj";
        private const string TEST_TEXTURE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Assets\SphereTexture_Texture.png";

        private const bool MULTI_THREADING_ENABLED = true;

        private const string SAVE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Generated";

        private const int CHUNKS_X = 16;
        private const int CHUNKS_Y = 16;

        private const int RES_X = 1024;
        private const int RES_Y = 1024;

        private const int RAYS_PER_PIXEL_X = 1;
        private const int RAYS_PER_PIXEL_Y = 1;

        private const int BOUCES = 2;

        private World world;

        /*
        private RTColor CalculateColor(ColoredRay[][] clrs, float iDiv, float rDiv, float gDiv, float bDiv)
        {
            
        }
        */

        public TestScene()
        {
            Vector3D lightPos = new Vector3D(20, 0.5f, 0);
            Vector3D spherePos = new Vector3D(21, 0f, 100);
            world = new World(null, null);

            Camera camera = new Camera(
                new Vector3D(1, 0, 0),
                new Vector3D(0, 1f, 0f),
                new Vector3D(-15, -15, 0),
                new Vector3D(25, 25, -25),
                75,
                75,
                0,
                new Int2D(RES_X, RES_Y),
                new Int2D(RAYS_PER_PIXEL_X, RAYS_PER_PIXEL_Y),
                BOUCES,
                new RTColor(RTColor.MAX_INTENSITY / 3, 123, 123, 123));

            world.SetMainCamera(camera);

            DefaultShapeShader meshShader = new DefaultShapeShader(TextureLoader.Load(TEST_TEXTURE_LOCATION));

            RTColor sunColor = new RTColor(RTColor.MAX_INTENSITY, 255, 255, 255);
            Vector3D sunAxis1 = new Vector3D(0.0001f, 0, 0f);
            Vector3D sunAxis2 = new Vector3D(0f, 0, 0.0001f);

            Vector3D v = Vector3D.Cross(sunAxis1, sunAxis2);

            MeshBuilder builder = MeshReader.ReadObj(TEST_MESH_LOCATION);
            world.AddShape(builder.Build(spherePos, meshShader, 15f));

//            world.AddLightSource(new GlobalLight(new Vector3D(0, 100, 0), -sunAxis1, sunAxis2, new Int2D(2, 2), sunColor));
            world.AddLightSource(new GlobalLight(new Vector3D(0, 100, 0), sunAxis1, sunAxis2, new Int2D(2, 2), sunColor));
            world.AddShape(new PlaneShape(new Vector3D(0, 0, 0), new Vector3D(100, 0, 0), new Vector3D(0, 0, 100), new DefaultShapeShader()));
        }

        public void Render()
        {
            string fileName = DateTime.Now.ToString().Replace(":", "-") + ".png";

            Renderer render = new Renderer(new Int2D(CHUNKS_X, CHUNKS_Y), world, Path.Combine(SAVE_LOCATION, fileName));
            render.Render(MULTI_THREADING_ENABLED);
        }
    }
}