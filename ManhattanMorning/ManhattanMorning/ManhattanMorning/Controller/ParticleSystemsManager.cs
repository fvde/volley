using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManhattanMorning.Model.ParticleSystem;
using ManhattanMorning.Misc;
using ManhattanMorning.Model;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Misc.Tasks;

using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;

namespace ManhattanMorning.Controller
{
    /// <summary>
    /// Manages and updates all Particle Systems
    /// </summary>
    class ParticleSystemsManager : IController
    {
        #region Members

        /// <summary>
        /// Some ParticleSystems.
        /// </summary>
        private ParticleSystem sandSystem;
        private ParticleSystem ballCollision;
        private ParticleSystem[] bombSystem; //holds emitters for fire, smoke and splash effects
        private ParticleSystem[] sparclingSystem;
        private ParticleSystem superbombExplosionSystem; //holds emitters for smoke and fire
        private ParticleSystem[] lavaExplosionSystem; //holds emitters for smoke and fire
        private ParticleSystem[] windSystem;
        private ParticleSystem meteorBallSystem;
        private ParticleSystem specialbarHighlight;
        private ParticleSystem highlightJumpSystem;
        private ParticleSystem[] orbitterPlayers; //all orbitters for the player. paralysis is 0th, stun is 1st element
        private ParticleSystem sandStormSystem;
        
        /// <summary>
        /// Are all Particle Systems Paused?
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// List of all Particle Systems
        /// </summary>
        private List<ParticleSystem> particleSystems;

        /// <summary>
        /// Current Instance of this Manager
        /// </summary>
        private static ParticleSystemsManager instance = null;

        /// <summary>
        /// Indicates which particle system has to be used to display a ball collision.
        /// </summary>
        private int ballCollisionCount;

        /// <summary>
        /// Indicates which particle system has to be used to display a bomb falling.
        /// </summary>
        private int bombCount;

        /// <summary>
        /// Indicates which particle system has to be used to display a superbomb sparcling.
        /// </summary>
        private int superbombCount;

        /// <summary>
        /// Indicates which particle system has to be used to display a bomb exploding.
        /// </summary>
        private int bombExplosionCount;

        /// <summary>
        /// Meter to pixel factor.
        /// </summary>
        private float metertopixel;

        /// <summary>
        /// THe size of the current screen. Used for percentage conversions.
        /// </summary>
        private Vector2 viewportSize;

        #endregion

        #region Properties
        /// <summary>
        /// Get the current Instance of the ParticleSystemsManager
        /// </summary>
        public static ParticleSystemsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ParticleSystemsManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// List of all ParticleSystems
        /// </summary>
        public List<ParticleSystem> ParticleSystems { get { return this.particleSystems;} }
        
        /// <summary>
        /// Are all Particle Systems Paused?
        /// </summary>
        public bool IsPaused { get { return this.isPaused; } set { this.isPaused = value; } }
        #endregion

        #region Initialization

        /// <summary>
        /// Create a new Manager
        /// </summary>
        private ParticleSystemsManager()
        {
            // All initialization moved to reset
            clear();
        }

        #endregion

        #region Methods

        #region Create Emitters

        private Emitter createDustEmitter(Vector2 pos)
        {
            EmitterFountain pointShape = new EmitterFountain(pos, 141, 35);
            pointShape.EmitterShape = new EmitterPointShape();
            pointShape.EmitterDirection = new Vector2(0, 1f);
            pointShape.EmitterAngle = 0.1f;
            pointShape.ParticleLifeTime = 3.0f;
            pointShape.ParticleFadeOut = FadeMode.Linear;
            pointShape.ParticleSize = new Vector2(0.12f, 0.12f);
            pointShape.InitialParticleSpeed = 0.20f;
            pointShape.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/Particle005");
            pointShape.ParticleColor = Color.Beige;
            pointShape.initializeParticles();
            pointShape.Active = true;
            pointShape.Pause = false;
            pointShape.EmittingDuration = 0f;

            return pointShape;
        }

        public void activateSandStorm(int direction)
        {
            Vector2 pos;
            if (direction < 0)
            {
                direction = -1;
                pos = new Vector2(13.0f, 7.0f);
            }
            else
            {
                direction = 1;
                pos = new Vector2(-1.0f, 7.0f);
            }
            foreach (Emitter e in sandStormSystem.EmitterList)
            {
                e.Active = true;
                e.Position = pos;
                ((EmitterFountain)e).EmitterDirection = new Vector2((float)direction, 0);
            }
            
        }

        public void deactivateSandStorm()
        {
            foreach (Emitter e in sandStormSystem.EmitterList)
            {
                e.Active = false;
            }
        }

        private ParticleSystem createSandStormSystem()
        {
            EmitterFountain e = new EmitterFountain(new Vector2(-1.0f, 7.0f), 800, 100);
            e.EmitterShape = new EmitterRectangleShape(0.1f, 1.50f, new Vector2(0, 1));
            e.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/particle1");
            e.ParticleColor = Color.Beige;
            e.ParticleFadeOut = FadeMode.None;
            e.ParticleSize = new Vector2(1.0f, 1.0f);
            e.Active = false;
            e.InitialParticleSpeed = 3.8f;
            e.EmitterDirection = Vector2.UnitX;
            e.EmitterAngle = 0.05f;
            e.ParticleLifeTime = 7.0f;
            e.ParticleStartingAlpha = 0.1f;
            e.initializeParticles(true);


            ParticleSystem p = new ParticleSystem(255);
            p.SystemBlendState = BlendState.AlphaBlend;
           
            p.addEmitter(e);

            return p;
        }
           


