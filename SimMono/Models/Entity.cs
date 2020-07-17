using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimMono.Models
{
    public abstract class Entity
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }

        public int X => (int)Position.X;
        public int Y => (int)Position.Y;

        public Entity(int width, int height, string file, Color color)
        {
            Width = width;
            Height = height;
            Color = color;

            var rng = Engine.Instance.RNG;
            var screenWidth = Engine.Instance.ScreenWidth;
            var screenHeight = Engine.Instance.ScreenHeight;

            Texture = TextureManager.GetTexture(file);

            Position = new Vector2(rng.Next(0, screenWidth), rng.Next(0, screenHeight));
        }

        public abstract void Update();
    }
}