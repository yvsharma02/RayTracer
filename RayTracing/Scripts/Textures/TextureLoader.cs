using System.Drawing;

namespace RayTracing
{
    public class TextureLoader
    {
        private static Dictionary<string, Texture> LoadedTextures;

        static TextureLoader()
        {
            LoadedTextures = new Dictionary<string, Texture>();
        }

        public static Texture Load(String location)
        {
            if(LoadedTextures.ContainsKey(location))
                return LoadedTextures[location];

            Texture txt = new Texture(location);
            LoadedTextures.Add(location, txt);
            return txt;
        }
    }
}