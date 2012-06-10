using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManhattanMorning.Model.GameObject;

using Microsoft.Xna.Framework;

namespace ManhattanMorning.Misc.Tasks
{
    /// <summary>
    /// Tasks of the Physic. Add Tasks your controller can perform here.
    /// </summary>
    public class PhysicsTask : Task
    {
        public enum PhysicTaskType
        {
            RemoveStun,
            RemoveHandStun,
            Explosion,
            CreateNewBall,
            CreateDoubleBall,
            CreateLava,
            CreateSuperBomb,
            DetonateSuperBomb
        }

        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The affected player.
        /// </summary>
        public Player AffectedPlayer { get { return affectedPlayer; } set { affectedPlayer = value; } }

        /// <summary>
        /// The type of the task.
        /// </summary>
        public List<PhysicTaskType> Type { get { return type; } set { type = value; } }

        /// <summary>
        /// A position in meters
        /// </summary>
        public Vector2 Position { get { return position; } set { position = value; } }

        #endregion

        #region Members
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// The affected player.
        /// </summary>
        private Player affectedPlayer;

        /// <summary>
        /// A list of flags for this task. If you want to check whether a task has a flag just use list operations.
        /// </summary>
        private List<PhysicTaskType> type;

        /// <summary>
        /// A position in meters
        /// </summary>
        private Vector2 position;

        #endregion


        #region Initialization
        // Feel free to overload the constructors!

        /// <summary>
        /// Basic Contructor of the PhysicsTask.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        public PhysicsTask(int time, PhysicTaskType type)
            : base(Sender.Physics, time)
        {
            this.type = new List<PhysicTaskType>();
            this.type.Add(type);

        }

        /// <summary>
        /// Constructor for tasks which need an additional position
        /// </summary>
        /// <param name="time">Time until the task is carried out. (in ms) </param>
        /// <param name="position">Position which is needed in Task (in meters)</param>
        public PhysicsTask(int time, PhysicTaskType type, Vector2 position)
            : base(Sender.Physics, time)
        {
            this.type = new List<PhysicTaskType>();
            this.type.Add(type);

            this.position = position;
        }

        /// <summary>
        /// Constructor for tasks concerning physical effects on players.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        /// <param name="affectedPlayer">Player that is affected by this task.</param>
        /// <param name="type">Type of effect</param>
        public PhysicsTask(int time, Player affectedPlayer, PhysicTaskType type)
            : base(Sender.Physics, time)
        {
            this.type = new List<PhysicTaskType>();
            this.type.Add(type);
            this.affectedPlayer = affectedPlayer;

        }

        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
