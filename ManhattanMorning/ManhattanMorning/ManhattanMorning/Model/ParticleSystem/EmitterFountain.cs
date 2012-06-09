using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.ParticleSystem
{
    /// <summary>
    /// Emitter Fountain emitts Particles in the emitter Direction within a given emitterAngle
    /// </summary>
    class EmitterFountain : Emitter
    {

        #region Members
        private Vector2 emitterDirection;
        private float emitterAngle;
        private Vector2 initialParticleVelocityVariance;

        /// <summary>
        /// Indicates how much emitting speed of the particles can vary from original speed.
        /// X is minimum speed, Y is maximum speed.
        /// </summary>
        public Vector2 InitialParticleVelocityVariance
        {
            get { return initialParticleVelocityVariance; }
            set { initialParticleVelocityVariance = value; }
        }
        private Random random;

        //Vector Orthogonal to the emitter Direction
        private Vector2 orthogonalDirection;

        #endregion

        #region Properties
        /// <summary>
        /// The Primary Direction in which the Particles are emitted
        /// </summary>
        public Vector2 EmitterDirection 
        {
            get { return this.emitterDirection; }

            set
            {
                this.emitterDirection = value;

                //Reset orthogonal
                orthogonalDirection = new Vector2(this.emitterDirection.Y, -this.emitterDirection.X);
                orthogonalDirection.Normalize();
                
            }
        }

        /// <summary>
        /// The Opening Angle of the fountain 0: 0 Degrees: 1: 90°
        /// Clamped between 0 and 1
        /// </summary>
        public float EmitterAngle 
        {
            get { return this.emitterAngle; }
            set
            {
                if (value < 0) this.emitterAngle = 0;
                //else if (value > 1) this.emitterAngle = 1;
                else this.emitterAngle = value;
            } 
        }        

        #endregion

        #region Initialization
        public EmitterFountain(Vector2 position, int numberOfParticles, float particlesPerSecond) 
            : base(position, numberOfParticles, particlesPerSecond)
        {
            random = new Random((int) particlesPerSecond);
            orthogonalDirection = new Vector2();
            this.EmitterDirection = new Vector2(0f,-1.0f);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Generates a Velocity with a Random direction and initial Speed
        /// </summary>
        /// <returns>Velocity as Vector2</returns>
        protected override Vector2 getStartVelocity()
        {
            Vector2 vel = new Vector2();
            float precision = 100000f;

            //Create Random angle 
            float rndAngle = (random.Next(0, +(int)(2.0f * emitterAngle * precision))- (emitterAngle * precision))/ precision;

            vel = emitterDirection + (orthogonalDirection * rndAngle); ;

            vel.Normalize();

            return vel * ((initialParticleVelocityVariance != Vector2.Zero) ? 
                initialParticleVelocityVariance.X + (float) random.NextDouble() 
                * (initialParticleVelocityVariance.Y - initialParticleVelocityVariance.X)
                : initialParticleSpeed);
        }

        #endregion


    }
}
