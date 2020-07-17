using Microsoft.Xna.Framework;
using SimMono.Helpers;
using System.Linq;

namespace SimMono.Models.Beings
{
    public abstract class Seperator : Creature
    {
        public Seperator(int width, int height, Color color) : base(width, height, color) { }

        protected Vector2 Separate(float desiredSeperation)
        {
            var steer = new Vector2(0, 0);

            var others = VisibleEntities.OfType<Being>().Where(being => being.IsAlive && being.GetType() == GetType());

            if (!others.Any()) return steer;

            foreach (var other in others)
            {
                if (other == this) continue;

                var distance = VectorHelper.TorusDistance(Position, other.Position);

                if (distance < desiredSeperation)
                {
                    var difference = Vector2.Subtract(Position, other.Position);
                    if (difference.Length() > 0) difference = Vector2.Normalize(difference);

                    // Weight by distance
                    if (distance != 0)
                    {
                        difference = Vector2.Divide(difference, distance);
                    }

                    steer = Vector2.Add(steer, difference);
                }
            }
            
            steer = Vector2.Divide(steer, others.Count());            

            if (steer.Length() > 0)
            {
                if (steer.Length() > 0) steer = Vector2.Normalize(steer);
                steer = Vector2.Multiply(steer, MaxSpeed);
                steer = Vector2.Subtract(steer, Velocity);

                steer = VectorHelper.Limit(steer, MaxForce);
            }

            return steer;
        }
    }
}