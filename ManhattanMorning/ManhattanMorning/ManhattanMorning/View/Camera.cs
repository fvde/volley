using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;
using ManhattanMorning.Controller;


namespace ManhattanMorning.View
{
    /// <summary>
    /// Represents the display window, that is actually displayed onscreen
    /// </summary>
    class Camera : IObserver
    {

        #region Properties

        /// <summary>
        /// Position of the Camera's center Point ( in Pixel)
        /// </summary>
        public Vector2 Position { get { return this.position; } set { this.position = value; } }

        /// <summary>
        /// Camera's zoom clamped to values greater equal Zero
        /// Zoom value between 0 and "inf". 1.0f is real size
        /// </summary>
        public float Zoom { get {return this.zoom ;} set { this.zoom = Math.Abs(value); } }

        /// <summary>
        /// Rotation angle in radians
        /// </summary>
        public float Rotation { get { return this.rotation; } set { this.rotation = value; } }

        /// <summary>
        /// Width of Camera's Viewport (in Pixel)
        /// </summary>
        public float ViewPortWidth { get { return this.viewPortWidth; } set { this.viewPortWidth = value; } }

        /// <summary>
        /// Height of Camera's Viewport (in Pixel)
        /// </summary>
        public float ViewPortHeight { get { return this.viewPortHeight; } set { this.viewPortHeight = value; } }


        #endregion

        #region Members

        // position of the Camera's center Point ( in Pixel)
        private Vector2 position;
        // zoom value between 0 and "inf". 1.0f is real size
        private float zoom;
        // rotation angle in radians
        private float rotation;
        // width of camera's viewport ( in Pixel)
        private float viewPortWidth;
        // height of camera's viewport ( in Pixel)
        private float viewPortHeight;
        //Logger
        private Logger logger;

        /// <summary>
        /// Instance of the settingsManager
        /// Used to apply settings concerning the graphic
        /// </summary>
        private static SettingsManager settings = SettingsManager.Instance;
        
        #endregion


        #region Initialization

        // all constructors right here
        /// <summary>
        /// Creates a new Camera for use in the Graphics Engine
        /// </summary>
        /// <param name="viewPortWidth">Width of the current Viewport</param>
        /// <param name="viewPortHeight">Height of the current Viewport</param>
        /// <param name="staringPosition">Position of the Camera's center Point</param>
        /// <param name="zoom">Zoom value between 0 and "inf". 1.0f is real size</param>
        /// <param name="rotation">Rotation angle in radians</param>
        public Camera(float viewPortWidth, float viewPortHeight, Vector2 staringPosition, float zoom, float rotation)
        {
            this.position = staringPosition;
            this.Zoom = zoom;
            this.rotation = rotation;
            this.viewPortWidth = viewPortWidth;
            this.viewPortHeight = viewPortHeight;
            this.logger = Logger.Instance;
            // Register as an observer for SettingsManager
            settings.registerObserver(this);
        }

        /// <summary>
        /// Creates a new Camera for use in the Graphics Engine
        /// </summary>
        /// <param name="viewPortWidth">Width of the current Viewport</param>
        /// <param name="viewPortHeight">Height of the current Viewport</param>
        public Camera(float viewPortWidth, float viewPortHeight)
        {
            this.position = new Vector2(viewPortWidth / 2, viewPortHeight / 2);
            this.zoom = 1.0f;
            this.rotation = 0.0f;
            this.viewPortWidth = viewPortWidth;
            this.viewPortHeight = viewPortHeight;
            this.logger = Logger.Instance;
            // Register as an observer for SettingsManager
            settings.registerObserver(this);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Calculates a Matrix from all Camera Parameters
        /// </summary>
        /// <returns>All in One Matrix </returns>
        public Matrix getCameraMatrix()
        {
            return
                //Translate to top left Corner of ViewPort
                Matrix.CreateTranslation(new Vector3(-this.viewPortWidth / 2, -this.viewPortHeight / 2, 0)) *
                //Rotate
                Matrix.CreateRotationZ(this.rotation) *
                //Scale
                Matrix.CreateScale(this.Zoom)*
                //Translate back to centerPoint of Camera
                Matrix.CreateTranslation(new Vector3(this.position.X, this.position.Y, 0));
        }

        /// <summary>
        /// Update Camera parameters
        /// </summary>
        /// <param name="position">any 2d Position in World (Not clamped!)</param>
        /// <param name="rotation">Rotation in radians</param>
        /// <param name="zoom">between 0 and "inf", standard is 1.0f</param>
        public void update(Vector2 position, float rotation, float zoom)
        {
            this.position = position;
            this.Zoom = zoom;
            this.rotation = rotation;
        }

        /// <summary>
        /// Has to be Implemented to Listen to the SettingsManager
        /// </summary>
        /// <param name="o">The Observable Object that sends the Message</param>
        public void notify(ObservableObject o)
        {
            //Check if we are Dealing with the right ObservableObject
            if (o is SettingsManager)
            {
                logger.log(Sender.Graphics, this.ToString() + " received a notification from the Settings Manager",PriorityLevel.Priority_2);
                this.zoom = (float)((SettingsManager)o).get("cameraZoomLevel");
                this.position = (Vector2)((SettingsManager)o).get("cameraPosition");
            }
        }


        #endregion

    }
}
