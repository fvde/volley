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
    /// Represents a button in the menu
    /// </summary>
    public class MenuButtonObject : MenuObject
    {


        #region Properties

        /// <summary>
        /// Indicates if the button is selected
        /// </summary>
        public bool Selected { get { return selected; } set { selected = value; } }

        /// <summary>
        /// Color of the light.
        /// </summary>
        public override Texture2D Texture
        {
            get
            {
                return (selected) ? texture_selected : base.Texture;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Indicates if the button is selected
        /// </summary>
        private bool selected;

        /// <summary>
        /// The texture that is displayed when the button is selected
        /// </summary>
        private Texture2D texture_selected;

        #endregion

        #region Initialization

        /// <summary>
        /// Intialization of a MenuButtonObject
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public MenuButtonObject(String name, bool visible, Texture2D texture_unselected, Texture2D texture_selected, SpriteAnimation animation,
            Vector2 size, Vector2 position, int layer, MeasurementUnit unit) :
            base(name, visible, texture_unselected, animation, size, position, layer, unit)
        {

            // Save values
            this.texture_selected = texture_selected;

            // Set default values
            this.selected = false;
            
        }

        #endregion

        #region Methods

        #endregion


    }
}
