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
    public class Hand
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

        // Defines offset from middle position on player
        private Vector2 positionOffset;
        public Vector2 PositionOffset
        {
            get { return positionOffset; }
            set { positionOffset = value; }
        }

        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }



        // Constructors

        public Hand()
        {

        }

        // Methods

        public Rectangle DestinationRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public Vector2 Speed()
        {
            return new Vector2((position.X - lastPosition.X), (position.Y - lastPosition.Y));
        }

    }
}
