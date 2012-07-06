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
using Torrero.Controller;

namespace Torrero.Model
{
    public class Player : MoveableGameObject
    {
        // All getters, setters.
        #region Properties
        
        /// <summary>
        /// The Tile the player currently is on.
        /// </summary>
        public Tile CurrentTile
        {
            get { return currentTile; }
            set { currentTile = value; }
        }

        /// <summary>
        /// The Tile the player is moving towards.
        /// </summary>
        public Tile NextTile
        {
            get { return nextTile; }
            set { nextTile = value; }
        }

        /// <summary>
        /// Direction in which the player moves at the next crossing.
        /// </summary>
        public MovingDirection DirectionAtCrossing
        {
            get { return directionAtCrossing; }
            set { directionAtCrossing = value; }
        }

        /// <summary>
        /// Vertical speed of the player.
        /// </summary>
        public float VerticalSpeed
        {
            get { return verticalSpeed; }
            set { verticalSpeed = value; }
        }

        /// <summary>
        /// Horizontal speed of the player.
        /// </summary>
        public float HorizontalSpeed
        {
            get { return horizontalSpeed; }
            set { horizontalSpeed = value; }
        }

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        /// <summary>
        /// The Tile the player currently is on.
        /// </summary>
        private Tile currentTile;

        /// <summary>
        /// The Tile the player is moving towards.
        /// </summary>
        private Tile nextTile;

        /// <summary>
        /// Direction in which the player moves at the next crossing.
        /// </summary>
        private MovingDirection directionAtCrossing;

        /// <summary>
        /// Horizontal speed of the player.
        /// </summary>
        private float horizontalSpeed;

        /// <summary>
        /// Vertical speed of the player.
        /// </summary>
        private float verticalSpeed;
        
        #endregion

        // Object constructors.
        #region Initialization

        public Player(Vector2 position, Vector2 size, Texture2D texture)
            : base(position, size, texture)
        {
            horizontalSpeed = TorreroConstants.HorizontalSpeed;
            verticalSpeed = TorreroConstants.VerticalSpeed;
        }

        #endregion
    }
}
