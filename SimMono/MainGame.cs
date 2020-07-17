using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimMono.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimMono
{
    public class MainGame : Game
    {
        private List<Entity> _entities { get; set; } = new List<Entity>();
        private Queue<double> _fpsHistory { get; set; } = new Queue<double>(50);
        private DateTime _lastUpdate { get; set; } = DateTime.Now;

        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        public MainGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d);
        }

        protected override void LoadContent()
        {
            var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Engine.Instance.InitiliseEngine(_entities, GraphicsDevice, width, height, 100);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("Arial");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Engine.Instance.Update();

            UpdateFPS();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_spriteFont, $"{Math.Round(_fpsHistory.Average())} fps", new Vector2(100, 100), Color.White);

            foreach (var entity in _entities)
            {
                _spriteBatch.Draw(entity.Texture, new Rectangle(entity.X, entity.Y, entity.Width, entity.Height), entity.Color);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateFPS()
        {
            var timeBetweenFrames = DateTime.Now - _lastUpdate;

            var fps = 1 / timeBetweenFrames.TotalSeconds;

            if (_fpsHistory.Count == 50) _fpsHistory.Dequeue();

            _fpsHistory.Enqueue(fps);

            _lastUpdate = DateTime.Now;
        }
    }
}
