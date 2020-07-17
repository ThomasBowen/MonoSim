using Microsoft.Xna.Framework;
using System;

namespace SimMono.Models.Beings
{
    public abstract class Being : Entity
    {
        private bool _isAlive = true;

        public TimeSpan AvergeLifeSpan { get; set; } = new TimeSpan(0, 0, 300);
        public DateTime TimeOfBirth { get; } = DateTime.Now;
        public DateTime? TimeOfDeath { get; set; } = null;

        public bool IsAlive
        {
            get
            {
                return _isAlive;
            }
            set
            {
                if (value)
                { 
                    TimeOfDeath = null;
                }
                else if (_isAlive)
                {
                    TimeOfDeath = DateTime.Now;
                    Color = Color.Gray;
                }

                _isAlive = value;
            }
        }

        public Being(int width, int height, string file, Color color) : base(width, height, file, color) { }

        public override void Update()
        {
            if (DateTime.Now - TimeOfBirth > AvergeLifeSpan) IsAlive = false;

            if (!IsAlive) return;
        }
    }
}