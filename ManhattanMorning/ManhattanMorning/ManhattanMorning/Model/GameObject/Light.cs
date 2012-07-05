using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ManhattanMorning.Misc;
using ManhattanMorning.Model.GameObject;

namespace ManhattanMorning.Model
{
    public class Light : PassiveObject
    {

        #region Properties

        /// <summary>
        /// Object that this light is attached to. It will share the same position.
        /// </summary>
        public DrawableObject AttachedObject { get { return attachedObject; } set { attachedObject = value; } }

        public override Vector2 Position
        {
            get
            {
                return (attachedObject == null)?base.Position: attachedObject.Position - (this.Size - attachedObject.Size) / 2 ;
            }
            set
            {
                base.Position = value;
            }
        }

        #endregion


        #region Members

        /// <summary>
        /// Light that is attached to this object. It will share the same position.
        /// </summary>
        private DrawableObject attachedObject;

        #endregion

        /// <summary>
        /// Create a new light with custom position, color etc...
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="texture">Shape of the light.</param>
        /// <param name="size">Size of the light.</param>
        /// <param name="position">Position of the light.</param>
        /// <param name="rotation">Rotation of the light.</param>
        /// <param name="color">Color of the light.</param>
        /// <param name="active">If the light is active or not.</param>
        public Light(String name, Texture2D texture, Vector2 size, Vector2 position, float rotation, Color color, bool active):
            base(name, active, texture, null, null, size, position, -1, MeasurementUnit.Meter)
        {
            Rotation = rotation;
            BlendColor = color;
        }

        /// <summary>
        /// Create a new light with custom position, color, some waypoints to follow etc...
        /// </summary>
        /// <param name="texture">Shape of the light.</param>
        /// <param name="size">Size of the light.</param>
        /// <param name="color">Color of the light.</param>
        /// <param name="active">If the light is active or not.</param>
        public Light(String name, Texture2D texture, Vector2 size, Vector2 position, Color color, bool active, PathAnimation path ):
            base(name, active, texture, null, null, size, position, -1, MeasurementUnit.Meter, path)
        {
            BlendColor = color;
        }

        /// <summary>
        /// Create a new light with custom position, color etc...
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="shape">Shape of the light.</param>
        /// <param name="size">Size of the light.</param>
        /// <param name="position">Position of the light.</param>
        /// <param name="rotation">Rotation of the light.</param>
        /// <param name="color">Color of the light.</param>
        /// <param name="active">If the light is active or not.</param>
        public Light(String name, String textureName, Vector2 size, Vector2 position, Color color, int layer) :
            base(name, textureName, size, position, layer, null)
        {
            BlendColor = color;
        }



    }
}
