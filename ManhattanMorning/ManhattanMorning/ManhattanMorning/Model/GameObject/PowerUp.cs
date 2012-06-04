using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.GameObject
{
    /// <summary>
    /// Types of PowerUps.
    /// </summary>
    /// 
    public enum PowerUpType
    {
        Wind,
        Jumpheight,
        InvertedControl,
        DoubleBall,
        BallRain,
        Volcano,
        SwitchStones,
        SunsetSunrise,
        SuperBomb
    };

    /// <summary>
    /// Behaviour of PowerUps. Some Powerups can be active multiple times, some are exclusive, etc.
    /// </summary>
    /// 
    public enum PowerUpBehaviour
    {
        GameExclusive,
        TeamExclusive,
        PlayerExclusive,
        NonExclusive
    };

    /// <summary>
    /// Version of the Powerup. Most Powerups can have good and bad effects, some can be neutral.
    /// 0 = positive, 1 = neutral, 2 = negative
    /// </summary>
    /// 
    public enum PowerUpVersion
    {
        Positive,
        Neutral
    };

    /// <summary>
    /// The powerUps are special features in the game. They will have great effect on the game plan.
    /// </summary>
    public class PowerUp : ActiveObject
    {

        #region Properties

        // all the variables, getters, setters right here

        /// <summary>
        /// Owner of the PowerUp. null if not yet picked up.
        /// </summary>
        public Player Owner { get { return owner; } set { owner = value; } }

        /// <summary>
        /// Remaining lifetime of the PowerUp in MILLISECONDS.
        /// </summary>
        public int LifeTime { get { return lifeTime; } set { lifeTime = value; } }

        /// <summary>
        /// Remaining time before the PowerUp will be removed if its not picked up. In MILLISECONDS.
        /// </summary>
        public int RemoveTime { get { return removeTime; } set { removeTime = value; } }

        /// <summary>
        /// Type of the PowerUp.
        /// </summary>
        public PowerUpType PowerUpType { get { return type; } set { type = value; } }

        /// <summary>
        /// Behaviour of the PowerUp. Indicates if a powerup can be active for multiple times, or if it is exclusive. 
        /// </summary>
        public PowerUpBehaviour PowerUpBehaviour { get { return behaviour; } set { behaviour = value; } }

        /// <summary>
        /// Version of the Powerup. Most Powerups can have good and bad effects, some can be neutral.
        /// </summary>
        public PowerUpVersion PowerUpVersion { get { return version; } set { version = value; } }


        #endregion

        #region Members
        //place for the members; remove, when adding some.

        /// <summary>
        /// Owner of the PowerUp. null if not yet picked up.
        /// </summary>
        private Player owner;

        /// <summary>
        /// Remaining lifetime of the PowerUp in MILLISECONDS!
        /// </summary>
        private int lifeTime;

        /// <summary>
        /// Remaining time before the PowerUp will be removed if its not picked up. In MILLISECONDS.
        /// </summary>
        private int removeTime;

        /// <summary>
        /// Type of the PowerUp.
        /// </summary>
        private PowerUpType type;

        /// <summary>
        /// Behaviour of the PowerUp. Indicates if a powerup can be active for multiple times, or if it is exclusive. 
        /// </summary>
        private PowerUpBehaviour behaviour;

        /// <summary>
        /// Version of the Powerup. Most Powerups can have good and bad effects, some can be neutral.
        /// </summary>
        private PowerUpVersion version;

        #endregion

        #region Initialization
        /// <summary>
        /// Intialization of a PowerUp.
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>
        /// <param name="measurementUnit">The measurement unit. </param>
        /// <param name="life">The duration of the PowerUp, once picked up.</param>
        /// <param name="newType">The type of the PowerUp. </param>
        public PowerUp(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation, Vector2 size, Vector2 position, int layer, 
            MeasurementUnit measurementUnit, int life, PowerUpType newType, PowerUpBehaviour newBehaviour, PowerUpVersion newVersion)
            : base(name,visible, texture, shadowTexture, animation, size, position, layer,measurementUnit)
        {
            lifeTime = life;
            type = newType;
            owner = null;
            behaviour = newBehaviour;
            version = newVersion;
            removeTime = 5000;
        }

        #endregion

        #region Methods
        //place for the methods; remove, when adding some.
        #endregion

    }
}
