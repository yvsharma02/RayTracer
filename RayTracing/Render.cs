`namespace RayTracing
{
    public class Render
    {
        private World world;

        private Int2D pixelBoundsStart;
        private Int2D pixelBoundsEnd;

        public Render(World world, Int2D pbs, Int2D pbe)
        {
            this.world = world;
        }

        public void SetPixelBounds(Int2D pbs, Int2D pbe)
        {
            Int2D worldStart = new Int2D(0, 0);
            Int2D worldEnd = new Int2D(world.GetMainCamera().ResolutionX, world.GetMainCamera().ResolutionY);

            if (pbs.x < worldStart.x || pbs.y < worldStart.y || pbs.x > worldEnd.x || pbs.y > worldEnd.y)
                throw new ArgumentOutOfRangeException("Pixel Start Index");
            else if (pbe.x < worldStart.x || pbe.y < worldStart.y || pbe.x > worldEnd.x || pbe.y > worldEnd.y)
                throw new ArgumentOutOfRangeException("Pixel End Index");

            if (pbe.x < pbs.x || pbe.y < pbs.y)
                throw new ArgumentException("Pixel Bounds");

            this.pixelBoundsStart = pbe;
            this.pixelBoundsEnd = pbe;
        }

        // Ouput, Time render was started, World to render, pixelStartIndex, PixelEndIndex
        public void Render(Action<RTColor[,], DateTime, World, Int2D, Int2D> output)
        {
            
        }
    }
}
