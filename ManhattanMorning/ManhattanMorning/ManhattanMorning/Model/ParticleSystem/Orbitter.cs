using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Controller;

namespace ManhattanMorning.Model.ParticleSystem
{
    public class Orbitter
    {
        #region Members
        private Vector2 position;
        private float rotation;
        private int particleCount;
        private float particleSpeed;
        private Vector2 particleSize;
        private Texture2D particleTexture;
        private Random random;
        private float offset = 0;
        private float width = 1f;
        private float height = 1f;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random rng = new Random();

        private DrawableObject linkedObject;

        /// <summary>
        /// Lifetime of the particles. If lifeTimeOffset differs from zero, this is the maximum lifetime the first particle has.
        /// </summary>
        private float lifeTime = 0f;

        /// <summary>
        /// This is the difference lifetime between each particle. If zero, all particle have the same lifetime.
        /// </summary>
        private float lifeTimeOffset = 0f;
        
        /// <summary>
        /// List of the Particles
        /// </summary>
        private List<Particle> particleList;

        /// <summary>
        /// True if visible, false otherwise.
        /// </summary>
        private bool visible;

        /// <summary>
        /// Particle color.
        /// </summary>
        private Color color;

        #endregion

        #region Properties
        /// <summary>
        /// Link the Position of the Particle Emitter to any Drawable Object
        /// The Position of the Emitter is set to the Center of the object
        /// </summary>
        public DrawableObject LinkedObject { get { return linkedObject; } set { linkedObject = value; } }

        /// <summary>
        /// Center of the Orbit
        /// </summary>
        public Vector2 Position { get { return this.position; } set { this.position = value; } }       

        /// <summary>
        /// Width in Meter
        /// </summary>
        public float Width { get { return this.width; } set { this.width = value; } }

        /// <summary>
        /// Heigth in Meter
        /// </summary>
        public float Height { get { return this.height; } set { this.height = value; } }

        /// <summary>
        /// The Up Vector. Needed to rotate the orbit
        /// </summary>
        public float Rotation { get { return this.rotation;} set { this.rotation = value;} }

        /// <summary>
        /// Number of Particles which are evenly distributed on the ellipse around the center
        /// </summary>
        public int ParticleCount
        { get { return this.particleCount; } 
          set 
          { 
              this.particleCount = value;
              initializeParticles(lifeTime, lifeTimeOffset);
              calculateParticlePositions(0f);
          }
        }

        /// <summary>
        /// Speed of the orbitting Particles in revolutions per Second
        /// </summary>
        public float ParticleSpeed { get { return this.particleSpeed; } set { this.particleSpeed = value; } }

        /// <summary>
        /// Size of the Particles
        /// </summary>
        public Vector2 ParticleSize { get { return this.particleSize; } set { this.particleSize = value; } }

        /// <summary>
        /// Texture of each Particle
        /// </summary>
        public Texture2D ParticleTexture { get { return this.particleTexture; } set { this.particleTexture = value; } }

        /// <summary>
        /// True if visible, false otherwise.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        /// <summary>
        /// Particle color.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        #endregion

        #region Init
        /// <summary>
        /// Creates a new Orbitter with Position of the center, lifetime of the particles and difference in lifetime.
        /// </summary>
        /// <param name="pos">Position of the center</param>
        public Orbitter(Vector2 pos)
        {
            this.position = pos;
            this.random = new Random();
            this.particleList = this.particleList = new List<Particle>();
        }

        /// <summary>
        /// Creates a new Orbitter with Position of the center, lifetime of the particles and difference in lifetime.
        /// </summary>
        /// <param name="pos">Position of the center</param>
        /// <param name="lifeTime">Lifetime of the particles. If lifeTimeOffset differs from zero, this is the maximum lifetime the first particle has.</param>
        /// <param name="lifeTimeOffset">This is the difference lifetime between each particle. If zero, all particle have the same lifetime.</param>
        public Orbitter(Vector2 pos, float lifeTime, float lifeTimeOffset)
            : this(pos)
        {
            this.lifeTime = lifeTime;
            this.lifeTimeOffset = lifeTimeOffset;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the positions of the particles and hides them if necessary. Position if only a Vector relative to the center of the orbitter which is then calculated in
        /// Graphics class beacuse there is additional rotation and translation.
        /// </summary>
        /// <param name="elapsedTimeInMS">Time in milliseconds since last update.</param>
        private void calculateParticlePositions(float elapsedTimeInMS)
        {
            double stepAngle = (2.0d * Math.PI)/this.particleCount;

            for (int i = 0; i < particleCount; i++)
            {
                Particle p = particleList[i];
                p.Age += elapsedTimeInMS / 1000f;

                //Check if Particle is dead
                if (p.Age >= p.LifeTime && p.LifeTime != 0)
                {
                    //If Particle is dead => hide it
                    //    Logger.Instance.log(Sender.ParticleSystem, "Particle Killed", PriorityLevel.Priority_5);
                    p.Visible = false;
                }
                else //Update Particle
                {
                    Vector2 dirVec = new Vector2(width * (float)Math.Sin(stepAngle * i + offset), height * (float)Math.Cos(stepAngle * i + offset));
                    p.Position = dirVec;
                }
            }
        }

        /// <summary>
        /// Create all the new Particles
        /// </summary>
        public void initializeParticles(float lifeTime, float lifeTimeOffset)
        {
            particleList.Clear();
            double stepAngle = (2 * Math.PI)/this.particleCount;
            for (int i = 0; i < particleCount; i++)
            {
                Particle p = new Particle(new Vector2(width*(float)Math.Sin(stepAngle * i + offset),height* (float)Math.Cos(stepAngle * i + offset)), lifeTime - i*lifeTimeOffset, (float) rng.NextDouble());
                p.Color = Color.White;
                p.Visible = true;
                particleList.Add(p);
            }
        }

        /// <summary>
        /// Return all Particles in this Orbitter.
        /// </summary>
        /// <returns>All Particles.</returns>
        public List<Particle> getParticles()
        {
            return this.particleList;
        }

        /// <summary>
        /// Reset all Particles in this Orbitter.
        /// </summary>
        /// <returns>All Particles.</returns>
        public void resetParticles()
        {
            foreach (Particle p in particleList)
            {
                p.Age = 0f;
                p.Visible = true;
            }
        }

        /// <summary>
        /// Updates the positions of the orbitter elements
        /// </summary>
        /// <param name="time">the current GameTime</param>
        public void update(GameTime time)
        {
            if (!visible) return;
            //Magic Number is 2.0f * Pi (Full Circle in Radians)
            //this.rotation +=  6.28318530f * ((float)time.ElapsedGameTime.TotalSeconds) * this.ParticleSpeed;
            
            //TODO Rotation aus Grafk ausbauen!!!!

            this.offset += 0.04f;

            if (linkedObject != null) position = linkedObject.Position + new Vector2(linkedObject.Size.X / 2, 0f);

            calculateParticlePositions((float) time.ElapsedGameTime.TotalMilliseconds);
        }

        /// <summary>
        /// Updates the center Position of the Orbiter system
        /// </summary>
        /// <param name="pos">Position in Meter</param>
        public void updatePosition(Vector2 pos)
        {
            this.position = pos;
        }
        
        #endregion

    }
}
