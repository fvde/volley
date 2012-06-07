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
    /// The Net seperating the volleyball field in two halfs
    /// </summary>
    public class Net : ActiveObject
    {

        #region Properties

        // all the Properties right here

        #endregion

        #region Members

        // all the Members right here

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public Net(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation, Vector2 size, Vector2 position, int layer, MeasurementUnit measurementUnit)
            : base(name, visible, texture, shadowTexture,animation, size, position, layer, measurementUnit)
        {
        }

        public Net(String name, Vector2 size)
            : base(name, size)
        {
            TextureName = "net";
        }

        #endregion

    }
}
