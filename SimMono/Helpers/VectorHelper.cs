using Microsoft.Xna.Framework;
using System;

namespace SimMono.Helpers
{
    public static class VectorHelper
    {
        public static Vector2 Limit(Vector2 vector, float limit)
        {
            var x = vector.X;
            var y = vector.Y;

            if (x > limit) x = limit;
            if (y > limit) y = limit;

            if (x < -limit) x = -limit;
            if (y < -limit) y = -limit;

            return new Vector2(x, y);
        }

        public static float TorusDistance(Vector2 first, Vector2 second)
        {
            var dx = Math.Abs(first.X - second.X);
            var dy = Math.Abs(first.Y - second.Y);

            if (dx > Engine.Instance.ScreenWidth / 2)
            {
                dx = Engine.Instance.ScreenWidth - dx;
            }

            if (dy > Engine.Instance.ScreenHeight / 2)
            {
                dy = Engine.Instance.ScreenHeight - dy;
            }

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}