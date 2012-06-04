using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace ManhattanMorning.Model.ParticleSystem
{
    /// <summary>
    /// Represents a single Particle.
    /// In every other Language you would use a struct instead of a class, but poeple say in c#, this is faster
    /// </summary>
    public class Particle
    {

        #region Members

        //Position of the Particle
        private Vector2 position;

        //size of the Particle in Meter
        private Vector2 size;

        //The current age of the Particle in Milleseconds
        private float age;

        //The time until the Particle is destroyed
        private float lifeTime;

        //Is the Particle Visible
        private bool visible;

        //Particle Movement in Units per Second
        private Vector2 velocity;

        //Transparency of the Particle
        private float alpha;

        /// <summary>
        /// Rotation in Rad. 2*pi = 360°
        /// </summary>
        private float rotation;

        /// <summary>
        /// True if the direction of the Particle has influence on its rotation.
        /// </summary>
        private bool dirAsRot;

        #endregion

        #region Properties

        /// <summary>
        /// Position of the Particle
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// size of the Particle in Meter
        /// </summary>
        public Vector2 Size { get { return this.size; } set { this.size = value; } }

        /// <summary>
        /// The time until the Particle is destroyed
        /// </summary>
        public float LifeTime { get { return this.lifeTime; } set { this.lifeTime = value; } }


        /// <summary>
        /// Is the Particle Visible?
        /// </summary>
        public bool Visible { get { return this.visible; } set { this.visible = value;} }

        /// <summary>
        /// Velocity multiplied with direction
        /// </summary>
        public Vector2 Veclocity { get { return this.velocity;  } 
            set{
                this.velocity = value;
                if (dirAsRot) Rotation = 1f;
            }
        }

        /// <summary>
        /// Age of the Particle
        /// </summary>
        public float Age { get { return this.age; } set { this.age = value; } }

        /// <summary>
        /// The Transparency of the Blog between 0f and 1f (0 is invisible)
        /// </summary>
        public float Alpha { get { return this.alpha; }  set { this.alpha = value; } }

        /// <summary>
        /// Rotation in Rad.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = (dirAsRot)?(float)Math.Atan2(velocity.Y, velocity.X): MathHelper.ToRadians(value * 360); }
        }

        /// <summary>
        /// True if the direction of the Particle has influence on its rotation.
        /// </summary>
        public bool DirAsRot
        {
            get { return dirAsRot; }
            set { dirAsRot = value; }
        }

        public Color Color { get; set; }

        #endregion


        #region Initialization

        public Particle(Vector2 position, float lifeTime, float rotation)
        {
            Rotation = rotation;
            this.position = position;
            this.lifeTime = lifeTime;
            this.alpha = 1;
            this.age = 0;
        }

        #endregion


    }
}
