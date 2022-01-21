namespace RayTracing
{
    public enum TypeIDs
    {
        Light = 1,
        Shape = 1 << 1,
        Camera = 1<< 2,

        PointLight = Light & (0 << 31),
        SunLight = Light & (1 << 31)
    }
}