        /// <summary>
        /// Creates a vulcano particle system for beach level with lava and smoke.
        /// </summary>
        /// <param name="pos">The position where you want to create the system.</param>
        /// <returns>The particle system holding all emitters.</returns>
        private ParticleSystem createVulcanoSystem(Vector2 pos)
        {
            //lava
            EmitterFountain e = new EmitterFountain(pos, 23, 15f);
            e.EmitterShape = new EmitterPointShape();
            e.EmitterForcesList.Add(Vector2.UnitY*4);
            e.EmitterDirection = -Vector2.UnitY;
            e.ParticleColor = Color.Red;
            e.EmitterAngle = 0.45f;
            e.ParticleLifeTime = 1.5f;
            e.ParticleFadeOut = FadeMode.None;
            e.ParticleSize = new Vector2(0.07f, 0.07f);
            e.InitialParticleSpeed = 3.8f;
            e.InitialParticleVelocityVariance = new Vector2(2.9f, 3.9f);
            e.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/particle002");
            e.initializeParticles(true);
            e.Active = false;
            e.RandomEmitDuration = new Vector4(1f, 4f, 2f, 5f);
            
            ParticleSystem p = new ParticleSystem(7);
            p.SystemBlendState = BlendState.AlphaBlend;
            //smoke
            p.addEmitter(createSmokeEmitter(new Vector2(0.9f), pos, -Vector2.UnitY));
            p.addEmitter(e);            

            return p;
        }

        /// <summary>
        /// Creates emitter for highlighting special jump.
        /// </summary>
        /// <returns>Created Emitter./returns>
        private Emitter createJumpEmitter()
        {
            EmitterFountain e = new EmitterFountain(Vector2.Zero, 29, 14);
            e.EmitterShape = new EmitterRectangleShape(0.7f, 0.1f, -Vector2.UnitY);
            e.EmitterDirection = new Vector2(0, 1f);
            e.ParticleLifeTime = 0.5f;
            e.ParticleFadeOut = FadeMode.Quadratic;
            e.ParticleSize = new Vector2(0.10f, 0.10f);
            e.InitialParticleSpeed = 0.95f;
            e.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/Particle1");
            e.ParticleColor = Color.DarkGreen;
            e.EmittingDuration = 0f;
            e.EmitterAngle = 0.5f;
            e.initializeParticles();
            e.LinkedObjectOffset = new Vector2(0f, 0.3f);
            e.Active = false;
            e.Pause = true;

            return e;
        }

        /// <summary>
        /// Creates a new smoke Emitter for fast traveling objects.
        /// </summary>
        /// <param name="size">Size of the Particles.</param>
        /// <returns>The created Emitter.</returns>
        private Emitter createSmokeEmitterForTrail(Vector2 particleSize)
        {
            //Create new Emitter and Initialize Particles
            EmitterFountain e = new EmitterFountain(Vector2.Zero, 16, 40f);
            e.EmitterShape = new EmitterPointShape();
            e.IncludeMovingOffset = true;
            e.ParticleLifeTime = 0.3f;
            e.ParticleFadeOut = FadeMode.Linear;
            e.ParticleGrowth = GrowthMode.Linear;
            e.ParticleGrowthRate = 0.5f;
            e.ParticleSize = particleSize;
            e.LinkedObjectOffset = new Vector2(0.2f, 0.2f);
            e.InitialParticleSpeed = 0.3f;
            e.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/smoke_particle");
            e.ParticleColor = Color.White;
            e.initializeParticles();
            e.Active = true;
            e.Pause = true;
            return e;
        }

        /// <summary>
        /// Creates a new sparcling emitter for superbomb.
        /// </summary>
        /// <param name="size">Size of the Particles.</param>
        /// <returns>The created Emitter.</returns>
        private Emitter createSparclingEmitterForSuperbomb(Vector2 particleSize)
        {
            //Create new Emitter and Initialize Particles
            EmitterExplosion e = new EmitterExplosion(Vector2.Zero, 16, 30f);
            e.EmitterShape = new EmitterPointShape();
            e.ParticleLifeTime = 0.25f;
            e.ParticleGrowth = GrowthMode.Linear;
            e.ParticleGrowthRate = 1.7f;
            e.ParticleSize = particleSize;
            e.InitialParticleSpeed = 1.65f;
            e.MovingParticles = true;
            e.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/sparcles");
            e.ParticleColor = Color.Yellow;
            e.initializeParticles();
            e.Active = true;
            e.Pause = true;
            return e;
        }

        /// <summary>
        /// Creates a new smoke Emitter which emitts smoke in a given direction.
        /// </summary>
        /// <param name="size">Size of the Particles.</param>
        /// <returns>The created Emitter.</returns>
        private Emitter createSmokeEmitter(Vector2 particleSize, Vector2 position, Vector2 emitterDirection)
        {
            //Create new Emitter and Initialize Particles
            EmitterFountain e = new EmitterFountain(position, 16, 1f);
            e.EmitterShape = new EmitterPointShape();
            e.IncludeMovingOffset = true;
            e.ParticleLifeTime = 18f;
            e.ParticleFadeOut = FadeMode.Linear;
            e.ParticleGrowth = GrowthMode.Linear;
            e.ParticleGrowthRate = 1.4f;
            e.ParticleSize = particleSize;
            e.InitialParticleSpeed = 0.2f;
            e.EmitterAngle = 0.30f;
            e.EmitterDirection = emitterDirection;
            e.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/smoke_particle");
            e.ParticleColor = Color.White;
            e.initializeParticles();
            e.Active = true;
            return e;
        }
        
