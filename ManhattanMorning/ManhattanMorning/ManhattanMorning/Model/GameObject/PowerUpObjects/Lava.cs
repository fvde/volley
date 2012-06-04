using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.GameObject
{
    public class Lava : PowerUpObject
    {

        #region Properties


        #endregion

        #region Members


        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a Lava object with an animation.
        /// </summary>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        /// <param name="ballIndicator">Reference to its ball indicator</param>
        public Lava(Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation, Vector2 size, Vector2 position, int layer, MeasurementUnit measurementUnit)
            : base(texture, shadowTexture, animation, size, position, layer, measurementUnit)
        {

        }

        #endregion


    }
}
