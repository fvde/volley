using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ManhattanMorning.Model.ParticleSystem;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Used to fade DrawableObjects.
    /// </summary>
    public class ScalingAnimation : Animation
    {

        #region Properties

        /// <summary>
        /// Time since the fading of the animation started in MS.
        /// </summary>
        public int TimeSinceFadingStarted { get { return timeSinceFadingStarted; } set { timeSinceFadingStarted = value; } }

        /// <summary>
        /// Duration of the fading effect in MS.
        /// </summary>
        public int FadingTime { get { return fadingTime; } set { fadingTime = value; } }

        /// <summary>
        /// When fading in with alpha, this sets how the alpha value is calculated.
        /// Modes: none, linear, quadratic
        /// </summary>
        public FadeMode FadeMode { get { return fadeMode; } set { fadeMode = value; timeSinceFadingStarted = 0; } }

        /// <summary>
        /// The waiting duration (in ms) before the animation is fading out
        /// </summary>
        public int DurationBeforeReverse { get { return durationBeforeReverse; } set { durationBeforeReverse = value; } }

        /// <summary>
        /// Pauses the fading animation
        /// </summary>
        public bool Paused { get { return paused; } set { paused = value; } }

        /// <summary>
        /// Reset the alpha values to the initial values after fading is over.
        /// </summary>
        public bool ResetAfterFade
        {
            get { return resetAfterFade; }
            set { resetAfterFade = value; }
        }

        /// <summary>
        /// Y is the minimum scaling factor and Z the maximum scaling factor of the object.
        /// </summary>
        public Vector2 ScalingRange
        {
            get { return scalingRange; }
            set { scalingRange = value; }
        }

        /// <summary>
        /// The current scaling value of the object.
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// If set, the fading duration will be a random number between x and y values of the vector.
        /// Z and W values represent the duration between 2 fades.
        /// The fade will repeat endlessly.
        /// 
        /// Values are in milliseconds.
        /// </summary>
        public Vector4 RandomFadeDuration
        {
            get { return randomFadeDuration; }
            set { randomFadeDuration = value; }
        }

        #endregion

        #region Members

        /// <summary>
        /// When fading in with alpha, this sets how the alpha value is calculated.
        /// Modes: none, linear, quadratic
        /// </summary>
        private FadeMode fadeMode = FadeMode.Linear;

        /// <summary>
        /// Time since the fading of the animation started in MS.
        /// </summary>
        private int timeSinceFadingStarted;

        /// <summary>
        /// Duration of the fading effect.
        /// </summary>
        private int fadingTime;

        /// <summary>
        /// The waiting duration (in ms) before the animation is fading out
        /// </summary>
        private int durationBeforeReverse;

        /// <summary>
        /// Pauses the fading animation
        /// </summary>
        private bool paused;

        /// <summary>
        /// Reset the alpha values to the initial values after fading is over.
        /// </summary>
        private bool resetAfterFade;

        /// <summary>
        /// Y is the minimum scaling factor and Z the maximum scaling factor of the object.
        /// </summary>
        private Vector2 scalingRange;

        /// <summary>
        /// The current scaling value of the object.
        /// </summary>
        private float scale = 1f;

        /// <summary>
        /// If set, the fading duration will be a random number between x and y values of the vector.
        /// Z and W values represent the duration between 2 fades.
        /// The fade will repeat endlessly.
        /// 
        /// Values are in milliseconds.
        /// </summary>
        private Vector4 randomFadeDuration = Vector4.Zero;

        #endregion


        #region Initialization

        /// <summary>
        /// Creates a ScalingAnimation with linear fading as standard.
        /// </summary>
        /// <param name="looping">Do you want to repeat the animation?</param>
        /// <param name="reverse">Shall the animation play first in ascending and then in descending order?</param>
        /// <param name="durationBeforeReverse">The waiting duration (in ms) before the animation is fading out (0, if there's no reverse)</param>
        /// <param name="active">Is the animation active?</param>
        /// <param name="fadingTime">How long the fading takes place in MS.</param>
        public ScalingAnimation(bool looping, bool reverse, int durationBeforeReverse, bool active, int fadingTime)
            : base(looping, reverse, active)
        {
            this.fadeMode = FadeMode.Linear;
            this.fadingTime = fadingTime;
            this.durationBeforeReverse = durationBeforeReverse;

            paused = false;
        }

        /// <summary>
        /// Creates a ScalingAnimation.
        /// </summary>
        /// <param name="looping">Do you want to repeat the animation?</param>
        /// <param name="reverse">Shall the animation play first in ascending and then in descending order?</param>
        /// <param name="durationBeforeReverse">The waiting duration (in ms) before the animation is fading out (0, if there's no reverse)</param>
        /// <param name="active">Is the animation active?</param>
        /// <param name="fadingTime">How long the fading takes place in MS.</param>
        /// <param name="fadeMode">The way the animation fades in and out.</param>
        public ScalingAnimation(bool looping, bool reverse, int durationBeforeReverse, bool active, int fadingTime, FadeMode fadeMode)
            : this(looping, reverse, durationBeforeReverse, active, fadingTime)
        {
            this.fadeMode = fadeMode;
        }

        #endregion

        #region Methods



        #endregion


    }
}
