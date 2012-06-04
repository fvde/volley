using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using ManhattanMorning.Controller;
using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.GameObject
{
    /// <summary>
    /// Part of the Specification Inheritance. Represents and inherits to the group of ActiveObjects.
    /// </summary>
    public abstract class ActiveObject : GameObject
    {

        #region Properties

        /// <summary>
        /// References to body of every object in farseer engine
        /// </summary>
        public Body Body { get { return body; } set { body = value; } }

        /// <summary>
        /// Overrides position getter and takes body's position
        /// </summary>
        public override Vector2 Position 
        { 
            get { return body.Position - this.Size * 0.5f; }
            set { body.Position = value + this.Size * 0.5f; ; }
        }
    
        /// <summary>
        /// Overrides Rotation getter and takes body's rotation 
        /// </summary>
        public override float Rotation { get { return body.Rotation;} set { body.Rotation = value; } }

        #endregion

        #region Members

        /// <summary>
        /// References to body of every object in farseer engine
        /// </summary>
        private Body body;

        #endregion


        #region Initialization
        /// <summary>
        /// Intialization of a Active Objects.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public ActiveObject(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation, Vector2 size, Vector2 position, int layer, MeasurementUnit measurementUnit)
            : base(name, visible, texture, shadowTexture, animation, size, position, layer,measurementUnit)
        {

            Body = Physics.Instance.addObjectToPhysicsSimulator(this, size, (position + (size * 0.5f)));
            Body.LinkedActiveObject = this;
            Physics.Instance.initializeCollisionListenerForbody(this.Body);
        }
        
        public ActiveObject(String name, Vector2 size)
        {
            this.Name = name;
            this.Size = size;
            Body = Physics.Instance.addObjectToPhysicsSimulator(this, size, Vector2.Zero);
            Body.LinkedActiveObject = this;
            Physics.Instance.initializeCollisionListenerForbody(this.Body);
        }

        public ActiveObject()
        {
        }

        #endregion

        #region Methods
        // place for methods; remove when adding some

        #endregion

    }
}
