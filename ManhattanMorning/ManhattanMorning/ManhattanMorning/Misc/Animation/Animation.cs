using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ManhattanMorning.Model.ParticleSystem;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Used to fade DrawableObjects.
    /// </summary>
    public class Animation
    {

        #region Properties

        /// <summary>
        /// Loops the animation.
        /// Values: true/false
        /// </summary>
        public bool Looping { get { return looping; } set { looping = value; } }

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
        /// Indicates the step size of the animation (mostly used to go backwards).
        /// </summary>
        public int AnimationDirection { get { return animationDirection; } set { animationDirection = value; } }

        /// <summary>
        /// True if starting value is interpreted as ending value. (So 1.0f is interpreted as starting value and 0f as ending).
        /// </summary>
        public bool Inverted
        {
            get { return inverted; }
            set
            {
                inverted = value;
                animationDirection = (inverted) ? -1 : 1;
            }
        }

        /// <summary>
        /// Loops the Animation x times.
        /// </summary>
        public int LoopTimes
        {
            get { return loopTimes; }
            set { loopTimes = value; }
        }

        /// <summary>
        /// Loops the Animation for x milliseconds.
        /// </summary>
        public int LoopTime
        {
            get { return loopTime; }
            set { loopTime = value; }
        }


        /// <summary>
        /// Counts the amount of loops.
        /// </summary>
        public int CountLoops
        {
            get { return countLoops; }
            set { countLoops = value; }
        }

        /// <summary>
        /// Time in ms since the whole Animation started.
        /// </summary>
        public int TimeSinceTotalAnimationStarted
        {
            get { return timeSinceTotalAnimationStarted; }
            set { timeSinceTotalAnimationStarted = value; }
        }

        #endregion

        #region Members

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
        /// Indicates the step size of the animation (mostly used to go backwards).
        /// </summary>
        private int animationDirection = 1;

        /// <summary>
        /// True if starting value is interpreted as ending value. (So 1.0f is interpreted as starting value and 0f as ending).
        /// </summary>
        private bool inverted;

        /// <summary>
        /// Loops the Animation x times.
        /// </summary>
        private int loopTimes;
        
        /// <summary>
        /// Loops the Animation for x milliseconds.
        /// </summary>
        private int loopTime;

        /// <summary>
        /// Time in ms since the whole Animation started.
        /// </summary>
        private int timeSinceTotalAnimationStarted;        

        /// <summary>
        /// Counts the amount of loops.
        /// </summary>
        private int countLoops;        
        
        #endregion


        #region Initialization

        /// <summary>
        /// Creates a FadingAnimation with linear fading as standard.
        /// </summary>
        /// <param name="looping">Do you want to repeat the animation?</param>
        /// <param name="reverse">Shall the animation play first in ascending and then in descending order?</param>
        /// <param name="active">Is the animation active?</param>
        public Animation(bool looping, bool reverse, bool active)
        {
            this.looping = looping;
            this.reverse = reverse;
            this.active = active;
        }

        public Animation()
        {
        }

        #endregion

        #region Methods



        #endregion


    }
}
