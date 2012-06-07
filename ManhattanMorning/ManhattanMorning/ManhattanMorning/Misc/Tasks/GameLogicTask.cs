using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManhattanMorning.Model;

namespace ManhattanMorning.Misc.Tasks
{
    /// <summary>
    /// Tasks of the GameLogic. Add Tasks your controller can perform here.
    /// </summary>
    public class GameLogicTask : Task
    {
        public enum GameLogicTaskType
        {
            RemoveStone
        }


        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The object you want to remove.
        /// </summary>
        public DrawableObject ObjectToRemove
        {
            get { return objectToRemove; }
            set { objectToRemove = value; }
        }

        /// <summary>
        /// Enum tells gameLogic what to do.
        /// </summary>
        public GameLogicTaskType Task
        {
            get { return task; }
            set { task = value; }
        }

        #endregion

        #region Members
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The object you want to remove.
        /// </summary>
        private DrawableObject objectToRemove;

        /// <summary>
        /// Enum tells gameLogic what to do.
        /// </summary>
        private GameLogicTaskType task;

        #endregion


        #region Initialization
        // Feel free to overload the constructors!

        /// <summary>
        /// Basic Contructor of the GameLogicTask.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        public GameLogicTask(int time, DrawableObject objectToRemove)
            : base(Sender.GameLogic, time)
        {
            this.objectToRemove = objectToRemove;
        }
        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
