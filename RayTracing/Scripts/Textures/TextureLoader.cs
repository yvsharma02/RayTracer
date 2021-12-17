using System.Drawing;

namespace RayTracing
{
    public class TextureLoader
    {

        private static List<Texture> LoadedTextures;

        static TextureLoader()
        {
            LoadedTextures = new List<Texture>();
        }

        public static Texture Load(String location)
        {
            foreach (Texture texture in LoadedTextures)
            {
                if (texture.Location == location)
                    return texture;
            }

            Texture txt = new Texture(location);
            LoadedTextures.Add(txt);
            return txt;
        }
    }
}