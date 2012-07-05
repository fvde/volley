using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.Menu
{
    /// <summary>
    /// Represents all menu objects which aren't Button- or Overlayobjects
    /// </summary>
    public class MenuObject : DrawableObject
    {


        #region Properties

        /// <summary>
        /// A path the object follows
        /// </summary>
        public PathAnimation MovingPath { get { return movingPath; } set { movingPath = value; } }

        #endregion

        #region Members

        /// <summary>
        /// A path the object follows
        /// </summary>
        private PathAnimation movingPath;

        #endregion

        #region Initialization

        /// <summary>
        /// Intialization of a MenuObject
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public MenuObject(String name, bool visible, Texture2D texture, SpriteAnimation animation,
            Vector2 size, Vector2 position, int layer, MeasurementUnit unit) :
            base(name, visible, texture, animation, size, position, layer, unit)
        {

        }

        #endregion

        #region Methods

        #endregion


    }
}
