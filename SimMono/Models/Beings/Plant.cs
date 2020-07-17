using Microsoft.Xna.Framework;

namespace SimMono.Models.Beings
{
    public class Plant : Being
    {
        public Plant(int width, int height) : base(width, height, "Content/Sprites/Plant.png", Color.Green) { }
    }
}