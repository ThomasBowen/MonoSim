using Microsoft.Xna.Framework;

namespace SimMono.Models
{
    public class SafeZone : Entity
    {
        public SafeZone(int width, int height) : base(width, height, "Content/Sprites/SafeZone.png", Color.Purple)
        {

        }

        public override void Update()
        {
            //throw new System.NotImplementedException();
        }
    }
}