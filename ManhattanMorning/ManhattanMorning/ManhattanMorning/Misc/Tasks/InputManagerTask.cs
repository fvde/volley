using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManhattanMorning.Misc.Tasks
{
    /// <summary>
    /// Tasks of the InputProcessor. Add Tasks your controller can perform here.
    /// </summary>
    public class InputManagerTask : Task
    {

        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        #endregion

        #region Members
        // Add all the information that you need to send to
        // other controllers here.

        #endregion


        #region Initialization
        // Feel free to overload the constructors!

        /// <summary>
        /// Basic Contructor of the InputManagerTask.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        public InputManagerTask(int time)
            : base(Sender.InputManager, time)
        {

        }
        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
