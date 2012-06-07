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
    public abstract class GameObject
    {
        // All getters, setters.
        #region Properties

        /// <summary>
        /// Position of the object.
        /// </summary>
        public Vector2 BottomLeftPosition
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Texture of the object.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        /// <summary>
        /// Size of the object.
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Center of the Object.
        /// </summary>
        public Vector2 CenterPosition
        {
            get { return (position + size / 2); }
            set { position = value - size / 2; }
        }

        /// <summary>
        /// Top coordinate of the object.
        /// </summary>
        public float Top
        {
            get { return position.Y + size.Y; }
            set { position.X = value - size.Y; }
        }

        /// <summary>
        /// Left coordinate of the object.
        /// </summary>
        public float Left
        {
            get { return position.X; }
            set { position.X = value; }
        }

        /// <summary>
        /// Right coordinate of the object.
        /// </summary>
        public float Right
        {
            get { return position.X + size.X; }
            set { position.X = value - size.X; }
        }

        /// <summary>
        /// Rotation in radians.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        private Vector2 position;

        private Texture2D texture;

        private Vector2 size;

        /// <summary>
        /// Rotation in radians.
        /// </summary>
        private float rotation;

        #endregion

        // Object constructors.
        #region Initialization

        public GameObject(Vector2 position, Vector2 size, Texture2D texture)
        {
            this.position = position;
            this.size = size;
            this.texture = texture;
        }

        #endregion
    }
}
