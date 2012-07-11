using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManhattanMorning.Model.GameObject;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Misc.Tasks
{
    /// <summary>
    /// Starting: powerup was pickep up
    /// Finishing: effect of the powerup ended
    /// NotPickedUp: nobody picked up the powerup
    /// TextExpired: The text, indicating what the powerup does, expired.
    /// </summary>
    public enum PowerUpState
    {
        Starting,
        Finshing,
        NotPickedUp,
        TextExpired,
    }

    public enum GraphicTask
    {
        DisplayPowerUp,
        Sunset,
        Sunrise,
        LightEnable,
        LightDisable,
        CreateBall
    }

    /// <summary>
    /// Tasks of the Graphic. Add Tasks your controller can perform here.
    /// </summary>
    public class GraphicsTask : Task
    {

        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The powerup that triggered this task.
        /// </summary>
        public PowerUp TriggeringPowerUp { get { return triggeringPowerUp; } set { triggeringPowerUp = value; } }

        /// <summary>
        /// Current state of the powerup.
        /// </summary>
        public PowerUpState PowerUpState { get { return powerUpState; } set { powerUpState = value; } }

        /// <summary>
        /// Enum tells graphic what to do.
        /// </summary>
        public GraphicTask Task { get { return task; } set { task = value; } }

        /// <summary>
        /// Which team is affected. If you do not need this, set it to 0.
        ///                    0 = no team
        ///                    1 = left team
        ///                    2 = right team
        ///                    3 = both teams
        /// </summary>
        public int Team { get { return team; } set { team = value; } }

        /// <summary>
        /// Position of something.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        #region Members
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The powerup that triggered this task.
        /// </summary>
        private PowerUp triggeringPowerUp;

        /// <summary>
        /// The powerup that triggered this task.
        /// </summary>
        private PowerUpState powerUpState;

        /// <summary>
        /// Which team is affected. If you do not need this, set it to 0.
        ///                    0 = no team
        ///                    1 = left team
        ///                    2 = right team
        ///                    3 = both teams
        /// </summary>
        private int team;

        /// <summary>
        /// Enum tells graphic what to do.
        /// </summary>
        private GraphicTask task;

        /// <summary>
        /// Position of something.
        /// </summary>
        private Vector2 position;
        
        #endregion


        #region Initialization

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        /// <param name="whatToDo">Enum tells graphic what to do. </param>
        public GraphicsTask(int time, GraphicTask whatToDo)
            : base(Sender.Graphics, time)
        {
            task = whatToDo;
        }

        /// <summary>
        /// Tasks concerning visualization of HUD effects.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        /// <param name="whatToDo">Enum tells graphic what to do. </param>
        /// <param name="team">Which team is affected. If you do not need this, set it to 0.
        ///                    0 = no team
        ///                    1 = left team
        ///                    2 = right team
        ///                    3 = both teams</param>
        public GraphicsTask(int time, GraphicTask whatToDo, int team)
            : base(Sender.Graphics, time)
        {
            task = whatToDo;
            this.team = team;
        }

        /// <summary>
        /// Tasks Concerning the visualization of PowerUps.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        /// <param name="powerup">Triggering Powerup.</param>
        /// <param name="state">Current state of the powerup. </param>
        public GraphicsTask(int time, GraphicTask whatToDo, PowerUp powerup, PowerUpState state)
            : base(Sender.Graphics, time)
        {
            task = whatToDo;
            triggeringPowerUp = powerup;
            powerUpState = state;
        }
        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