        /// <summary>
        /// Creates a new fire Emitter.
        /// </summary>
        /// /// <param name="size">Size of the Particles.</param>
        /// <param name="offset">Offset from center of the Emitter in meter.</param>        
        /// <param name="texture">Texture of the Particles.</param>
        /// <returns>The created Emitter.</returns>
        private Emitter createFireEmitter(Vector2 size, Vector2 offset, String texture)
        {
            //Create new Emitter and Initialize Particles
            EmitterFountain e = new EmitterFountain(Vector2.Zero, 16, 50);
            e.IncludeMovingOffset = true;
            e.EmitterAngle = 0.3f;
            e.EmitterShape = new EmitterPointShape();
            e.ParticleLifeTime = 0.2f;
            e.ParticleFadeOut = FadeMode.Linear;
            e.ParticleSize = size;
            e.LinkedObjectOffset = offset;
            e.InitialParticleSpeed = 0.3f;
            e.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/"+texture);
            e.ParticleColor = Color.OrangeRed;
            e.initializeParticles();
            e.Active = true;
            e.Pause = true;
            return e;
        }
        
        /// <summary>
        /// Create a new slow explosion Emitter.
        /// </summary>
        /// <param name="size">Size of the Particles.</param>
        /// <param name="texturePath">Path of the particle texture that you want to use.</param>
        /// <param name="tinting">Color of the particles.</param>
        /// <returns></returns>
        private Emitter createExplosionEmitter(Vector2 size, String texturePath, Color tinting)
        {
            //Create new Emitter and Initialize Particles
            EmitterExplosion explosion = new EmitterExplosion(new Vector2(6f, 5.0f), 16, 30);
            explosion.EmitterShape = new EmitterPointShape();
            explosion.ParticleLifeTime = 0.4f;
            explosion.EmittingDuration = 0.2f;
            explosion.ParticleFadeOut = FadeMode.Quadratic;
            explosion.ParticleSize = size;
            explosion.InitialParticleSpeed = 0.8f;
            explosion.EmitterForcesList.Add(new Vector2(0f, 0.5f));
            explosion.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/" + texturePath);
            explosion.ParticleColor = tinting;
            explosion.initializeParticles();
            explosion.Active = false;
            explosion.Pause = true;
            return explosion;
        }

        /// <summary>
        /// Create a new Explosion Emitter.
        /// </summary>
        /// <param name="size">Size of the Particles.</param>
        /// <param name="texturePath">Path of the particle texture that you want to use.</param>
        /// <param name="tinting">Color of the particles.</param>
        /// <param name="speed">Emitting velocity</param>
        /// <param name="particles">Particle count</param>
        /// <returns></returns>
        private Emitter createExplosionEmitter(Vector2 size, String texturePath, Color tinting, float speed, int particles, float lifeTime)
        {
            //Create new Emitter and Initialize Particles
            EmitterExplosion explosion = new EmitterExplosion(Vector2.Zero, particles, particles / 0.15f);
            explosion.EmitterShape = new EmitterPointShape();
            explosion.ParticleLifeTime = lifeTime;
            explosion.EmittingDuration = 0.15f;
            explosion.ParticleFadeOut = FadeMode.Linear;
            explosion.ParticleGrowth = GrowthMode.Linear;
            explosion.ParticleGrowthRate = 0.3f;
            explosion.ParticleSize = size;
            explosion.InitialParticleSpeed = speed;
            explosion.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/" + texturePath);
            explosion.ParticleColor = tinting;
            explosion.initializeParticles();
            explosion.Active = false;
            explosion.Pause = true;
            return explosion;
        }

        /// <summary>
        /// Create a new Explosion Emitter.
        /// </summary>
        /// <param name="size">Size of the Particles.</param>
        /// <param name="texturePath">Path of the particle texture that you want to use.</param>
        /// <param name="tinting">Color of the particles.</param>
        /// <param name="speed">Emitting velocity</param>
        /// <param name="particles">Particle count</param>
        /// <param name="dirAsRot">Use emmiting direction as rotation</param>
        /// <returns></returns>
        private Emitter createExplosionEmitter(Vector2 size, String texturePath, Color tinting, float speed, int particles, bool dirAsRot, Vector2 force, float lifeTime)
        {
            //Create new Emitter and Initialize Particles
            EmitterExplosion explosion = new EmitterExplosion(Vector2.Zero, particles, particles / 0.15f);
            explosion.EmitterShape = new EmitterPointShape();
            explosion.EmitterForcesList.Add(force);
            explosion.ParticleLifeTime = lifeTime;
            explosion.EmittingDuration = 0.15f;
            explosion.ParticleFadeOut = FadeMode.Linear;
            explosion.ParticleGrowth = GrowthMode.Linear;
            explosion.ParticleGrowthRate = 0.3f;
            explosion.ParticleSize = size;
            explosion.InitialParticleSpeed = speed;
            explosion.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/" + texturePath);
            explosion.ParticleColor = tinting;
            explosion.initializeParticles(dirAsRot);
            explosion.Active = false;
            explosion.Pause = true;
            return explosion;
        }

