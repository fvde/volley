using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Used to animate things. Animations can't stand alone. They are always part of objects.
    /// </summary>
    public class SpriteAnimation : Animation
    {

        #region Properties

        /// <summary>
        /// Returns the source rectanglt of the Animation depending on the current frame. Cannot be set.
        /// </summary>
        public Rectangle SourceRectangle
        {
            get
            { return new Rectangle((currentFrame % numCols) * frameWidth, (currentFrame / numCols) * frameHeight, frameWidth, frameHeight); }
        }

        /// <summary>
        /// Time between 2 Frames.
        /// Values: millisecs as int
        /// </summary>
        public int FrameTime { get { return frameTime; } set { frameTime = value; } }

        /// <summary>
        /// The number of frames that the animation contains
        /// </summary>
        public int FrameCount { get { return frameCount; } set { frameCount = value; } }

        /// <summary>
        /// The index of the current frame we are displaying
        /// </summary>
        public int CurrentFrame { get { return currentFrame; } set { currentFrame = value; } }

        /// <summary>
        /// Width of a given frame
        /// </summary>
        public int FrameWidth { get { return frameWidth; } set { frameWidth = value; } }

        /// <summary>
        /// Height of a given frame
        /// </summary>
        public int FrameHeight { get { return frameHeight; } set { frameHeight = value; } }

        /// <summary>
        /// The time since we last updated the frame
        /// </summary>
        public int ElapsedTime { get { return elapsedTime; } set { elapsedTime = value; } }

        /// <summary>
        /// Texture of the Animation.
        /// </summary>
        public Texture2D SpriteStrip { get { return spriteStrip; } set { spriteStrip = value; } }

        /// <summary>
        /// Number of Columns.
        /// </summary>
        public int NumCols { get { return numCols; } set { numCols = value; } }

        /// <summary>
        /// Number of Rows.
        /// </summary>
        public int NumRows { get { return numRows; } set { numRows = value; } }

        /// <summary>
        /// Resets the animation to the first frame and deactivates it.
        /// </summary>
        /// 
        public bool Reset { get { return reset; } set { reset = value; } }
        /// <summary>
        /// If true, animation will be deactivated after one half of reverse animation has been played.
        /// It'll resume when you set active to true, after it has been paused.
        /// </summary>
        public bool WaitOnReverse
        {
            get { return waitOnReverse; }
            set { waitOnReverse = value; }
        }

        /// <summary>
        /// Name of the texture.
        /// </summary>
        public String TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }

        #endregion

        #region Members

        /// <summary>
        /// The image representing the collection of images used for animation
        /// </summary>
        private Texture2D spriteStrip;

        /// <summary>
        /// The time since we last updated the frame
        /// </summary>
        private int elapsedTime;

        /// <summary>
        /// The time we display a frame until the next one
        /// </summary>
        private int frameTime;

        /// <summary>
        /// The number of frames that the animation contains
        /// </summary>
        private int frameCount;

        /// <summary>
        /// The index of the current frame we are displaying
        /// </summary>
        private int currentFrame;

        /// <summary>
        /// Width of a given frame
        /// </summary>
        private int frameWidth;

        /// <summary>
        /// Height of a given frame
        /// </summary>        
        private int frameHeight;

        /// <summary>
        /// Number of Columns.
        /// </summary>
        private int numCols;

        /// <summary>
        /// Number of Rows.
        /// </summary>
        private int numRows;

        /// <summary>
        /// Resets the animation to the first frame and deactivates it.
        /// </summary>
        private bool reset;

        /// <summary>
        /// If true, animation will be deactivated after one half of reverse animation has been played.
        /// It'll resume when you set active to true, after it has been paused.
        /// </summary>
        private bool waitOnReverse;

        /// <summary>
        /// Name of the texture.
        /// </summary>
        private String textureName;        

        #endregion


        #region Initialization

        /// <summary>
        /// Creates an animation with a spritestrip.
        /// </summary>
        /// <param name="spriteStrip">Texture of the animation.</param>
        /// <param name="frameTime">Timespan until the next frame of the animation is shown.</param>
        /// <param name="frameCount">Number of frames the animation has.</param>
        /// <param name="frameWidth">Width of the displayed segment.</param>
        /// <param name="frameHeight">Height of the displayed segment.</param>
        /// <param name="looping">Do you want the animation repeat?</param>
        /// <param name="reverse">Shall the animation play first in ascending and then in descending order?</param>
        /// <param name="active">Is the animation active?</param>
        public SpriteAnimation(Texture2D spriteStrip, int frameTime, int frameCount, int frameWidth, int frameHeight, int numCols, int numRows, bool looping, bool reverse, bool active)
            : base(looping, reverse, active)
        {
            this.spriteStrip = spriteStrip;
            this.frameTime = frameTime;
            this.frameCount = frameCount;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.numCols = numCols;
            this.numRows = numRows;
        }

        /// <summary>
        /// Creates an animation with a spritestrip.
        /// </summary>
        /// <param name="textureName">TextureName of the animation.</param>
        /// <param name="frameTime">Timespan until the next frame of the animation is shown.</param>
        /// <param name="frameCount">Number of frames the animation has.</param>
        /// <param name="frameWidth">Width of the displayed segment.</param>
        /// <param name="frameHeight">Height of the displayed segment.</param>
        /// <param name="looping">Do you want the animation repeat?</param>
        /// <param name="reverse">Shall the animation play first in ascending and then in descending order?</param>
        /// <param name="active">Is the animation active?</param>
        public SpriteAnimation(String textureName, int frameTime, int frameCount, int frameWidth, int frameHeight, int numCols, int numRows, bool looping, bool reverse, bool active)
            : base(looping, reverse, active)
        {
            this.textureName = textureName;
            this.frameTime = frameTime;
            this.frameCount = frameCount;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.numCols = numCols;
            this.numRows = numRows;
        }

        /// <summary>
        /// Creates a Animation object
        /// </summary>
        /// <param name="textureName">The name of the animation texture</param>
        /// <param name="frameTime">Defines how long a single frame should be shown (in ms)</param>
        /// <param name="frameCount">The number of frames the animation consists of</param>
        /// <param name="frameWidth">The width of a single frame (in pixels)</param>
        /// <param name="frameHeight">The heigt of a single frame (in pixels)</param>
        /// <param name="numberOfCols">The number of columns in the animation texture</param>
        /// <param name="numberOfRows">The number of rows in the animation texture</param>
        /// <param name="looping">Defines if the animation should be played once(false) or in a loop(true)</param>
        public SpriteAnimation(String textureName, int frameTime, int frameCount, int frameWidth, int frameHeight, int numberOfCols, int numberOfRows, bool looping)
        {
            // Save all parameters
            this.TextureName = textureName;
            this.FrameTime = frameTime;
            this.FrameCount = frameCount;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.numCols = numberOfCols;
            this.numRows = numberOfRows;
            this.Looping = looping;
        }

        #endregion


        #region Methods



        #endregion


    }
}
