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
    public class Animation
    {

        #region Properties

        /// <summary>
        /// Returns the source rectanglt of the Animation depending on the current frame. Cannot be set.
        /// </summary>
        public Rectangle SourceRectangle { get 
            { return new Rectangle((currentFrame % numCols) * frameWidth, (currentFrame / numCols) * frameHeight, frameWidth, frameHeight) ; } }

        /// <summary>
        /// Loops the animation.
        /// Values: true/false
        /// </summary>
        public bool Looping { get { return looping; } set { looping = value; } }

        /// <summary>
        /// Time between 2 Frames.
        /// Values: millisecs as int
        /// </summary>
        public int FrameTime { get { return frameTime; } set { frameTime = value; } }

        /// <summary>
        /// Whether the animation is active or not.
        /// Values: true/false
        /// </summary>
        public bool Active { get { return active; } set { active = value; } }

        /// <summary>
        /// Plays the animation in ascending order and then in descending order.
        /// Values: true/false
        /// </summary>
        public bool Reverse { get { return reverse; } set { reverse = value; } }

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
        /// Indicates the step size of the animation (mostly used to go backwards)
        /// </summary>
        public int FrameStep { get { return frameStep; } set { frameStep = value; } }

        /// <summary>
        /// The time since we last updated the frame
        /// </summary>
        public int ElapsedTime { get { return elapsedTime; } set { elapsedTime = value; } }

        /// <summary>
        /// Texture of the Animation.
        /// </summary>
        public Texture2D SpriteStrip { get { return spriteStrip; } set { SpriteStrip = value; } }

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
        public bool Reset { get { return reset; } set { reset = value; } }


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
        /// The state of the Animation
        /// </summary>
        private bool active;

        /// <summary>
        /// Determines if the animation will keep playing or deactivate after one run
        /// </summary>
        private bool looping;

        /// <summary>
        /// Determines if the animation first runs in ascending and then in descending order
        /// </summary>
        private bool reverse;

        /// <summary>
        /// Indicates the step size of the animation (mostly used to go backwards)
        /// </summary>
        private int frameStep;

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
        public Animation(Texture2D spriteStrip, int frameTime, int frameCount, int frameWidth, int frameHeight, int numCols, int numRows, bool looping, bool reverse, bool active)
        {
            this.spriteStrip = spriteStrip;
            this.frameTime = frameTime;
            this.frameCount = frameCount;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.numCols = numCols;
            this.numRows = numRows;
            this.looping = looping;
            this.reverse = reverse;
            this.active = active;

            elapsedTime = 0;
            currentFrame = 0;
            frameStep = 1;
        }

        #endregion


        #region Methods



        #endregion


    }
}
