using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;
using ManhattanMorning.Controller;

namespace ManhattanMorning.Model
{
    /// <summary>
    /// Can be actual meters or percent of screen resolution
    /// </summary>
    public enum MeasurementUnit
    {
        /// <summary>
        /// Meters like in the physic
        /// </summary>
        Meter,
        /// <summary>
        /// indicates that all sizes within this object are given in percentage of screen resolution.
        /// </summary>
        PercentOfScreen,
        /// <summary>
        /// indicates that all sizes within this object are given in pixel.
        /// </summary>
        Pixel
    }

    public abstract class DrawableObject : LayerInterface
    {

        #region Properties

        /// <summary>
        /// Name of the Object.
        /// </summary>
        public String Name { get { return this.name; } set { name = value; } }

        /// <summary>
        /// Current Unit for all the sizes in this Object
        /// </summary>
        public MeasurementUnit Unit { 
            get { return this.unit; } 
            set{this.unit = value;} 
        }

        /// <summary>
        /// Indicates if object is visible
        /// </summary>
        public bool Visible { 
            get { return this.visible; } 
            set { 
                if(value && this.texture == null && this.animation == null) 
                { throw new ArgumentException("Drawable Object cannot be visible if it has no texture or animation"); }
                this.visible = value;
            }
        }

        /// <summary>
        ///  size in meter, if the object is a circle the x-part of the Vector is the aperture of the circle (not the radius)
        /// </summary>
        public virtual Vector2 Size { get { return this.size; } set { this.size = value;} }

        /// <summary>
        /// Upper left position in meter.
        /// </summary>
        public virtual Vector2 Position
        {
            get
            {
                return (attachedTo == null) ? position + offset : attachedTo.Position - (size - attachedTo.Size) / 2 + offset; 
            }
            set { position = value; }
        }

        /// <summary>
        /// Offset which is used to change the position of the object from its actual position (e.g. when linked to another object).
        /// </summary>
        public Vector2 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        /// <summary>
        /// Rotation in radians.
        /// </summary>
        public virtual float Rotation { get { return this.rotation; } set { rotation = value; } }

        /// <summary>
        /// Layer Position of this Object between 0 and 100. 100 being on top.
        /// </summary>
        public int Layer { get { return this.layer; } set { layer = value; } }

        /// <summary>
        /// Texture of the drawable Object. If It has an animation, this is set to null
        /// </summary>
        public virtual Texture2D Texture { get { return this.texture; } set { texture = value; } }

        /// <summary>
        /// Animation of this Object. This is NULL if the object only has a simple Texture
        /// </summary>
        public SpriteAnimation Animation { get { return animation; } set { animation = value; } }

        /// <summary>
        /// Indicates if texture will be flipped horizontally
        /// </summary>
        public Boolean FlipHorizontally { get { return this.flipHorizontally; } set { this.flipHorizontally = value; } }

        /// <summary>
        /// Path the object moves along.
        /// </summary>
        public PathAnimation PathAnimation
        {
            get { return this.pathAnimation; }
            set 
            {
                if (value != null)
                {
                    this.pathAnimation = value;
                    this.pathAnimation.CorrespondingObject = this;
                }
            }
        }

        /// <summary>
        /// The alpha value of the Object. It's ensured that this value is only between 0 and 1.
        /// </summary>
        public float Alpha
        { 
            get { return alpha; }
            set 
            {
                alpha = value;
                //set object invisible because alpha value is so low that you can't see it. saves many actions on draw.
                if (value == 0f) visible = false; else visible = true;
            }
        }

        /// <summary>
        /// FadingAnimation is used to fade in and out with alpha value.
        /// </summary>
        public FadingAnimation FadingAnimation
        {
            get { return fadingAnimation; }
            set { fadingAnimation = value; }
        }

        /// <summary>
        /// Color of the light.
        /// </summary>
        public Color BlendColor { get { return blendColor; } set { blendColor = value; } }

        /// <summary>
        /// Reference to the object to which this object is attached.
        /// </summary>
        public DrawableObject AttachedTo
        {
            get { return attachedTo; }
            set { attachedTo = value; }
        }

        /// <summary>
        /// All objects that are attached.
        /// </summary>
        public List<DrawableObject> AttachedObjects
        {
            get { return attachedObjects; }
            set { attachedObjects = value; }
        }

        /// <summary>
        /// Name of the Texture.
        /// </summary>
        public String TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }
        
