using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.HUD
{   
    /// <summary>
    /// Responsible for displaying all necessary informations
    /// in game like score, time, ...
    /// </summary>
    public class HUD : DrawableObject
    {

        #region Properties

        /// <summary>
        /// Defines which section of the texture should be drawn
        /// (Rectangle.Empty if whole texture should be drawn)
        /// </summary>
        public Rectangle? SourceRectangle { get { return sourceRectangle; } set { sourceRectangle = value; } }

        #endregion

        #region Members

        /// <summary>
        /// Defines which section of the texture should be drawn
        /// (Rectangle.Empty if whole texture should be drawn)
        /// </summary>
        private Rectangle? sourceRectangle;

        #endregion

        #region Initialization

        /// <summary>
        /// Intialization of a HUD element
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="Position">The MIDDLE position. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public HUD(String name ,bool visible, Texture2D texture, SpriteAnimation animation, Vector2 size, Vector2 position, 
            int layer, MeasurementUnit measurementUnit) 
            : base (name, visible, texture, animation, size, position, layer,measurementUnit) 
        {   

            // Set members
        }

        #endregion


    }
}