        /// <summary>
        /// Creates a new Rectangle Emitter.
        /// </summary>
        /// <param name="texturePath">Path of the particle texture that you want to use.</param>
        /// <returns></returns>
        private Emitter createFallingParticlesEmitter(Vector2 pos, Vector2 size, Vector2 dir, String texturePath, float particlesPerSec)
        {
            //Create new Emitter and Initialize Particles
            EmitterFountain rectangle = new EmitterFountain(pos, 42, particlesPerSec);
            rectangle.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/" + texturePath);
            rectangle.EmitterShape = new EmitterRectangleShape(size.X, size.Y, -Vector2.UnitY);
            rectangle.EmitterDirection = dir;
            rectangle.ParticleLifeTime = 14f;
            rectangle.EmittingDuration = 0f;
            rectangle.ParticleFadeOut = FadeMode.None;
            rectangle.ParticleSize = new Vector2(0.25f, 0.25f);
            rectangle.InitialParticleSpeed = 1.1f;
            rectangle.ParticleColor = Color.White;
            rectangle.initializeParticles();
            rectangle.Active = true;
            rectangle.Pause = true;
            return rectangle;
        }
        
        /// <summary>
        /// Create a new SandFountain Emitter
        /// </summary>
        /// <returns></returns>
        private Emitter createSandFountainEmitter()
        {
            //Create new Emitter and Initialize Particles
            EmitterFountain explosion = new EmitterFountain(new Vector2(6f, 5.0f), 20, 30);
            explosion.EmitterShape = new EmitterPointShape();
            explosion.ParticleLifeTime = 0.5f;
            explosion.EmittingDuration = 0.2f;
            explosion.EmitterAngle = 5.0f;
            explosion.ParticleFadeOut = FadeMode.Quadratic;
            explosion.ParticleSize = new Vector2(0.3f, 0.3f);
            explosion.InitialParticleSpeed = 0.8f;
            explosion.EmitterForcesList.Add(new Vector2(0f, 0.5f));
            explosion.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/smoke_particle");
            explosion.ParticleColor = Color.SandyBrown;
            explosion.initializeParticles();
            explosion.Active = false;
            explosion.Pause = true;
            return explosion;
        }

        /// <summary>
        /// Creates a elliptical group of orbitting stars for illustrating paralysis of player.
        /// </summary>
        /// <param name="texture">Texture for the Particles.</param>
        /// <param name="particleSize">Particle size</param>
        /// <param name="lifetime">Lifetime of the longest lifing particle</param>
        /// <param name="timeOffset">Time offset between each particle.</param>
        /// <returns>a new Orbitter</returns>
        private Orbitter createParalysisOrbitter(String texture, Vector2 particleSize, float lifetime, float timeOffset, Color color)
        {
            Orbitter starOrb = new Orbitter(new Vector2(2f, 2f), lifetime, timeOffset);
            starOrb.Color = color;
            starOrb.ParticleCount = 7;
            starOrb.Height = 0.08f;
            starOrb.Width = 0.5f;
            starOrb.ParticleSize = particleSize;
            starOrb.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@texture);
            starOrb.ParticleSpeed = 0.5f;

            foreach (Particle p in starOrb.getParticles())
            {
                p.Rotation = 0f;
            }

            return starOrb;
        }

        /// <summary>
        /// Create a new specialbar Emitter.
        /// </summary>
        /// <returns></returns>
        private Emitter createSpecialbarEmitter()
        {
            //Create new Emitter and Initialize Particles
            EmitterExplosion emitter = new EmitterExplosion(Vector2.Zero, 20, 3);
            emitter.EmitterShape = new EmitterRectangleShape(0, 0, Vector2.UnitY);
            emitter.ParticleLifeTime = 0.9f;
            emitter.EmittingDuration = 0f;
            emitter.ParticleFadeOut = FadeMode.Linear;
            emitter.ParticleGrowth = GrowthMode.Linear;
            emitter.ParticleGrowthRate = 1f;
            emitter.ParticleSize = new Vector2(0.13f, 0.13f);
            emitter.InitialParticleSpeed = 0.2f;
            emitter.ParticleTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures/Particles/star_particle");
            emitter.ParticleColor = Color.Orange;
            emitter.initializeParticles();
            emitter.Active = true;
            emitter.Pause = true;
            return emitter;
        }

        #endregion

        #region Stop and Play

        /// <summary>
        /// Adds a smoke emitter to a given object. The emitter is linked to the object and moves
        /// when the object moves. The emitter disables itself when the object doesn't move anymore.
        /// </summary>
        /// <param name="ball">The object the emitter will be linked to.</param>
        public void playMeteorBall(DrawableObject ball)
        {
            foreach (Emitter e in meteorBallSystem.EmitterList)
            {
                e.LinkedObjectOffset = ball.Size / 4.5f; //little bit smaller than half the radius
                e.LinkedObject = ball;
                e.EmittingDuration = 0f;
                e.Active = true;
                e.Pause = false;
            }
        }

        /// <summary>
        /// Stops the all emitters on the ball which are responsible for smoke and fire.
        /// </summary>
        /// <param name="ball">The ball which is affected.</param>
        public void stopMeteorBall(DrawableObject ball)
        {
            foreach (Emitter e in meteorBallSystem.EmitterList)
            {
                //do nothing if this ball doesn't match the one linked in the emitter
                if (ball != e.LinkedObject) return;

                e.LinkedObject = null;
                e.EmittingDuration = float.Epsilon;
            }
        }

        /// <summary>
        /// Starts the wind system with falling particles.
        /// </summary>
        public void playFallingParticles()
        {
            foreach (ParticleSystem p in windSystem)
            {
                foreach (EmitterFountain e in p.EmitterList)
                {
                    e.Pause = false;
                }
            }
        }

