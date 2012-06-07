using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Misc.Tasks
{
    public enum EffectIndicator
    {
        ballCollision,
        playerJump,
        wind
    }

    /// <summary>
    /// Tasks of the ParticleSystemsManager. Add Tasks your controller can perform here.
    /// </summary>
    public class ParticleSystemTask : Task
    {

        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// Position of the particle effect.
        /// </summary>
        public Vector2 ParticleEffectPosition { get { return particleEffectPosition; } set { particleEffectPosition = value; } }

        /// <summary>
        /// Direction of the particle effect.
        /// </summary>
        public Vector2 ParticleEffectDirection { get { return particleEffectDirection; } set { particleEffectDirection = value; } }

        /// <summary>
        /// Texture of the particle effect.
        /// </summary>
        public String ParticleEffectTexture { get { return particleEffectTexture; } set { particleEffectTexture = value; } }

        /// <summary>
        /// Type of the Effect. For example playerJump or ballCollision.
        /// </summary>
        public int EffectType { get { return effectType; } set { effectType = value; } }


        #endregion

        #region Members
        // Add all the information that you need to send to
        // other controllers here.

        private Vector2 particleEffectPosition;

        private Vector2 particleEffectDirection;

        private String particleEffectTexture;

        private int effectType;

        #endregion


        #region Initialization
        // Feel free to overload the constructors!


        /// <summary>
        /// Basic Contructor of the GameLogicTask.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        public ParticleSystemTask(int time)
            : base(Sender.ParticleSystemsManager, time)
        {

        }

        /// <summary>
        /// Creates a ParticleEffect at a given Position.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        /// <param name="pos">Position of the ParticleEffect </param>
        /// <param name="type">Type of the effect. </param>
        public ParticleSystemTask(int time, Vector2 pos, EffectIndicator type)
            : base(Sender.ParticleSystemsManager, time)
        {
            particleEffectPosition = pos;
            effectType = (int)type;
        }

        /// <summary>
        /// Creates a ParticleEffect at a given Position.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        /// <param name="pos">Position of the ParticleEffect </param>
        /// <param name="type">Type of the effect. </param>
        /// <param name="dir">Direction of the effect</param>
        /// <param name="texture">Texture of the effect</param>
        public ParticleSystemTask(int time, Vector2 pos, Vector2 dir, String texture, EffectIndicator type)
            : this(time, pos, type)
        {
            particleEffectDirection = dir;
            particleEffectTexture = texture;
        }

        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
