﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManhattanMorning.Model.GameObject;

namespace ManhattanMorning.Misc.Tasks
{

    /// <summary>
    /// Add Tasks your controller can perform here.
    /// </summary>
    public class PowerUpManagerTask : Task
    {

        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        #endregion

        #region Members


        #endregion


        #region Initialization
        // Feel free to overload the constructors!

        /// <summary>
        /// Basic Contructor of the PowerUpManagerTask.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        public PowerUpManagerTask(int time)
            : base(Sender.PowerupManager, time)
        {

        }
        
        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
