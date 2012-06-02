using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManhattanMorning.Misc.Logic
{
    public class TimeLimit_WinCondition : WinCondition
    {

        #region Properties

        /// <summary>
        /// The time limit of the game
        /// </summary>
        public TimeSpan TimeLimit { get { return this.timeLimit; } set { this.timeLimit = value; } }

        #endregion

        #region Members

        /// <summary>
        /// The time limit of the game
        /// </summary>
        private TimeSpan timeLimit;


        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a TimeLimie_WinCondition
        /// </summary>
        /// <param name="timeSpan">Game duration</param>
        public TimeLimit_WinCondition(TimeSpan timeSpan)
            : base()
        {

            this.timeLimit = timeSpan;
            
        }

        #endregion

        #region Methods

        #endregion

    }
}
