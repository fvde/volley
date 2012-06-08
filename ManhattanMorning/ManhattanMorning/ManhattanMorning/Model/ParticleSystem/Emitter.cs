using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;
using ManhattanMorning.Model.GameObject;


namespace ManhattanMorning.Model.ParticleSystem
{

    /// <summary>
    /// The way particles fade out
    /// </summary>
    public enum FadeMode
    {
        None, //Just disappear when particle is dead
        Linear,
        Quadratic,
    }

    /// <summary>
    /// The way particles grow
    /// </summary>
    public enum GrowthMode
    {
        None, //Just disappear when particle is dead
        Linear,
        Quadratic,
    }


    /// <summary>
    /// Emitter of Particles
    /// </summary>
    public abstract class Emitter
    {
        #region Members

        //Strating Value for Particle Alpha
        private float particleStartingAlpha;

        //Position of the Vector
        private Vector2 position;

        //List of the Particles
        private List<Particle> particleList;

        //Forces for this emitter
        private List<Vector2> emitterForcesList;

        //Total number of Particles used fpr this emitter
        private int numberOfParticles;

        //Lifetime before Particle is killed
        private float particleLifeTime;

        //Shape of the emitter, where the Particles are spawned
        private EmitterShape emitterShape;

        //Used to store the Time since the Emitter emitted for the last time
        private float milliSecondsSinceLastEmit;

        //Rate of the Particles per Second the Emitter emits
        private float particlesPerSecond;

        //Initial Particle Speed
        protected float initialParticleSpeed;

        //Total Time The Emitter is running since it was set active.
        //Resetted when paused.
        private float totalEmittingTime;

        //Fade out mode
        protected FadeMode particlefadeOut;

        protected GrowthMode particleGrowth;

        //Freeze particles stop updating;
        private bool pause;

        //Start / Stop emitting Particles
        private bool active;

        /// <summary>
        /// Duration, how long Particles are emitted, before the emitter is paused.
        /// </summary>
        private float emittingDuration;

        //Texture of the Particles
        private Texture2D particleTexture;

        //Particle Size in Meters
        private Vector2 particleSize;

        //the ParticleSystem we are currently inside
        private ParticleSystem parentSystem;

        //Particle Growth Modifier
        private float particleGrowthRate;

        /// Link the Position of the Particle Emitter to any Drawable Object
        /// The Position of the Emitter is set to the Center of the object
        private DrawableObject linkedObject;

        /// <summary>
        /// Position of the emitter in the last update step.
        /// </summary>
        private Vector2 lastPosition;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random rng = new Random();

        /// <summary>
        /// Position Offset relative to the Center of the Object
        /// </summary>
        private Vector2 linkedObjectOffset = Vector2.Zero;

        /// <summary>
        /// True if the emitter rotates around the origin with its offset.
        /// </summary>
        private bool rotateWithOffset;

        /// <summary>
        /// If true, the position of the emitter is adjusted depending on the speed of the linked object.
        /// </summary>
        private bool includeMovingOffset;

        /// <summary>
        /// Particle Color.
        /// </summary>
        private Color particleColor;

        /// <summary>
        /// If true, the particles will move with the emitter so that there is no gap with increasing speed of the emitter.
        /// </summary>
        private bool movingParticles;        

        /// <summary>
        /// If set, the emitting duration will be a random number between x and y values of the vector.
        /// Z and W values represent the duration between 2 emits.
        /// The emit will repeat endlessly.
        /// 
        /// Values are in milliseconds.
        /// </summary>
        private Vector4 randomEmitDuration = Vector4.Zero;

        /// <summary>
        /// The distance between position in each update.
        /// </summary>
        private Vector2 movingOffset;

        #endregion

        #region Properties

        /// <summary>
        /// Starting position of the emitter;
        /// </summary>
        /*public Vector2 StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
        }*/

        /// <summary>
        /// Link the Position of the Particle Emitter to any Drawable Object
        /// The Position of the Emitter is set to the Center of the object
        /// </summary>
        public DrawableObject LinkedObject { get { return linkedObject; } set { linkedObject = value; } }

        /// <summary>
        /// Position Offset relative to the Center of the Object
        /// </summary>
        public Vector2 LinkedObjectOffset { get { return linkedObjectOffset; } set { linkedObjectOffset = value; } }

