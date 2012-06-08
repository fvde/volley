using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;
using ManhattanMorning.Misc.Curves;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model;

namespace ManhattanMorning.Misc.Levels
{

    /// <summary>
    /// Class that defines the structure for a level
    /// </summary>
    public abstract class Level
    {

        #region Properties

        /// <summary>
        /// Dictionary with all the static properties like levelname or levelsize
        /// </summary>
        public Dictionary<String, Object> LevelProperties { get { return levelProperties; } }

        /// <summary>
        /// List with all level objects
        /// </summary>
        public List<LayerInterface> LevelObjectsList { get { return levelObjectsList; } }

        /// <summary>
        /// Dictionary with all the static properties like levelname or levelsize
        /// </summary>
        public List<PowerUpType> AllowedPowerUps { get { return allowedPowerups; } }

        #endregion


        #region Members

        /// <summary>
        /// Dictionary with all the static properties like levelname or levelsize
        /// </summary>
        protected Dictionary<String, Object> levelProperties;

        /// <summary>
        /// List with all level objects
        /// </summary>
        protected List<LayerInterface> levelObjectsList;

        /// <summary>
        /// List with all level objects
        /// </summary>
        private List<PowerUpType> allowedPowerups;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a Level
        /// </summary>
        public Level()
        {

            levelObjectsList = new List<LayerInterface>();
            levelProperties = new Dictionary<string, object>();
            allowedPowerups = new List<PowerUpType>();
        }

        virtual public void load()
        {
        }

        #endregion

    }
}
