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
    public class MoveableGameObject : GameObject
    {
        // All getters, setters.
        #region Properties

        /// <summary>
        /// The multiplicator is used to modify object speed like acceleration and deceleration.
        /// </summary>
        public Vector2 SpeedMultiplicator
        {
            get { return speedMultiplicator; }
            set { speedMultiplicator = value; }
        }

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        private Vector2 speedMultiplicator;

        #endregion

        // Object constructors.
        #region Initialization

        public MoveableGameObject(Vector2 position, Vector2 size, Texture2D texture)
            : base(position, size, texture)
        {
            speedMultiplicator = new Vector2(1f);
        }

        #endregion
    }
}