        /// <summary>
        /// Stops the wind system with falling particles.
        /// </summary>
        public void stopFallingParticles()
        {
            foreach (ParticleSystem p in windSystem)
            {
                foreach (EmitterFountain e in p.EmitterList)
                {
                    e.EmittingDuration = float.Epsilon;
                    e.Active = false;
                }
            }
        }

        /// <summary>
        /// Shows a bomb effect when the bomb is falling.
        /// </summary>
        /// <param name="bomb">The object where the ParticleSystem is attached to.</param>
        public void playBombFalling(DrawableObject bomb)
        {
            ParticleSystem p = bombSystem[bombCount];
            foreach (Emitter e in p.EmitterList)
            {
                e.LinkedObjectOffset = Vector2.Zero;
                e.LinkedObject = bomb;
                e.Active = true;
                e.Pause = false;
                e.EmittingDuration = 0f;
                e.RotateWithOffset = false;
            }

            bombCount = (bombCount + 1) % 6;
        }

        /// <summary>
        /// Shows a bomb effect when the bomb is falling with an offset to the middle of the bomb.
        /// </summary>
        /// <param name="bomb">The object where the ParticleSystem is attached to.</param>
        /// <param name="offset">Offset to the middle of the bomb.</param>
        /// <param name="objectMovingFast">True if the emitter is attached to a fast moving object.</param>
        public void playBombFalling(DrawableObject bomb, Vector2 offset, bool objectMovingFast)
        {
            ParticleSystem p = bombSystem[bombCount];
            foreach (Emitter e in p.EmitterList)
            {
                e.LinkedObjectOffset = offset;
                e.LinkedObject = bomb;
                e.Active = true;
                e.Pause = false;
                e.IncludeMovingOffset = objectMovingFast;
                e.EmittingDuration = 0f;
                e.RotateWithOffset = true;
            }

            bombCount = (bombCount + 1) % 6;
        }

        /// <summary>
        /// Shows a bomb effect when the bomb is falling with an offset to the middle of the bomb.
        /// </summary>
        /// <param name="bomb">The object where the ParticleSystem is attached to.</param>
        /// <param name="offset">Offset to the middle of the bomb.</param>
        /// <param name="objectMovingFast">True if the emitter is attached to a fast moving object.</param>
        public void playSparclingBomb(DrawableObject bomb, Vector2 offset, bool objectMovingFast)
        {
            ParticleSystem p = sparclingSystem[superbombCount];
            foreach (Emitter e in p.EmitterList)
            {
                e.LinkedObjectOffset = offset;
                e.LinkedObject = bomb;
                e.Active = true;
                e.Pause = false;
                e.EmittingDuration = 0f;
                e.RotateWithOffset = true;
            }

            superbombCount = (superbombCount + 1) % 2;
        }

        /// <summary>
        /// Stops the effect.
        /// </summary>
        /// <param name="bomb">The object where the ParticleSystem is attached to.</param>
        public void stopBombFalling(DrawableObject bomb)
        {
            foreach (ParticleSystem p in bombSystem)
            {
                if (p.EmitterList.First().LinkedObject == bomb)
                {
                    foreach (Emitter e in p.EmitterList)
                    {
                        e.EmittingDuration = float.Epsilon;
                    }
                }
            }
        }

        /// <summary>
        /// Stops the effect.
        /// </summary>
        /// <param name="bomb">The object where the ParticleSystem is attached to.</param>
        public void stopSparcle(DrawableObject bomb)
        {
            foreach (ParticleSystem p in sparclingSystem)
            {
                if (p.EmitterList.First().LinkedObject == bomb)
                {
                    foreach (Emitter e in p.EmitterList)
                    {
                        e.EmittingDuration = float.Epsilon;
                    }
                }
            }
        }

        /// <summary>
        /// Shows a bomb effect when the bomb is falling.
        /// </summary>
        /// <param name="pos">The position of the effect.</param>
        public void playBombExplosion(Vector2 pos, Vector2 size, bool superBomb)
        {
            ParticleSystem p = (superBomb) ? superbombExplosionSystem : lavaExplosionSystem[bombExplosionCount];
            Light l = new Light("explL", StorageManager.Instance.getTextureByName("light"), size, pos - size / 2, Color.LightYellow, true, null);
            ScalingAnimation scaling = new ScalingAnimation(false, false, 0, true, 500);
            FadingAnimation fading = new FadingAnimation(false, true, 50, true, 225);
            scaling.ScalingRange = new Vector2(0.1f, 1f);
            l.ScalingAnimation = scaling;
            l.FadingAnimation = fading;
            SuperController.Instance.addGameObjectToGameInstance(l);
            TaskManager.Instance.addTask(new GameLogicTask(500, l));

            foreach (Emitter e in p.EmitterList)
            {
                e.Position = pos;
                e.Active = true;
                e.Pause = false;
            }

            bombExplosionCount = (bombExplosionCount + 1) % 3;
        }

        /// <summary>
        /// Changes the applied force for the falling particles.
        /// </summary>
        /// <param name="forceDirection">The force to apply.</param>
        public void applyForceFallingParticles(Vector2 forceDirection)
        {
            bool affectAllEmitters = true;
            if (SuperController.Instance.GameInstance.LevelName == "Beach")
            {
                forceDirection /= 2.2f;
                affectAllEmitters = false;
            }

            if (windSystem == null) return;
            foreach (ParticleSystem p in windSystem)
            {
                foreach (EmitterFountain e in p.EmitterList)
                {
                    e.EmitterForcesList.Clear();
                    e.EmitterForcesList.Add(e.InitialParticleSpeed / 1.1f * forceDirection);
                    if (!affectAllEmitters) return;
                }
            }
        }

