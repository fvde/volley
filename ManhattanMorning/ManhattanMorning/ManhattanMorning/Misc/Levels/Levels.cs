using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;
using ManhattanMorning.Model;
using ManhattanMorning.Controller;

namespace ManhattanMorning.Misc.Levels
{


    /// <summary>
    /// Contains the data for the actual levels
    /// </summary>
    public class Levels
    {

        #region Properties

        /// <summary>
        /// List with a preview for all loadable levels
        /// </summary>
        public LayerList<LayerInterface> LevelPreviews
        {
            get { return levelPreviews; }
        }

        #endregion



        # region Members

        /// <summary>
        /// List with a preview for all loadable levels
        /// </summary>
        private LayerList<LayerInterface> levelPreviews;


        #endregion



        #region Initialization

        /// <summary>
        /// Initializes the levels class
        /// </summary>
        public Levels()
        {


            // Load levelPreviews
            List<String> levelNames = new List<string>();
            foreach (String levelName in ImplementedLevels().Keys)
                levelNames.Add(levelName);

            levelPreviews = StorageManager.Instance.LoadLevelPreviews(levelNames);

        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a ist with a preview for all loadable levels
        /// </summary>
        /// <returns>The list with all implemented levels</returns>
        public Dictionary<String, Level> ImplementedLevels()
        {

            // Initialize a new dictionary for all the levels
            Dictionary<String, Level> implementedLevels = new Dictionary<string, Level>();

            // Add all loadable levels
            Level level;

            // ----------------------------------------------------------------------
            // Register your levels here
            level = new Beach();
            implementedLevels.Add((String)level.LevelProperties["LevelName"], level);

            level = new Maya();
            implementedLevels.Add((String)level.LevelProperties["LevelName"], level);

            level = new Forest();
            implementedLevels.Add((String)level.LevelProperties["LevelName"], level);
            // ----------------------------------------------------------------------

            return implementedLevels;

        }

        #endregion


    }


}
