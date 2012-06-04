using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Volley2.Objects
{
    public class Ball
    {

        // Variables

        private Texture2D graphics;
        public Texture2D Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // Stores position before last update
        private Vector2 lastPosition;
        public Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }
        }

        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        private Vector2 speed;
        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        // Constructors

        public Ball()
        {

            speed = new Vector2(0, 0);


        }


        // Methods

        public Rectangle destinationRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }


    }
}
