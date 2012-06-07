using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Model.ParticleSystem
{
    class EmitterRectangleShape : EmitterShape
    {
        #region Members
        private float width;
        private float height;
        private Vector2 orientation;
        private Random random;
        private Vector2 right;
        #endregion

        #region Properties
        /// <summary>
        /// Width of the Rectangle
        /// </summary>
        public float Width { get { return width; } set { width = value; } }
        /// <summary>
        /// Height of the Rectangle
        /// </summary>
        public float Height { get { return height; } set { height = value; } }

        /// <summary>
        /// The normalized Direction, the Top is Facing. (0f,1f) is up
        /// </summary>
        public Vector2 Orientation 
        {
            get { return this.orientation; }
            set
            {
                this.orientation = value;
                orientation.Normalize();
            } 
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Create a new Rectangle Shape
        /// </summary>
        public EmitterRectangleShape(float width, float height, Vector2 orientation)
        {
            this.orientation = orientation;
            orientation.Normalize();
            this.right = new Vector2(this.Orientation.Y, -this.Orientation.X);
            right.Normalize();
            this.height = height;
            this.width = width;
            this.random = new Random();
        }

        #endregion

        #region Methods

        /// <summary>
        ///Calculates a Random Position within the Shape relative to the center. (0f,0f) is Center
        /// </summary>
        /// <returns>Position as Vector2</returns>
        public override Vector2 randomPositionWithinShape()
        {
            float precision = 100000f;
            Vector2 position = new Vector2();

            float randWidth = (random.Next(0, (int)precision) - 0.5f * precision) / precision;
            float randHeight = (random.Next(0, (int)precision) - 0.5f * precision) / precision;

            position = randWidth * right * width + randHeight * this.orientation * height;
            return position;
        }

        #endregion

    }
}
