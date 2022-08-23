namespace RayTracing
{
    public class TestScene2
    {
        private const string SPHERE_MESH = @"D:\Projects\VisualStudio\RayTracing\Assets\Sphere.obj";

        private const string RED_TEXT = @"D:\Projects\VisualStudio\RayTracing\Assets\Red.png";
        private const string GREEN_TEXT = @"D:\Projects\VisualStudio\RayTracing\Assets\Green.png";
        private const string BLUE_TEXT = @"D:\Projects\VisualStudio\RayTracing\Assets\Blue.png";

        private const string PLANE_MESH = @"D:\Projects\VisualStudio\RayTracing\Assets\Plane.obj";

#if RELEASE
        private const bool MULTI_THREADING_ENABLED = true;
#elif DEBUG
        private const bool MULTI_THREADING_ENABLED = false;
#endif

        private const string SAVE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Generated";

        private const int CHUNKS_X = 16;
        private const int CHUNKS_Y = 16;

        private const int RES_X = 1920;
        private const int RES_Y = 1080;

        private const int RAYS_PER_PIXEL_X = 1;
        private const int RAYS_PER_PIXEL_Y = 1;

        private const int BOUCES = 1;

        private const int WORLD_SUBDIVISION_LEVEL = 0;

        private World world;
        private Camera camera;

        public TestScene2()
        {
            world = new World(null, WORLD_SUBDIVISION_LEVEL, default);

            Transformation cameraTransform = new Transformation(new Vector3D(0, 25, -15), new Vector3D(-15, 0, 0) * RTMath.DEG_TO_RAD, new Vector3D(100f / 1024f * 1920f, 100f / 1024f * 1080f, 25));

            Transformation sphere1Transform = new Transformation(new Vector3D(-10, 0, 25), default(Vector3D), Vector3D.One * 5f);
            Transformation sphere2Transform = new Transformation(new Vector3D(10, 0, 25), default(Vector3D), Vector3D.One * 5f);
            Transformation sphere3Transform = new Transformation(new Vector3D(0, 0, 35), default(Vector3D), Vector3D.One * 5f);

            Vector3D globalLightDirection = new Vector3D(0, -1f, .5f);
            RTColor globalLightColor = new RTColor(RTColor.MAX_INTENSITY, 1f, 1f, 1f);

            ShapeShader sphere1Shader = new DefaultShapeShader(TextureLoader.Load(RED_TEXT), null, 0, 1, 0);
            ShapeShader sphere2Shader = new DefaultShapeShader(TextureLoader.Load(GREEN_TEXT), null, 0, 1, 0);
            ShapeShader sphere3Shader = new DefaultShapeShader(TextureLoader.Load(BLUE_TEXT), null, 0, 1, 0);

            camera = new Camera(cameraTransform, new Int2D(RES_X, RES_Y), new DefaultPixelShader(new Int2D(RAYS_PER_PIXEL_X, RAYS_PER_PIXEL_Y)), BOUCES);
            world.SetMainCamera(camera);

            GlobalLight light = new GlobalLight(default(Transformation), globalLightDirection, globalLightColor);
            world.AddLightSource(light);

            PointLight pointLight = new PointLight(new RTColor(RTColor.MAX_INTENSITY, 1f, 1f, 1f), new Transformation(new Vector3D(0, 0, 85f / 3)));
            world.AddLightSource(pointLight);

            MeshBuilder sphereBuilder = MeshReader.ReadObj(SPHERE_MESH);

            world.AddShapes(sphereBuilder.Build(sphere1Transform, sphere1Shader, true));
            world.AddShapes(sphereBuilder.Build(sphere2Transform, sphere2Shader, true));
            world.AddShapes(sphereBuilder.Build(sphere3Transform, sphere3Shader, true));
//            world.AddShape(MeshReader.ReadObj(PLANE_MESH).Build(new Transformation(new Vector3D(0, -10, 0), default(Vector3D), new Vector3D(10000, 1, 10000)), sphere1Shader, true));
        }

        public void RenderAndSave()
        {
            String location = Path.Combine(SAVE_LOCATION, DateTime.Now.ToString().Replace(":", "-") + ".png");
            Renderer.RenderAndWriteToDisk(world, camera, new Int2D(CHUNKS_X, CHUNKS_Y), MULTI_THREADING_ENABLED, location);
        }
    }
}