        /// <summary>
        /// Clears all forces in the list that modify the particles position.
        /// </summary>
        public void removeForceFallingParticles()
        {
            bool affectAllEmitters = true;
            if (SuperController.Instance.GameInstance.LevelName == "Beach")
                affectAllEmitters = false;

            if (windSystem == null) return;
            foreach (ParticleSystem p in windSystem)
            {
                foreach (EmitterFountain e in p.EmitterList)
                {
                    e.EmitterForcesList.Clear();
                    if (!affectAllEmitters) return;
                }
            }
        }

        /// <summary>
        /// Shows a ball collision at the given Position.
        /// </summary>
        /// <param name="pos">Position where the collision happened.</param>
        public void playBallCollision(Vector2 pos)
        {
            EmitterExplosion e = ballCollision.EmitterList.ElementAt<Emitter>(ballCollisionCount) as EmitterExplosion;
            e.Position = pos;
            e.Pause = false;
            e.Active = true;

            ballCollisionCount = (ballCollisionCount + 1) % 3;
        }
        
        /// <summary>
        /// Shows a Sandfountain at the given Position
        /// </summary>
        /// <param name="pos">Position of the Fountain</param>
        /// <param name="num">Number of the Fountain</param>
        public void playSandFountain(Vector2 pos, int num)
        {
            EmitterFountain e = sandSystem.EmitterList.ElementAt<Emitter>(num) as EmitterFountain;
            e.Position = pos;
            e.Pause = false;
            e.Active = true;
        }

        /// <summary>
        /// Resets the paralysis orbitter for a team.
        /// </summary>
        /// <param name="team">The team you want to affect.</param>
        public void playParalysis(int team)
        {
            foreach (Player p in SuperController.Instance.getPlayerOfTeam((team == 1) ? 2 : 1))
            {
                Orbitter orb = orbitterPlayers[p.PlayerIndex-1].OrbitterList[0];
                orb.LinkedObject = p;
                orb.Position = p.Position + p.Size * 0.5f;
                orb.resetParticles();
                orb.Visible = true;
            }
        }

        /// <summary>
        /// Stops the paralysis orbitter for a team.
        /// </summary>
        /// <param name="team">The team you want to affect.</param>
        public void stopParalysis(int team)
        {
            foreach (Player p in SuperController.Instance.getPlayerOfTeam((team == 1) ? 2 : 1))
            {
                Orbitter orb = orbitterPlayers[p.PlayerIndex - 1].OrbitterList[0];
                orb.Visible = false;
            }
        }

        /// <summary>
        /// Resets the stun orbitter for a team.
        /// </summary>
        /// <param name="p">The Player you want to affect.</param>
        public void playStun(Player p, float duration)
        {
            Orbitter starOrb = orbitterPlayers[p.PlayerIndex - 1].OrbitterList[1];
			starOrb.initializeParticles(duration / 1000, 0.1f);
            starOrb.LinkedObject = p;
            starOrb.Position = p.Position + p.Size * 0.5f;
            starOrb.Visible = true;
        }

        /// <summary>
        /// Visualizes the jumpheight powerup.
        /// </summary>
        /// <param name="team">The team you want to affect.</param>
        public void playJumpHighlight(int team)
        {
            foreach (Player p in SuperController.Instance.getPlayerOfTeam(team))
            {
                Emitter e = highlightJumpSystem.EmitterList[p.PlayerIndex - 1];
                e.LinkedObject = p;
                e.Position = p.Position + p.Size * 0.5f;
                e.EmittingDuration = 0f;
                e.Pause = false;
                e.Active = true;
            }
        }

        /// <summary>
        /// Stops the jumpheight powerup.
        /// </summary>
        /// <param name="team">The team you want to affect.</param>
        public void stopJumpHighlight(int team)
        {
            foreach (Player p in SuperController.Instance.getPlayerOfTeam(team))
            {
                Emitter e = highlightJumpSystem.EmitterList[p.PlayerIndex - 1];
                e.Active = false;
                e.EmittingDuration = float.Epsilon;
            }
        }

        /// <summary>
        /// Resizes the emitter for a specialbar.
        /// </summary>
        /// <param name="size">New size in percent of screen.</param>
        /// <param name="position">New position in percent of screen</param>
        /// <param name="team">Teamnumber that tells you which side is affected.</param>
        public void resizeSpecialbarEmitter(Vector2 size, Vector2 position, int team)
        {
            EmitterExplosion e = specialbarHighlight.EmitterList.ElementAt<Emitter>(team - 1) as EmitterExplosion;
            EmitterRectangleShape shape = e.EmitterShape as EmitterRectangleShape;
            e.Pause = false;
            e.ParticlesPerSecond = (int)(size.X * 200); //adjust emmited particles depending on the size of the bar.

            shape.Width = size.X * viewportSize.X / metertopixel;
            shape.Height = float.Epsilon;
            e.Position = new Vector2(position.X * viewportSize.X + size.X * viewportSize.X / 2f, position.Y * viewportSize.Y + size.Y * viewportSize.Y / 2f) / metertopixel;
        }

        #endregion

        /// <summary>
        /// Update all Particle Systems and orbitter
        /// </summary>
        /// <param name="time">Game Time</param>
        public void update(GameTime time)
        {

            //don't update when it's paused!
            if(this.isPaused) return;

            getTasks();

            foreach (ParticleSystem system in particleSystems)
            {
                system.update(time);
            }
        }

