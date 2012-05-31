﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManhattanMorning.Misc.Tasks
{
    public enum SoundIndicator
    {
        playerJump,
        hitBall,
        netCollision,
        pickupPowerup,
        bottomCollision,
        bombBig,
        bombSmall,
        invertedControl,
        smashBall,
        sunsetPowerup,
        windPowerup,
        mayaStoneChange,
        startWhistle
    }

    /// <summary>
    /// Tasks of the SoundManager. Add Tasks your controller can perform here.
    /// </summary>
    public class SoundTask : Task
    {

        #region Properties

        // All the variables, getters, setters right here.
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// Number of the sound effect you want to play.
        /// </summary>
        public int SoundEffectNumber { get { return soundEffectNumber; } set { soundEffectNumber = value; } }

        /// <summary>
        /// Soundtype which we want to play.
        /// </summary>
        public SoundIndicator SoundType { get { return soundType; } set { SoundType = value; } }

        #endregion

        #region Members
        // Add all the information that you need to send to
        // other controllers here.

        /// <summary>
        /// Number of the sound effect you want to play.
        /// </summary>
        int soundEffectNumber = -1;

        /// <summary>
        /// Soundtype which we want to play.
        /// </summary>
        SoundIndicator soundType;

        #endregion


        #region Initialization
        // Feel free to overload the constructors!


        /// <summary>
        /// Basic Contructor of the SoundTask.
        /// </summary>
        /// <param name="time">Time until the task is carried out. </param>
        public SoundTask(int time)
            : base(Sender.SoundEngine, time)
        {

        }

        /// <summary>
        /// Plays a SoundEffect with the given number or enum.
        /// Flag indicates which kind of sound it is.
        /// </summary>
        /// <param name="time">Time in ms until the task is executed</param>
        /// <param name="soundType">Type of sound you want to play.</param>
        /// <param name="SoundEffectNumber">Number of the sound effect you want to play.</param>
        public SoundTask(int time, SoundIndicator soundType, int SoundEffectNumber)
            : base(Sender.SoundEngine, time)
        {
            this.soundEffectNumber = SoundEffectNumber;
            this.soundType = soundType;
        }

        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
