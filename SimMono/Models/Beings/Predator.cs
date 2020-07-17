using Microsoft.Xna.Framework;

namespace SimMono.Models.Beings
{
    public class Predator : Seperator
    {
        public Predator(int width, int height) : base(width, height, Color.Red)
        {
            Consumes = new [] { typeof(Boid) };

            Vision *= 2;
            MaxForce = 0.4f;
            MaxSpeed += Engine.Instance.RNG.Next(3, 5);
        }

        protected override void CalculateState()
        {
            //if (IsPredatorClose())
            //{
            //    State = Enums.State.Danger;
            //}
            if (Hunger > MaxHunger * 0.6)
            {
                State = Enums.State.Hungry;
            }
            else if (Fatigue > MaxFatigue)
            {
                State = Enums.State.Tired;
            }
            else
            {
                State = Enums.State.Normal;
            }
        }

        protected override Vector2 NormalAcceleration()
        {
            var acceleration = new Vector2(0, 0);
            acceleration += Vector2.Multiply(Separate(Engine.Instance.ScreenWidth / 4), 2.5f);

            acceleration += Vector2.Multiply(Hunt(), 1f);
            acceleration += Vector2.Multiply(Attack(), 3f);

            return acceleration;
        }

        protected override Vector2 DangerAcceleration()
        {
            throw new System.NotImplementedException();
        }

        protected override Vector2 HungryAcceleration()
        {
            var acceleration = new Vector2(0, 0);
            acceleration += Vector2.Multiply(Separate(Engine.Instance.ScreenWidth / 4), 2.5f);

            acceleration += Vector2.Multiply(Hunt(), 4f);
            acceleration += Vector2.Multiply(Attack(), 10f);

            return acceleration;
        }
    }
}