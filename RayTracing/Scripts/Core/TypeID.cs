namespace RayTracing
{
    public enum TypeID
    {
        Light = 1,
        Shape = 1 << 1,
        Camera = 1 << 2,

        Skybox = Shape | (1 << 31),

        PointLight = Light | (0 << 31),
        SunLight = Light | (1 << 31)
    }
}
