using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Model.ParticleSystem
{
    /// <summary>
    /// Explosion Emitter emitts Particles uniform in all directions
    /// </summary>
    class EmitterExplosion : Emitter
    {
        #region Members
        private Random random = new Random();
        #endregion


        #region Methods

        #region Initialization

        public EmitterExplosion(Vector2 position, int numberOfParticles, float particlesPerSecond) 
            : base(position, numberOfParticles, particlesPerSecond)
        {
        }

        #endregion

        /// <summary>
        /// Generates a Velocity with a random direction and initial Speed
        /// </summary>
        /// <returns>Velocity as Vector2</returns>
        protected override Vector2 getStartVelocity()
        {
            float precision = 100000f;
            //Generate Values between -50000 and +50000
            float rndX = (random.Next(0, (int)precision) - 0.5f * precision);
            float rndY = (random.Next(0, (int)precision) - 0.5f * precision);
            Vector2 vel = new Vector2(rndX,rndY);
            //Normalize Vector
            vel.Normalize();
            //Scale Vector with initial Particle Speed
            return vel*initialParticleSpeed;
        }

        #endregion
    }
}