        /// <summary>
        /// List of all Particles linked to this emitter
        /// </summary>
        public List<Particle> ParticleList { get {return this.particleList ;} }

        /// <summary>
        /// Position of the emitter
        /// </summary>
        public Vector2 Position { get { return this.position; } set { this.position = value;} }

        /// <summary>
        /// Duration how long particles are emitted, before Emitter is set to inactive
        /// If it's 0 duration is infinite!
        /// </summary>
        public float EmittingDuration { get { return this.emittingDuration; } set { this.emittingDuration = value; } }

        /// <summary>
        /// Determines how the Particles are growing
        /// </summary>
        public GrowthMode ParticleGrowth {get{return this.particleGrowth;} set{this.particleGrowth = value;}}

        /// <summary>
        /// Determines how the particle fade out
        /// </summary>
        public FadeMode ParticleFadeOut { get { return this.particlefadeOut; } set { this.particlefadeOut = value; } }


        public float ParticleStartingAlpha { get { return this.particleStartingAlpha; } set { this.particleStartingAlpha = value; } }

        public float ParticleGrowthRate { get { return this.particleGrowthRate; } set { this.particleGrowthRate = value; } }
        /// <summary>
        /// Freeze particles
        /// </summary>
        public bool Pause { get { return pause; } set { pause = value; } }

        /// <summary>
        /// Only an active Emitter emitts Particles
        /// </summary>
        public bool Active { get { return this.active; } set { this.active = value;} }

        /// <summary>
        /// Shape of the emitter
        /// </summary>
        public EmitterShape EmitterShape { get { return this.emitterShape; } set { this.emitterShape = value; } }

        /// <summary>
        /// Texture to be displayed at the Particle Position
        /// </summary>
        public Texture2D ParticleTexture { get; set; }

        /// <summary>
        /// Color of the Particles
        /// </summary>
        public Color ParticleColor { get { return particleColor; } set { particleColor = value; } }

        /// <summary>
        /// Forces with effect just in this Emitter!
        /// </summary>
        public List<Vector2> EmitterForcesList { get { return this.emitterForcesList; } set { this.emitterForcesList = value; } }

        /// <summary>
        /// Number of Particles connected to this Emitter
        /// </summary>
        public int NumberOfParticles { get { return this.numberOfParticles; } set { this.numberOfParticles = value; } }

        /// <summary>
        /// Time before a Particle gets killed
        /// </summary>
        public float ParticleLifeTime { get { return this.particleLifeTime; } set { this.particleLifeTime = value; } }

        public Vector2 ParticleSize { get { return this.particleSize; } set { this.particleSize = value; } }
        public float InitialParticleSpeed { get { return this.initialParticleSpeed; } set { this.initialParticleSpeed = value;} }

        public ParticleSystem ParentParticleSystem { get { return this.parentSystem; } set { this.parentSystem = value; } }

        /// <summary>
        /// Particles per second
        /// </summary>
        public float ParticlesPerSecond
        {
            get { return particlesPerSecond; }
            set { particlesPerSecond = value; }
        }

        /// <summary>
        /// True if the emitter rotates around the origin with its offset.
        /// </summary>
        public bool RotateWithOffset
        {
            get { return rotateWithOffset; }
            set { rotateWithOffset = value; }
        }

        /// <summary>
        /// If true, the position of the emitter is adjusted depending on the speed of the linked object.
        /// </summary>
        public bool IncludeMovingOffset
        {
            get { return includeMovingOffset; }
            set { includeMovingOffset = value; }
        }

        public bool Visible { get; set; }

        /// <summary>
        /// If set, the emitting duration will be a random number between x and y values of the vector.
        /// Z and W values represent the duration between 2 emits.
        /// The emit will repeat endlessly.
        /// 
        /// Values are in milliseconds.
        /// </summary>
        public Vector4 RandomEmitDuration
        {
            get { return randomEmitDuration; }
            set { randomEmitDuration = value;
            emittingDuration = rng.Next((int)randomEmitDuration.X, (int)randomEmitDuration.Y) + (float)rng.NextDouble();
            }
        }

        /// <summary>
        /// If true, the particles will move with the emitter so that there is no gap with increasing speed of the emitter.
        /// </summary>
        public bool MovingParticles
        {
            get { return movingParticles; }
            set { movingParticles = value; }
        }

