using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManhattanMorning.Misc;

namespace ManhattanMorning.Misc.Tasks
{


    /// <summary>
    /// Tasks of the SoundManager. Add Tasks your controller can perform here.
    /// </summary>
    public class AnimationTask : Task
    {

        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The animation that should be activated delayed
        /// </summary>
        public Animation DelayedAnimation { get { return delayedAnimation; } set { delayedAnimation = value; } }

        #endregion

        #region Members
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The animation that should be activated delayed
        /// </summary>
        private Animation delayedAnimation;

        #endregion


        #region Initialization
        // Feel free to overload the constructors!


        /// <summary>
        /// Basic Contructor of the AnimationTask
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        public AnimationTask(int time)
            : base(Sender.Animationmanager, time)
        {

        }

        /// <summary>
        /// Plays a SoundEffect with the given number or enum.
        /// Flag indicates which kind of sound it is.
        /// </summary>
        /// <param name="time">Time in ms until the task is executed</param>
        /// <param name="animation">The animation that should be started</param>
        public AnimationTask(int time,  Animation animation)
            : base(Sender.Animationmanager, time)
        {

            // Set members
            this.delayedAnimation = animation;

        }

        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
