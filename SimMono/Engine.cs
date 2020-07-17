using Microsoft.Xna.Framework.Graphics;
using SimMono.Models;
using SimMono.Models.Beings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimMono
{
    public sealed class Engine
    {
        public GraphicsDevice GraphicsDevice { get; set; }

        public List<Entity> Entities { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public Random RNG { get; } = new Random();

        private Engine()
        {
        }

        public static Engine Instance { get; } = new Engine();

        public void InitiliseEngine(List<Entity> entities, GraphicsDevice graphicsDevice, int width, int height, int initialPopulation)
        {
            GraphicsDevice = graphicsDevice;
            Entities = entities;
            ScreenWidth = width;
            ScreenHeight = height;

            for (var i = 0; i < initialPopulation; i++)
            {
                Entities.Add(new Boid(2, 2));
            }

            for (var i = 0; i < initialPopulation / 10; i++)
            {
                Entities.Add(new SafeZone(RNG.Next(40, 70), RNG.Next(40, 70)));
            }

            float numberOfPredators = initialPopulation / 20f;

            if (numberOfPredators < 1) numberOfPredators = 1;

            for (var i = 0; i < numberOfPredators; i++)
            {
                Entities.Add(new Predator(4, 4));
            }

            for (var i = 0; i < initialPopulation; i++)
            {
                Entities.Add(new Plant(4, 4));
            }
        }

        public void Update()
        {
            var beings = Entities.OfType<Being>();
            var beingsToRemove = beings.Where(being => !being.IsAlive && being.TimeOfDeath < DateTime.Now.AddSeconds(-5)).ToList();

            foreach (var being in beingsToRemove)
            {
                Entities.Remove(being);
            }

            if (RNG.Next(0, 10) == 0)
            {
                var plant = new Plant(2, 2);
                Entities.Add(plant);
            }

            foreach (var entity in Entities)
            {
                entity.Update();
            }
        }
    }
}