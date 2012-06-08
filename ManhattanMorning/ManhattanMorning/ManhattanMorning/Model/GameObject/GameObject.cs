using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.GameObject
{
   public abstract class GameObject : DrawableObject
    {

        #region Properties

        /// <summary>
        /// Shadow texture of the Ball.
        /// </summary>
        public Texture2D ShadowTexture { get { return shadowTexture; } set { shadowTexture = value; } }

        #endregion

        #region Members

        /// <summary>
        /// Shadow texture of the Ball.
        /// </summary>
        private Texture2D shadowTexture;

        #endregion

        #region Intializaition
        /// <summary>
        /// Intialization of a Game Objects.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public GameObject(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation,
            Vector2 size, Vector2 position, int layer, MeasurementUnit unit) :
            base(name, visible, texture, animation, size, position, layer, unit)
        {
            this.shadowTexture = shadowTexture;
        }

        /// <summary>
        /// Intialization of a Game Objects with Waypoints.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        /// <param name="waypoints">Waypoints the object moves along.</param>
        public GameObject(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation,
            Vector2 size, Vector2 position, int layer, MeasurementUnit unit, PathAnimation path) :
            base(name, visible, texture, animation, size, position, layer, unit, path)
        {
            this.shadowTexture = shadowTexture;
        }

        public GameObject()
        {
        }

        #endregion

        #region Methods
        /// place for methods; remove when addine some
        #endregion
    }
}