        #endregion

        #region Members

        /// <summary>
        /// Name of the Object.
        /// </summary>
        private String name;

        /// <summary>
        /// Texture of the drawable Object. If It has an animation, this is set to null
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Animation of this Object. This is NULL if the object only has a simple Texture
        /// </summary>
        private SpriteAnimation animation;

        /// <summary>
        /// Indicates if texture will be flipped horizontally
        /// </summary>
        private bool flipHorizontally;

        /// <summary>
        /// Layer Position of this Object between 0 and 100. 100 being on top.
        /// </summary>
        private int layer;

        /// <summary>
        /// Indicates if object is visible
        /// </summary>
        private bool visible;

        /// <summary>
        /// Current Unit for all the sizes in this Object
        /// </summary>
        private MeasurementUnit unit;

        /// <summary>
        ///  size in meter, if the object is a circle the x-part of the Vector is the aperture of the circle (not the radius)
        /// </summary>
        private Vector2 size;

        /// <summary>
        /// Rotation in radians
        /// </summary>
        private float rotation;

        /// <summary>
        /// Upper left position in meter.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// Offset which is used to change the position of the object from its actual position (e.g. when linked to another object).
        /// </summary>
        private Vector2 offset;

        /// <summary>
        /// A Path the Object moves along
        /// </summary>
        private PathAnimation pathAnimation;

        /// <summary>
        /// The alpha value of the Object.
        /// </summary>
        private float alpha = 1f;

        /// <summary>
        /// FadingAnimation is used to fade in and out with alpha value.
        /// </summary>
        private FadingAnimation fadingAnimation;

        /// <summary>
        /// Color of the light.
        /// </summary>
        private Color blendColor = Color.White;

        /// <summary>
        /// All objects that are attached.
        /// </summary>
        private List<DrawableObject> attachedObjects;

        /// <summary>
        /// Reference to the object to which this object is attached.
        /// </summary>
        private DrawableObject attachedTo;

        /// <summary>
        /// Name of the Texture.
        /// </summary>
        private String textureName;
                        
        #endregion


        #region Initialization

        /// <summary>
        /// Initialization of a DrawableObject.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        public DrawableObject(String name, bool visible, Texture2D texture, SpriteAnimation animation, Vector2 Size, Vector2 Position, int layer, MeasurementUnit measurementUnit ){

            if (visible && texture == null && animation == null && !(this is Light))
            {
                throw new ArgumentException("a Drawable Object cannot be visible if it has no animation or texture");
            }

            this.name = name;
            this.visible = visible;
            this.texture = texture;
            this.animation = animation;
            this.size = Size;
            this.position = Position;
            this.layer = layer;
            this.Unit = measurementUnit;

            // Set other members to default values
            rotation = 0;
            flipHorizontally = false;
            attachedObjects = new List<DrawableObject>();
        }

        /// <summary>
        /// Initialization of a DrawableObject.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in percent of screen</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit.</param>
        /// <param name="waypoints">Waypoints we want to move along.</param>
        public DrawableObject(String name, bool visible, Texture2D texture, SpriteAnimation animation, Vector2 Size, Vector2 Position, int layer, MeasurementUnit measurementUnit, PathAnimation path)
        : this(name, visible, texture, animation, Size, Position, layer, measurementUnit)
        {
            this.PathAnimation = path;
        }

        public DrawableObject()
        {            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the given object to this object.
        /// </summary>
        /// <param name="objectToAttach">The object to attach.</param>
        public void attachObject(DrawableObject objectToAttach)
        {
            attachedObjects.Add(objectToAttach);
            objectToAttach.attachedTo = this;
        }

        /// <summary>
        /// Uncouples the given object.
        /// </summary>
        /// <param name="objectToUncouple">The object to uncouple.</param>
        public void uncoupleObject(DrawableObject objectToUncouple)
        {
            attachedObjects.Remove(objectToUncouple);
        }

        /// <summary>
        /// Uncouples the name of the object by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param rreturns>Returns the object you removed.</param>
        public DrawableObject uncoupleObjectByName(String name)
        {
			foreach(DrawableObject obj in attachedObjects)
			{
				if(obj.Name == name)
				{
					attachedObjects.Remove(obj);
					return obj;
				}
			}
			
            return null;
        }

        public override string ToString()
        {
            return (base.ToString() + " -> \"" + this.Name + "\"");
        }

        #endregion 
    }
}
