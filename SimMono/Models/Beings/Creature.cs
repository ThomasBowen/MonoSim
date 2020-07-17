using Microsoft.Xna.Framework;
using SimMono.Enums;
using SimMono.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimMono.Models.Beings
{
    public abstract class Creature : Being
    {
        private List<Entity> _visibleBeings;
        private List<Being> _visiblePrey;
        private List<SafeZone> _visibleSafeZone;

        protected List<Entity> VisibleEntities
        {
            get
            {
                if (_visibleBeings == null)
                {
                    _visibleBeings = Engine.Instance.Entities.Where(n => VectorHelper.TorusDistance(Position, n.Position) < Vision && n != this).ToList();
                }

                return _visibleBeings;
            }
            set
            {
                _visibleBeings = value;
            }
        }

        protected List<Being> VisibleFood
        {
            get
            {
                if (_visiblePrey == null)
                {
                    _visiblePrey = VisibleEntities.OfType<Being>().Where(n => n.IsAlive && (Consumes?.Contains(n.GetType()) ?? false)).ToList();
                }

                return _visiblePrey;
            }
            set
            {
                _visiblePrey = value;
            }
        }

        protected List<SafeZone> VisibleSafeZones
        {
            get
            {
                if (_visibleSafeZone == null)
                {
                    _visibleSafeZone = VisibleEntities.OfType<SafeZone>().ToList();
                }

                return _visibleSafeZone;
            }
            set
            {
                _visibleSafeZone = value;
            }
        }

        public State State { get; set; }
        public int Hunger { get; set; }
        public int MaxHunger { get; set; } = 2000;
        public int Fatigue { get; set; }
        public int MaxFatigue { get; set; } = 500;
        public Type[] Consumes { get; set; }
        public Vector2 Velocity { get; set; }
        public int Vision { get; set; } = Engine.Instance.ScreenWidth / 10;
        protected float MaxForce { get; set; } = 0.3f;
        protected float MaxSpeed { get; set; } = 2;

        public Creature(int width, int height, Color color) : base(width, height, "Content/Sprites/Creature.png", color)
        {
            var rng = Engine.Instance.RNG;
            Velocity = new Vector2(rng.Next(0, 10), rng.Next(0, 10));
        }

        public override void Update()
        {
            Hunger++;
            Fatigue++;

            if (Hunger > MaxHunger)
            {
                IsAlive = false;
                FinishUpdate();
                return;
            }

            CalculateState();

            UpdatePosition();

            ProcessCollisions();

            FinishUpdate();
        }

        protected abstract void CalculateState();

        private Vector2 CalculateAcceleration()
        {
            switch (State)
            {
                case State.Normal:
                    return NormalAcceleration();
                case State.Tired:
                    return TiredAcceleration();
                case State.Hungry:
                    return HungryAcceleration();
                case State.Danger:
                    return DangerAcceleration();
                default:
                    return NormalAcceleration();
            }
        }

        protected abstract Vector2 NormalAcceleration();

        protected abstract Vector2 HungryAcceleration();

        protected abstract Vector2 DangerAcceleration();

        protected Vector2 Seek(Vector2 target)
        {
            var steer = Vector2.Subtract(target, Position);

            return LimitSteer(steer);
        }

        protected Vector2 ApplyMultipleDirection(Vector2 steer, int count)
        {
            steer = Vector2.Divide(steer, count);

            return steer.Length() > 0 ? LimitSteer(steer) : steer;
        }

        protected Vector2 LimitSteer(Vector2 steer)
        {
            if (steer.Length() > 0) steer = Vector2.Normalize(steer);

            steer = Vector2.Multiply(steer, MaxSpeed);
            steer = Vector2.Subtract(steer, Velocity);
            steer = VectorHelper.Limit(steer, MaxForce);

            return steer;
        }

        protected virtual void ProcessCollisions()
        {
            foreach (var food in VisibleFood)
            {
                var requiredProximity = (Width / 2) + (food.Width / 2);

                if (VectorHelper.TorusDistance(food.Position, Position) < requiredProximity)
                {
                    Eat(food);
                }
            }
        }

        protected Vector2 Hunt()
        {
            if (!VisibleFood.Any()) return new Vector2(0, 0);

            var averageFood = new Vector2(0, 0);

            foreach (var food in VisibleFood)
            {
                if (VectorHelper.TorusDistance(food.Position, Position) > Vision) return new Vector2(0, 0);

                averageFood = Vector2.Add(averageFood, food.Position);
            }

            averageFood = Vector2.Divide(averageFood, VisibleFood.Count());
            averageFood = Seek(averageFood);

            return averageFood;
        }

        protected Vector2 Attack()
        {
            if (!VisibleFood.Any()) return new Vector2(0, 0);

            var closestBoid = VisibleFood.OrderBy(b => VectorHelper.TorusDistance(b.Position, Position)).FirstOrDefault();

            if (VectorHelper.TorusDistance(closestBoid.Position, Position) > Vision / 4) return new Vector2(0, 0);

            var steer = Seek(closestBoid.Position);

            return steer;
        }

        protected Vector2 TiredAcceleration()
        {
            var acceleration = new Vector2(0, 0);
            acceleration += Vector2.Multiply(FindSafeZone(), 7f);

            return acceleration;
        }

        protected Vector2 FindSafeZone()
        {
            if (!VisibleSafeZones.Any()) return new Vector2(0, 0);

            var averageSafeZone = new Vector2(0, 0);

            foreach (var safeZone in VisibleSafeZones)
            {
                if (VectorHelper.TorusDistance(safeZone.Position, Position) > Vision) return new Vector2(0, 0);

                averageSafeZone = Vector2.Add(averageSafeZone, safeZone.Position);
            }

            averageSafeZone = Vector2.Divide(averageSafeZone, VisibleSafeZones.Count());
            averageSafeZone = Seek(averageSafeZone);

            return averageSafeZone;
        }

        private void Eat(Being being)
        {
            being.IsAlive = false;

            Width += being.Width / 5;
            Height += being.Height / 10;

            Hunger -= being.Width * 100;
        }

        private void UpdatePosition()
        {
            var acceleration = CalculateAcceleration();

            Velocity += acceleration;

            Velocity = VectorHelper.Limit(Velocity, MaxSpeed);

            Position += Velocity;

            var x = Position.X;
            var y = Position.Y;

            if (x > Engine.Instance.ScreenWidth - 10) x = 10;
            if (x < 10) x = Engine.Instance.ScreenWidth - 10;
            if (y > Engine.Instance.ScreenHeight - 10) y = 10;
            if (y < 10) y = Engine.Instance.ScreenHeight - 10;

            Position = new Vector2(x, y);
        }

        private void FinishUpdate()
        {
            VisibleEntities = null;
            VisibleFood = null;
            VisibleSafeZones = null;

            base.Update();
        }
    }
}