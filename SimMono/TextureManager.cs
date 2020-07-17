using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace SimMono
{
    public sealed class TextureManager
    {
        private static Dictionary<string, Texture2D> _cache { get; set; } = new Dictionary<string, Texture2D>();

        private TextureManager()
        {
        }

        public static Texture2D GetTexture(string file)
        {
            if (_cache.ContainsKey(file)) return _cache[file];

            FileStream fileStream = new FileStream(file, FileMode.Open);
            var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
            fileStream.Dispose();

            _cache.Add(file, texture);

            return texture;
        }
    }
}