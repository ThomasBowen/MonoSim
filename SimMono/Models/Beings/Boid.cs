using Microsoft.Xna.Framework;
using SimMono.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SimMono.Models.Beings
{
    public class Boid : Seperator
    {
        private List<Predator> _visiblePredators { get; set; }

        protected List<Predator> VisiblePredators
        {
            get
            {
                if (_visiblePredators == null)
                {
                    _visiblePredators = VisibleEntities.OfType<Predator>().Where(n => n.IsAlive).ToList();
                }

                return _visiblePredators;
            }
            set
            {
                _visiblePredators = value;
            }
        }

        public Boid(int width, int height) : base(width, height, Color.Blue)
        {
            Consumes = new[] { typeof(Plant) };

            MaxForce = 0.4f;
            MaxSpeed += Engine.Instance.RNG.Next(1, 3);
        }

        protected override void CalculateState()
        {
            if (IsPredatorClose())
            {
                State = Enums.State.Danger;
            }
            else if (Hunger > MaxHunger * 0.8)
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

        public override void Update()
        {
            base.Update();

            VisiblePredators = null;
        }

        private bool IsPredatorClose()
        {
            return VisiblePredators.Any(b => VectorHelper.TorusDistance(b.Position, Position) < Vision / 2);
        }

        protected override Vector2 NormalAcceleration()
        {
            var boids = VisibleEntities.OfType<Boid>().Where(b => b.IsAlive).ToList();
            var predators = VisibleEntities.OfType<Predator>().Where(b => b.IsAlive).ToList();

            var acceleration = new Vector2(0, 0);
            acceleration += Vector2.Multiply(Flee(predators), 7f);
            acceleration += Vector2.Multiply(Separate(25f), 2.5f);
            acceleration += Vector2.Multiply(Align(boids), 1.2f);
            acceleration += Vector2.Multiply(Cohesion(boids), 1.3f);

            acceleration += Vector2.Multiply(Hunt(), 2f);
            acceleration += Vector2.Multiply(Attack(), 7f);

            return acceleration;
        }

        protected override Vector2 HungryAcceleration()
        {
            var boids = VisibleEntities.OfType<Boid>().Where(b => b.IsAlive).ToList();
            var predators = VisibleEntities.OfType<Predator>().Where(b => b.IsAlive).ToList();

            var acceleration = new Vector2(0, 0);
            acceleration += Vector2.Multiply(Flee(predators), 2f);
            acceleration += Vector2.Multiply(Separate(10f), 0.5f);
            acceleration += Vector2.Multiply(Align(boids), 0.5f);
            acceleration += Vector2.Multiply(Cohesion(boids), 0.5f);

            acceleration += Vector2.Multiply(Hunt(), 5f);
            acceleration += Vector2.Multiply(Attack(), 10f);

            return acceleration;
        }

        protected override Vector2 DangerAcceleration()
        {
            var predators = VisibleEntities.OfType<Predator>().Where(b => b.IsAlive).ToList();

            var acceleration = new Vector2(0, 0);
            acceleration += Vector2.Multiply(Flee(predators), 7f);

            return acceleration;
        }

        private Vector2 Flee(List<Predator> predators)
        {
            if (!predators.Any()) return new Vector2(0, 0);

            var steer = new Vector2(0, 0);

            foreach (var predator in predators)
            {
                float distance = VectorHelper.TorusDistance(Position, predator.Position);
                var difference = Vector2.Subtract(Position, predator.Position);
                if (difference.Length() > 0) difference = Vector2.Normalize(difference);
                difference = Vector2.Divide(difference, distance);

                steer = Vector2.Add(steer, difference);
            }

            steer = ApplyMultipleDirection(steer, predators.Count);

            return steer;
        }

        private Vector2 Align(List<Boid> boids)
        {
            if (!boids.Any()) return new Vector2(0, 0);

            var steer = new Vector2(0, 0);

            foreach (var other in boids)
            {
                steer = Vector2.Add(steer, other.Velocity);
            }

            steer = ApplyMultipleDirection(steer, boids.Count);

            return steer;
        }

        private Vector2 Cohesion(List<Boid> boids)
        {
            if (boids.Count() < 2) return new Vector2(0, 0);

            var steer = new Vector2(0, 0);

            foreach(var other in boids)
            {
                steer = Vector2.Add(steer, other.Position);
            }

            steer = Vector2.Divide(steer, boids.Count());
            steer = Seek(steer);

            return steer;
        }
    }
}