        /// <summary>
        /// Return the vector between the last position of the emitter and the current position.
        /// </summary>
        public Vector2 MovingOffset { get { return movingOffset; } }

        #endregion

        #region Initialization

        
        /// <summary>
        /// Create a new Emitter.
        /// </summary>
        /// <param name="position">Position of the Emitter</param>
        /// <param name="numberOfParticles">Total number of Particles</param>
        /// <param name="particlesPerSecond">Emitted Partikels per Second</param>
        protected Emitter(Vector2 position, int numberOfParticles, float particlesPerSecond)
        {            
            this.lastPosition = position;
            this.position = position;
            this.ParticleColor = Color.White;
            this.particleSize = new Vector2(0.5f, 0.5f);
            this.numberOfParticles = numberOfParticles;
            this.particlesPerSecond = particlesPerSecond;
            this.initialParticleSpeed = 5.0f;
            this.particleLifeTime = 2.0f;
            this.particleList = new List<Particle>();
            this.emitterForcesList = new List<Vector2>();
            this.particleGrowthRate = 1.0f;
            this.ParticleGrowth = GrowthMode.None;
            this.ParticleFadeOut = FadeMode.None;
            this.totalEmittingTime = 0;
            this.active = true;
            this.particleStartingAlpha = 1.0f;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Update the Emitter and all Particles it owns
        /// </summary>
        /// <param name="time">GameTime</param>
        /// <param name="forces">VektorList of forces which are applied on the Partikels</param>
        public void update(GameTime time, List<Vector2> forces)
        {
            //don't update Particles when it's paused;
            if (this.pause) return;

            movingOffset = position;

            //Update Emitter Position when it's linked to an Drawable Object
            if (linkedObject != null)
            {
                position = linkedObject.Position;
                //Set Position to center of Object
                position += (linkedObject.Size / 2f);

                if (includeMovingOffset)
                {
                    //Add offset to the position
                    Vector2 offset = linkedObject.Position - lastPosition;
                    lastPosition = linkedObject.Position;

                    if (offset != Vector2.Zero)
                        offset.Normalize();

                    position += (linkedObjectOffset * offset);
                }
                else
                {
                    lastPosition = linkedObject.Position;
                }
                if (rotateWithOffset)
                {
                    position += Vector2.Transform(linkedObjectOffset, Matrix.CreateRotationZ(linkedObject.Rotation));
                }
                else
                {
                    position += linkedObjectOffset;
                }
            }

            movingOffset -= position;
            movingOffset *= -1;
            

            if (emittingDuration != 0)
            {
                totalEmittingTime += (float)time.ElapsedGameTime.TotalSeconds;
                if (totalEmittingTime >= emittingDuration)
                {                    
                    milliSecondsSinceLastEmit = 0;
                    totalEmittingTime = 0;

                    if (randomEmitDuration != Vector4.Zero)
                    {
                        if (active)
                        {
                            emittingDuration = rng.Next((int)randomEmitDuration.Z, (int)randomEmitDuration.W) + (float)rng.NextDouble();
                        }
                        else
                        {
                            emittingDuration = rng.Next((int)randomEmitDuration.X, (int)randomEmitDuration.Y) + (float)rng.NextDouble();
                        }
                        active = !active;
                    }
                    else
                    {                        
                        active = false;
                    }
                }
            }

            int numberEmitParticles = 0;
            if (active)
            {
                //Update the Time since the Emitter emitted for the last time
                milliSecondsSinceLastEmit += (float)time.ElapsedGameTime.TotalMilliseconds;

                //Calculate the number of Particles we have to emit
                numberEmitParticles = (int)((milliSecondsSinceLastEmit / 1000.0f) * particlesPerSecond);

                //Put the rest of the time back, so we don't lose Particles
                milliSecondsSinceLastEmit = milliSecondsSinceLastEmit - particlesPerSecond / 1000.0f * numberEmitParticles;
            }

            //Update Particles  
            int count = 0;
            foreach (Particle p in this.particleList)
            {
                //Only Update Particles that are visible
                if (p.Visible == true)
                {
                    count++;

                    //Set current Age of the Particle
                    p.Age += (float)time.ElapsedGameTime.TotalSeconds;

                    //Check if Particle is dead
                    if (p.Age >= p.LifeTime)
                    {
                        //If Particle is dead => hide it
                        //    Logger.Instance.log(Sender.ParticleSystem, "Particle Killed", PriorityLevel.Priority_5);
                        p.Visible = false;
                    }
                    else //Update Particle
                    {
                        //Apply Forces
                        foreach (Vector2 force in this.parentSystem.Forces)
                        {
                            p.Veclocity += force * (float)time.ElapsedGameTime.TotalSeconds;
                        }
                        foreach (Vector2 force in this.EmitterForcesList)
                        {
                            p.Veclocity += force * (float)time.ElapsedGameTime.TotalSeconds;
                        }
                        //Move Particle

                        p.Position += p.Veclocity * (float)time.ElapsedGameTime.TotalSeconds;
                        if (movingParticles)
                            p.Position += movingOffset;

                        if (this.ParticleFadeOut == FadeMode.Linear)
                        {
                            p.Alpha = this.particleStartingAlpha - this.particleStartingAlpha * (p.Age / p.LifeTime);
                        }
                        else if (this.ParticleFadeOut == FadeMode.Quadratic)
                        {
                            p.Alpha = this.particleStartingAlpha - this.particleStartingAlpha * (p.Age / p.LifeTime) * (p.Age / p.LifeTime);
                        }

                        //Let Particles Grow
                        if (this.ParticleGrowth == GrowthMode.Linear)
                        {
                            p.Size = this.particleSize * (1 + (p.Age / p.LifeTime) * this.particleGrowthRate);
                        }
                        else if (this.ParticleGrowth == GrowthMode.Quadratic)
                        {
                            p.Size = this.particleSize * (float) Math.Pow((1 + (p.Age / p.LifeTime) * this.particleGrowthRate) * (1 + (p.Age / p.LifeTime) * this.particleGrowthRate),2);
                        }
                    }
                }
                else
                {
                    //We found a hidden Particle. Can we emit it again?
                    if (active && numberEmitParticles > 0)
                    {
                        
                        //Logger.Instance.log(Sender.ParticleSystem, "Emitted Particle", PriorityLevel.Priority_5);
                        numberEmitParticles--;
                        p.Visible = true;
                        p.Age = 0; //reborn
                        p.Veclocity = getStartVelocity();
                        p.Size = this.particleSize;
                        p.Position = this.Position + getStartingPosition();                        
                        p.Rotation = (float) rng.NextDouble();;
                        milliSecondsSinceLastEmit = 0;
                    }
                }
            }

            //set visibility of the emitter if particles are visible. this safes many foreach loops in graphics
            Visible = (count != 0) ? true : false;
        }

        /// <summary>
        /// Create all the new Particles
        /// </summary>
        public void initializeParticles()
        {
            this.particleList.Clear();
            for (int i = 0; i < numberOfParticles; i++)
            {
                Particle p = new Particle(this.Position + getStartingPosition(), this.particleLifeTime, (float) rng.NextDouble());
                p.Color = this.ParticleColor;
                p.Alpha = this.particleStartingAlpha;
                this.particleList.Add(p);
            }
        }

        /// <summary>
        /// Create all the new Particles
        /// </summary>
        public void initializeParticles(bool dirAsRot)
        {
            this.particleList.Clear();
            for (int i = 0; i < numberOfParticles; i++)
            {
                Particle p = new Particle(this.Position + getStartingPosition(), this.particleLifeTime, (float)rng.NextDouble());
                p.DirAsRot = true;
                p.Color = this.ParticleColor;
                p.Alpha = this.particleStartingAlpha;
                this.particleList.Add(p);
            }
        }

        /// <summary>
        /// Changes the amount of particles we have.
        /// </summary>
        /// <param name="count"></param>
        public void changeNumberOfParticles(int count)
        {
            if (numberOfParticles == count) return;
            numberOfParticles = count;
            initializeParticles();
        }


        /// <summary>
        /// Create a Start Velocity from the Emitter Properties
        /// Should be overridden in the different Emitter implementations
        /// </summary>
        /// <returns>Velocity</returns>
        abstract protected Vector2 getStartVelocity();

        /// <summary>
        /// Set The Starting Position to a random Point within the shape
        /// </summary>
        /// <returns>Random Point within shape</returns>
        private Vector2 getStartingPosition()
        {
            return this.emitterShape.randomPositionWithinShape();
        }

        #endregion
    }
}
