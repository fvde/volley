using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.GameObject
{
    /// <summary>
    /// Display a String on the Screen.
    /// </summary>
    public class TextObject : DrawableObject
    {
        
        #region Properties

        public SpriteFont Font { get { return font; } set { font = value; } }

        #endregion

        #region Members

        /// <summary>
        /// Font of the text.
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// String we want to print on the screen.
        /// </summary>
        private String text;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a text that we can print on the screen.
        /// </summary>
        /// <param name="name">Name of the Object</param>
        /// <param name="visible">If the Object is visible or not</param>
        /// <param name="texture">Texture of the object</param>
        /// <param name="animation">Animation of the object</param>
        /// <param name="size">Size of the Object</param>
        /// <param name="position">Position of the Object</param>
        /// <param name="layer"></param>
        /// <param name="measurementUnit"></param>
        /// <param name="text"></param>
        public TextObject(String name, bool visible, Texture2D texture, SpriteAnimation animation, Vector2 size, Vector2 position,
            int layer, MeasurementUnit measurementUnit, ref int integer, SpriteFont font)
            : base(name, visible, texture, animation, size, position, layer, measurementUnit)
        {
            this.font = font;
            this.text = Convert.ToString(integer);
        }
        
        #endregion


        #region Methods


        #endregion


    }
}
