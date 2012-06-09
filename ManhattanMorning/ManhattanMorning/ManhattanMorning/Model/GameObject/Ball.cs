using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;
using ManhattanMorning.Model.HUD;

namespace ManhattanMorning.Model.GameObject
{
    public class Ball : ActiveObject
    {

        #region Properties

        /// <summary>
        /// Reference to its ball indicator
        /// </summary>
        public HUD.HUD BallIndicator
        {
            get { return ballIndicator; }
            set { ballIndicator = value; }
        }

        #endregion

        #region Members

        /// <summary>
        /// Reference to its ball indicator
        /// </summary>
        private HUD.HUD ballIndicator;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a Ball object with an animation.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        /// <param name="ballIndicator">Reference to its ball indicator</param>
        public Ball(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation, Vector2 size, Vector2 position, int layer, MeasurementUnit measurementUnit, HUD.HUD ballIndicator)
            : base(name, visible, texture, shadowTexture ,animation, size, position, layer, measurementUnit)
        {

            // set members
            this.ballIndicator = ballIndicator;

            //this.attachObject(new PassiveObject("Ball-Highlight", name, size, position, layer + 1));

        }

        public Ball(String name)
        {
            TextureName = "ball";
            Name = name;
        }

        #endregion


    }
}
