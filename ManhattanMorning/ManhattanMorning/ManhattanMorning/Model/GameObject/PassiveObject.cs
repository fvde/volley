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
    /// Part of the Specification Inheritance. Represents and inherits to the group of PassiveObjects.
    /// </summary>
    public class PassiveObject : GameObject
    {

        #region Properties

        // all the variables, getters, setters right here

        #endregion

        #region Members
        /// place for the members; remove when adding some.
        #endregion


        #region Initialization

        /// <summary>
        /// Intialization of a Passive Objects.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public PassiveObject(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation, Vector2 size, Vector2 position, int layer, MeasurementUnit measurementUnit)
            : base(name, visible, texture, shadowTexture,animation,size, position, layer, measurementUnit)
        {
        }

        /// <summary>
        /// Intialization of a Passive Object with Waypoints.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        /// <param name="waypoints">Waypoints the object moves along.</param>
        public PassiveObject(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation, Vector2 size, Vector2 position, int layer, MeasurementUnit measurementUnit, PathAnimation path)
            : base(name, visible, texture, shadowTexture ,animation, size, position, layer, measurementUnit, path)
        {
        }

        /// <summary>
        /// Creates a PassiveObject
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="textureName">The name of the object's texture</param>
        /// <param name="size">The size of the object (in meters)</param>
        /// <param name="position">The position of the object (in meters, origin is in the upper left half)</param>
        /// <param name="layer">The layer in which the object is positioned</param>
        public PassiveObject(String name, String textureName, Vector2 size, Vector2 position, int layer)
        {
            // Save all parameters
            this.Name = name;
            this.TextureName = textureName;
            this.Size = size;
            this.Position = position;
            this.Layer = layer;
        }

        /// <summary>
        /// Creates PassiveObject
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="size">The size of the object (in meters)</param>
        /// <param name="position">The position of the object (in meters, origin is in the upper left half)</param>
        /// <param name="rotation">Rotation in radians.</param>
        /// <param name="layer">The layer in which the object is positioned</param>
        /// <param name="animation">The animation for the object</param>
        public PassiveObject(String name, Vector2 size, Vector2 position, float rotation, int layer, SpriteAnimation animation)
        {
            // Save all parameters
            this.Name = name;
            this.TextureName = null;
            this.Size = size;
            this.Position = position;
            this.Rotation = rotation;
            this.Layer = layer;
            this.Animation = animation;
        }

        /// <summary>
        /// Creates a PassiveObject
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="textureName">The name of the object's texture</param>
        /// <param name="size">The size of the object (in meters)</param>
        /// <param name="position">The position of the object (in meters, origin is in the upper left half)</param>
        /// <param name="layer">The layer in which the object is positioned</param>
        /// <param name="path">A path which the object follows</param>
        public PassiveObject(String name, String textureName, Vector2 size, Vector2 position, int layer, PathAnimation path)
        {
            // Save all parameters
            this.Name = name;
            this.TextureName = textureName;
            this.Size = size;
            this.Position = position;
            this.Layer = layer;
            this.PathAnimation = path;
        }

        /// <summary>
        /// Creates a PassiveObject
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="size">The size of the object (in meters)</param>
        /// <param name="position">The position of the object (in meters, origin is in the upper left half)</param>
        /// <param name="layer">The layer in which the object is positioned</param>
        /// <param name="animation">The animation for the object</param>
        /// <param name="path">The path ehich the object follows</param>
        public PassiveObject(String name, Vector2 size, Vector2 position, int layer, SpriteAnimation animation, PathAnimation path)
        {
            // Save all parameters
            this.Name = name;
            this.Size = size;
            this.Position = position;
            this.Layer = layer;
            this.Animation = animation;
            this.PathAnimation = path;
        }

        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
