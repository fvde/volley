using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManhattanMorning.Misc.Logic
{
    public class ScoreLimit_WinCondition: WinCondition
    {

        #region Properties

        /// <summary>
        /// The score which defines the end of a game
        /// </summary>
        public int WinningScore { get { return this.winningScore; } set { this.winningScore = value; } }

        #endregion

        #region Members

        /// <summary>
        /// The score which defines the end of a game
        /// </summary>
        private int winningScore;


        #endregion


        #region Initialization

        /// <summary>
        /// Initialies a ScoreLimit_WinCondition
        /// </summary>
        /// <param name="winningScore">The score which defines the end of a game</param>
        public ScoreLimit_WinCondition(int winningScore)
            : base()
        {

            this.winningScore = winningScore;

        }

        #endregion

        #region Methods

        #endregion

    }
}
