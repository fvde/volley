
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
        bombBig,
        bombSmall,
        invertedControl,
        smashBall,
        sunsetPowerup,
        windPowerup,
        mayaStoneChange,
        startWhistle,
        applauseGameEnd,
        powerUpSpawn,
        matchballHeartbeat,
        bottomTouchBeach,
        bottomTouchMaya,
        bottomTouchForest
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
        public SoundIndicator SoundType { get { return soundType; } set { soundType = value; } }

        /// <summary>
        /// Duration of the time in MS, sound is looped if it is too short
        /// 0 if no looping is required
        /// </summary>
        public int LoopingDuration { get { return loopingDuration; } set { loopingDuration = value; } }

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

        /// <summary>
        /// Duration of the time in MS, sound is looped if it is too short
        /// 0 if no looping is required
        /// </summary>
        int loopingDuration = 0;

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

        /// <summary>
        /// Plays a SoundEffect with the given number or enum.
        /// Flag indicates which kind of sound it is.
        /// </summary>
        /// <param name="time">Time in ms until the task is executed</param>
        /// <param name="soundType">Type of sound you want to play.</param>
        /// <param name="SoundEffectNumber">Number of the sound effect you want to play.</param>
        /// <param name="loopingDuration">Duration of the time in MS, sound is looped if it is too short</param>
        public SoundTask(int time, SoundIndicator soundType, int SoundEffectNumber, int loopingDuration)
            : base(Sender.SoundEngine, time)
        {
            this.soundEffectNumber = SoundEffectNumber;
            this.soundType = soundType;
            this.loopingDuration = loopingDuration;
        }

        #endregion

        #region Methods
        /// place for methods; remove when adding some.
        #endregion
    }
}