        /// <summary>
        /// Does all necessary action, to bring controller back to the state after initialization
        /// </summary>
        public void clear()
        {
            TaskManager.Instance.ParticleSystemTasks.Clear();
        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        public void initialize()
        {
            sandStormSystem = createSandStormSystem();

            metertopixel = (float)SettingsManager.Instance.get("meterToPixel");
            viewportSize = new Vector2(Game1.Instance.GraphicsDevice.Viewport.Width, Game1.Instance.GraphicsDevice.Viewport.Height);

            particleSystems = new List<ParticleSystem>();

            sandSystem = new ParticleSystem(55);
            sandSystem.SystemBlendState = BlendState.Additive;

            ballCollision = new ParticleSystem(55);
            ballCollision.SystemBlendState = BlendState.AlphaBlend;

            specialbarHighlight = new ParticleSystem(85);
            specialbarHighlight.SystemBlendState = BlendState.AlphaBlend;

            meteorBallSystem = new ParticleSystem(55);
            meteorBallSystem.SystemBlendState = BlendState.NonPremultiplied;

            highlightJumpSystem = new ParticleSystem(55);
            highlightJumpSystem.SystemBlendState = BlendState.AlphaBlend;

            bombSystem = new ParticleSystem[6];
            lavaExplosionSystem = new ParticleSystem[6];
            orbitterPlayers = new ParticleSystem[SuperController.Instance.getPlayerOfTeam(1).Count * 2];
            sparclingSystem = new ParticleSystem[2];

            for (int i = 0; i < bombSystem.Length; i++)
            {
                bombSystem[i] = new ParticleSystem(55);
                bombSystem[i].SystemBlendState = BlendState.NonPremultiplied;
                lavaExplosionSystem[i] = new ParticleSystem(55);
                lavaExplosionSystem[i].SystemBlendState = BlendState.NonPremultiplied;
            }
            for (int i = 0; i < sparclingSystem.Length; i++)
            {
                sparclingSystem[i] = new ParticleSystem(55);
                sparclingSystem[i].SystemBlendState = BlendState.NonPremultiplied;
            }

            //Add a StarOrbitter and JumpHighlightEmitter for every Player
            for (int i = 0; i < SuperController.Instance.getPlayerOfTeam(1).Count * 2; i++)
            {
                sandSystem.addEmitter(createSandFountainEmitter());
                highlightJumpSystem.addEmitter(createJumpEmitter());

                orbitterPlayers[i] = new ParticleSystem(55);
                orbitterPlayers[i].SystemBlendState = BlendState.AlphaBlend;
                orbitterPlayers[i].addOrbitter(createParalysisOrbitter("Textures/Particles/questionmark", new Vector2(0.2f, 0.2f), 8f, 1f, Color.White));
                orbitterPlayers[i].addOrbitter(createParalysisOrbitter("Textures/Particles/star_particle", new Vector2(0.1f, 0.1f), (int)SettingsManager.Instance.get("lavaStunDuration") / 1000f, 0.125f, Color.Yellow));

                addSystem(orbitterPlayers[i]);
            }

            //Create some star emitters for displaying a ball colission with an object
            ballCollision.addEmitter(createExplosionEmitter(new Vector2(0.1f, 0.1f), "star_particle", Color.Yellow));
            ballCollision.addEmitter(createExplosionEmitter(new Vector2(0.1f, 0.1f), "star_particle", Color.Yellow));
            ballCollision.addEmitter(createExplosionEmitter(new Vector2(0.1f, 0.1f), "star_particle", Color.Yellow));
            
            float resizeFactor = 1.0f;
            if (SuperController.Instance.getAllPlayers().Count > 2)
                resizeFactor = (float)SettingsManager.Instance.get("ResizeFactor2vs2Game");
            //Create a wind system for falling particles
            switch (SuperController.Instance.GameInstance.LevelName)
            {
                case "Maya":
                    break;
                case "Forest":
                    windSystem = new ParticleSystem[2];
                    windSystem[0] = new ParticleSystem(9);
                    windSystem[1] = new ParticleSystem(60);
					windSystem[0].addEmitter(createFallingParticlesEmitter(new Vector2(SuperController.Instance.GameInstance.LevelSize.X / 4 - 0.2f, 3.3f) * resizeFactor,
                        new Vector2(SuperController.Instance.GameInstance.LevelSize.X / 2 - 0.5f, 0.3f) * resizeFactor, new Vector2(0f, 1f), "leave_new", 1.5f));
                    windSystem[0].addEmitter(createFallingParticlesEmitter(new Vector2(SuperController.Instance.GameInstance.LevelSize.X * 0.75f + 0.7f, 2.8f) * resizeFactor,
                        new Vector2(SuperController.Instance.GameInstance.LevelSize.X / 2 - 2, 0.3f) * resizeFactor, new Vector2(0f, 1f), "leave_new", 1.5f));
                    windSystem[1].addEmitter(createFallingParticlesEmitter(new Vector2(SuperController.Instance.GameInstance.LevelSize.X / 2, -0.5f) * resizeFactor,
                        new Vector2(SuperController.Instance.GameInstance.LevelSize.X, 0.3f) * resizeFactor, new Vector2(0f, 1f), "leave_new", 0.5f));
                    playFallingParticles();
                    addSystem(windSystem[0]);
                    addSystem(windSystem[1]);
                    break;
                case "Beach":
                    windSystem = new ParticleSystem[1];
                    windSystem[0] = createVulcanoSystem(new Vector2(9.7f, 4) * resizeFactor);
                    addSystem(windSystem[0]);
                    break;
            }

            //Create highlighting for specialbar
            specialbarHighlight.addEmitter(createSpecialbarEmitter());
            specialbarHighlight.addEmitter(createSpecialbarEmitter());

            //create system for ball smoke and fire
            meteorBallSystem.addEmitter(createSmokeEmitterForTrail(new Vector2(1.0f, 1.0f)));
            meteorBallSystem.addEmitter(createFireEmitter(new Vector2(1.65f, 1.65f), new Vector2(0.2f, 0.2f), "Particle001"));
            meteorBallSystem.addEmitter(createFireEmitter(new Vector2(0.55f, 0.55f), new Vector2(0.25f, 0.25f), "Flame"));

            float scale = (float)SettingsManager.Instance.get("lavaRange") / (float) SettingsManager.Instance.get("superBombRange");

            for (int i = 0; i < bombSystem.Length; i++)
            {
                bombSystem[i].addEmitter(createSmokeEmitterForTrail(new Vector2(0.8f, 0.8f)));
                bombSystem[i].addEmitter(createFireEmitter(new Vector2(1.3f, 1.3f), new Vector2(0.15f, 0.15f), "Particle001"));
                bombSystem[i].addEmitter(createFireEmitter(new Vector2(0.45f, 0.45f), new Vector2(0.2f, 0.2f), "Flame"));
                addSystem(bombSystem[i]);

                lavaExplosionSystem[i].addEmitter(createExplosionEmitter(new Vector2(0.3f) * scale, "FlowerBurst", Color.Red, 6.2f, 90, 0.5f * scale));
                lavaExplosionSystem[i].addEmitter(createExplosionEmitter(new Vector2(2.5f) * scale, "smoke_particle", Color.White, 2.1f, 15, 0.7f * scale));
                lavaExplosionSystem[i].addEmitter(createExplosionEmitter(new Vector2(0.5f, 0.65f) * scale, "Beam", Color.Yellow, 5.8f, 15, true, new Vector2(0f, 4.5f), 1.0f * scale));
                lavaExplosionSystem[i].addEmitter(createExplosionEmitter(new Vector2(0.8f) * scale, "Flame", Color.Orange, 1.9f, 13, 0.5f * scale));
                lavaExplosionSystem[i].addEmitter(createExplosionEmitter(new Vector2(0.45f) * scale, "Flame", Color.Orange, 0.35f, 17, 0.4f * scale));
                addSystem(lavaExplosionSystem[i]);
            }
            superbombExplosionSystem = new ParticleSystem(55);
            superbombExplosionSystem.addEmitter(createExplosionEmitter(new Vector2(0.3f), "FlowerBurst", Color.Red, 6.2f, 90, 0.5f));
            superbombExplosionSystem.addEmitter(createExplosionEmitter(new Vector2(2.5f), "smoke_particle", Color.White, 2.1f, 15, 0.7f));
            superbombExplosionSystem.addEmitter(createExplosionEmitter(new Vector2(0.5f, 0.65f), "Beam", Color.Yellow, 5.8f, 15, true, new Vector2(0f, 4.5f), 1.0f));
            superbombExplosionSystem.addEmitter(createExplosionEmitter(new Vector2(1.2f), "Flame", Color.Orange, 2.3f, 17, 0.55f));
            superbombExplosionSystem.addEmitter(createExplosionEmitter(new Vector2(0.45f), "Flame", Color.Orange, 0.35f, 17, 0.4f));
            addSystem(superbombExplosionSystem);

            for (int i = 0; i < sparclingSystem.Length; i++)
            {
                sparclingSystem[i].addEmitter(createSparclingEmitterForSuperbomb(new Vector2(0.05f)));
                addSystem(sparclingSystem[i]);
            }

            addSystem(highlightJumpSystem);
            addSystem(sandSystem);
            addSystem(ballCollision);
            addSystem(specialbarHighlight);
            addSystem(meteorBallSystem);
            addSystem(sandStormSystem);
        }

        /// <summary>
        /// Forces controller to pause, has to make sure that all timers etc. pause
        /// </summary>
        /// <param name="on">true: controller has to pause, false: controller has to work </param>
        public void pause(bool on)
        {
            isPaused = on;
        }

        /// <summary>
        /// Add a new Particle System to the Manager
        /// </summary>
        /// <param name="system">A Particle System</param>
        public void addSystem(ParticleSystem system)
        {
            this.particleSystems.Add(system);
            SuperController.Instance.addGameObjectToGameInstance(system);
        }
        /// <summary>
        /// Remove a Particle System
        /// </summary>
        /// <param name="system">The Particle System to be removed</param>
        public void removeSystem(ParticleSystem system)
        {
            this.particleSystems.Remove(system);
        }

        /// <summary>
        /// Processes all tasks that are sent to the ParticleSystemTask by other Controllers.
        /// </summary>
        private void getTasks()
        {
            foreach (ParticleSystemTask task in TaskManager.Instance.ParticleSystemTasks)
            {
                switch (task.EffectType)
                {
                    case 0:
                        playBallCollision(task.ParticleEffectPosition);
                        break;
                    case 1:
                        playSandFountain(task.ParticleEffectPosition, 0);
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }
            }

            // Clear list after executing all tasks
            TaskManager.Instance.ParticleSystemTasks.Clear();
        }


        #endregion


    }
}
