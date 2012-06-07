using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Torrero.Model
{
    public abstract class Tile
    {
        // All getters, setters.
        #region Properties

        // The tiles position
        public Vector2 BottomLeftPosition
        {
            get { return position; }
            set { position = value; }
        }

        // The tiles texture
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// Y Position.
        /// </summary>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// X Position
        /// </summary>
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Center of the Object.
        /// </summary>
        public Vector2 CenterPosition
        {
            get { return (position + new Vector2(TorreroConstants.TileSizeHalf)); }
            set { position = value - new Vector2(TorreroConstants.TileSizeHalf); }
        }

        /// <summary>
        /// Top coordinate of the object.
        /// </summary>
        public float Top
        {
            get { return position.Y + TorreroConstants.TileSizeHalf; }
        }

        /// <summary>
        /// Left coordinate of the object.
        /// </summary>
        public float Left
        {
            get { return position.X; }
        }

        /// <summary>
        /// Right coordinate of the object.
        /// </summary>
        public float Right
        {
            get { return position.X + TorreroConstants.TileSizeHalf; }
        }

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        // The tiles position
        private Vector2 position;

        // The tiles texture
        private Texture2D texture;

        private int x;

        private int y;

        #endregion

        // Object constructors.
        #region Initialization

        /// <summary>
        /// Creates a new tile. Should not be called, since Tile is abstract.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        public Tile(int x, int y, Vector2 position, Texture2D texture)
        {
            this.x = x;
            this.y = y;
            this.position = position;
            this.texture = texture;
        }

        #endregion
    }
}
