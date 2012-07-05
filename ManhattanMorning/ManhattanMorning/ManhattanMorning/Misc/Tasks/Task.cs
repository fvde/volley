using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManhattanMorning.Misc;

namespace ManhattanMorning.Misc.Tasks
{

    /// <summary>
    /// Tasks are designed to act as (delayed) messages between controllers. The can be used to trigger effects, GUI elements or logic events.
    /// HOW TO USE:
    /// If your class doesn't have a YourClassTask class, then add one. Make sure to have it inherit from Task.
    /// Now you can add all the attributes, flags and members you want to your YourClassTask. 
    /// </summary>
    public class Task
    {

        #region Properties

        // all the variables, getters, setters right here

        /// <summary>
        /// Sender that this task has been adressed to.
        /// </summary>
        public Sender To { get { return to; } set { to = value; } }

        /// <summary>
        /// Current time of the task. The task will be executed at 0.
        /// </summary>
        public int CurrentTime { get { return currentTime; } set { currentTime = value; } }

        /// <summary>
        /// The time that the task started with. Can be used for repeated actions.
        /// </summary>
        public int MaximumTime { get { return maximumTime; } set { maximumTime = value; } }

        #endregion

        #region Members
        /// place for the members; remove when adding some.

        /// <summary>
        /// Sender that this task has been adressed to.
        /// </summary>
        private Sender to;

        /// <summary>
        /// Current time of the task. The task will be executed at 0.
        /// </summary>
        private int currentTime;

        /// <summary>
        /// The time that the task started with. Can be used for repeated actions.
        /// </summary>
        private int maximumTime;

        #endregion


        #region Initialization

        protected Task(Sender newTo, int time)
        {
            to = newTo;
            maximumTime = time;
            currentTime = time;
        }

        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
