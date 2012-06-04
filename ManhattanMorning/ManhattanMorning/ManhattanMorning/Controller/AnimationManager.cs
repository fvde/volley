using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ManhattanMorning.Model;
using ManhattanMorning.Misc;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model.ParticleSystem;

namespace ManhattanMorning.Controller
{
    /// <summary>
    /// Controlls and updates the Animations and moves passive objects.
    /// </summary>
    class AnimationManager : IController
    {

        #region Properties

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static AnimationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AnimationManager();
                }
                return instance;
            }
        }


        #endregion

        #region Members

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        private static AnimationManager instance;

        /// <summary>
        /// Counts time since last update.
        /// </summary>
        private int elapsedTime;

        /// <summary>
        /// Temporary alpha value.
        /// </summary>
        private float tempAlpha;

        /// <summary>
        /// Pauses the Controller.
        /// </summary>
        private bool paused;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random rng = new Random();

        #endregion

        #region Initialization

        /// <summary>
        /// Creates the animation manager
        /// </summary>
        private AnimationManager()
        {
        }

        #endregion


        #region Methods


        /// <summary>
        /// This Method is called by game1, all the functionality is implemented
        /// in this method or a submethod(update Animation)
        /// It updates all (visible) DrawableObjects in the the List.
        /// </summary>
        /// <param name="gameTime">recent gameTime</param>
        /// <param name="drawableObjectsList">recent List of DrawableObjects</param>
        public void update(GameTime gameTime, LayerList<LayerInterface> gameObjects)
        {
            //do nothing if this controller is paused
            if (paused) return;

            //only update every 15 ms
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            DrawableObject drawableObject = null;
            foreach (LayerInterface l in gameObjects)
            {
                if (l is DrawableObject)
                    drawableObject = l as DrawableObject;
                else
                    continue;

                //updates the animation of the object
                if (drawableObject.Animation != null && drawableObject.Visible)
                {
                    updateAnimation(drawableObject.Animation, gameTime);
                }
                //updates the alpha value for fading
                if (drawableObject.FadingAnimation != null)
                {
                    updateAlpha(drawableObject, gameTime);
                }
                if (drawableObject.PathAnimation != null && elapsedTime >= 15 && drawableObject.PathAnimation.Active)
                {
                    updateWaypointsAndPosition(gameTime, drawableObject.PathAnimation);
                }
                if (drawableObject.ScalingAnimation != null && drawableObject.ScalingAnimation.Active)
                {
                    updateScaling(drawableObject, gameTime);
                }
            }

            if (elapsedTime >= 15) elapsedTime -= 15;
        }

        /// <summary>
        /// Does all necessary action, to bring controller back to the state after initialization
        /// </summary>
        public void clear()
        {
            elapsedTime = 0;
            tempAlpha = 0f;
            paused = false;
        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        public void initialize()
        {

        }

        /// <summary>
        /// Forces controller to pause, has to make sure that all timers etc. pause
        /// </summary>
        /// <param name="on">true: controller has to pause, false: controller has to work </param>
        public void pause(bool on)
        {
            paused = on;
        }

        private void updateScaling(DrawableObject obj, GameTime gameTime)
        {
            ScalingAnimation sanim = obj.ScalingAnimation;

            // Do not update if animation is not active
            if (sanim.FadeMode == FadeMode.None) return;

            //update time since fading started
            sanim.TimeSinceFadingStarted += (int)(gameTime.ElapsedGameTime.TotalMilliseconds);
            sanim.TimeSinceTotalAnimationStarted += (int)(gameTime.ElapsedGameTime.TotalMilliseconds);

            bool reverse = (sanim.ScalingRange.X > sanim.ScalingRange.Y) ? true : false;
            //calculate alpha value depending on FadeValue
            float diff;
            switch (sanim.FadeMode)
            {
                case FadeMode.Linear:
                    diff = (sanim.TimeSinceFadingStarted / (float)sanim.FadingTime);
                    if(diff > 1) diff = 1;
                    if (!reverse)
                        sanim.Scale = (sanim.ScalingRange.Y - sanim.ScalingRange.X) * diff + sanim.ScalingRange.X;
                    else
                        sanim.Scale = sanim.ScalingRange.X - (sanim.ScalingRange.X - sanim.ScalingRange.Y) * diff;
                    break;
                case FadeMode.Quadratic:
                    diff = (sanim.TimeSinceFadingStarted / (float)sanim.FadingTime);
                    if (diff > 1) diff = 1;
                    if (!reverse)
                        sanim.Scale = (sanim.ScalingRange.Y - sanim.ScalingRange.X) * diff + sanim.ScalingRange.X;
                    else
                        sanim.Scale = sanim.ScalingRange.X - (sanim.ScalingRange.X - sanim.ScalingRange.Y) * diff;
                    break;
                case FadeMode.None:
                    break;
            }


            //fading is over, set clip alpha and set fadingMode to do nothing
            if (sanim.TimeSinceFadingStarted >= sanim.FadingTime)
            {
                sanim.TimeSinceFadingStarted = 0;
                sanim.CountLoops++;

                //negate animation direction when minimum or maximum is reached
                if (sanim.Reverse)
                {
                    if ((!sanim.Looping && sanim.CountLoops == 2) || sanim.CountLoops >= sanim.LoopTimes * 2 + 2)
                    {
                        sanim.Active = false;
                        sanim.CountLoops = 0;
                    }
                    else
                        sanim.ScalingRange = new Vector2(sanim.ScalingRange.Y, sanim.ScalingRange.X);
                }
                //disable animation if you don't want to reverse it
                else
                {
                    if (!sanim.Looping || sanim.CountLoops > sanim.LoopTimes + 1)
                    {
                        sanim.Active = false;
                        sanim.CountLoops = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the alpha values of object that are meant to fade.
        /// </summary>
        /// <param name="obj">Object that fades.</param>
        /// <param name="gameTime">Current gameTime.</param>
        private void updateAlpha(DrawableObject obj, GameTime gameTime)
        {
            FadingAnimation fanim = obj.FadingAnimation;

            // If paused just count durationTime down
            if (fanim.Paused)
            {

                fanim.DurationBeforeReverse -= (int)(gameTime.ElapsedGameTime.TotalMilliseconds);

                if (fanim.DurationBeforeReverse < 0)
                {
                    fanim.DurationBeforeReverse = 0;
                    fanim.Paused = false;
                }

            }
            else
            {
                // Do not update if animation is not active
                if (!fanim.Active || fanim.FadeMode == FadeMode.None) return;

                //update time since fading started
                fanim.TimeSinceFadingStarted += (int)(gameTime.ElapsedGameTime.TotalMilliseconds);
                fanim.TimeSinceTotalAnimationStarted += (int)(gameTime.ElapsedGameTime.TotalMilliseconds);

                //calculate alpha value depending on FadeValue
                switch (fanim.FadeMode)
                {
                    case FadeMode.Linear:
                        tempAlpha = fanim.TimeSinceFadingStarted / (float)fanim.FadingTime;
                        break;
                    case FadeMode.Quadratic:
                        tempAlpha = (fanim.TimeSinceFadingStarted / (float)fanim.FadingTime) * (fanim.TimeSinceFadingStarted / (float)fanim.FadingTime);
                        break;
                    case FadeMode.None:
                        break;
                }

                //if we want to fade out then negate the alpha value
                obj.Alpha = (fanim.AnimationDirection == 1) ? 0 + tempAlpha : 1 - tempAlpha;

                //fading is over, set clip alpha and set fadingMode to do nothing
                if (fanim.TimeSinceFadingStarted >= fanim.FadingTime)
                {
                    obj.Alpha = (float)Math.Round(obj.Alpha);
                    fanim.TimeSinceFadingStarted = 0;

                    //negate animation direction when minimum or maximum is reached
                    if (fanim.Reverse)
                    {
                        //check if whole animation (fade in and out) is over and disable it if looping is not set
                        if ((fanim.AnimationDirection == 1 && fanim.Inverted)
                        || (fanim.AnimationDirection == -1 && !fanim.Inverted))
                        {
                            fanim.Active = fanim.Looping;
                            if (fanim.RandomFadeDuration != Vector4.Zero)
                            {
                                fanim.DurationBeforeReverse = rng.Next((int)fanim.RandomFadeDuration.Z, (int)fanim.RandomFadeDuration.W);
                                fanim.FadingTime = rng.Next((int)fanim.RandomFadeDuration.X, (int)fanim.RandomFadeDuration.Y);
                            }
                        }

                        fanim.AnimationDirection *= -1;
                        // Set paused when there is a durationBeforeReverse
                        if (fanim.DurationBeforeReverse > 0)
                            fanim.Paused = true;
                    }
                    //disable animation if you don't want to reverse it
                    else
                    {
                        if ((fanim.LoopTime == 0 && fanim.LoopTimes == 0) ||
                            (fanim.LoopTime > 0 && fanim.LoopTime <= fanim.TimeSinceTotalAnimationStarted)
                            || (fanim.LoopTimes > 0 && fanim.LoopTimes <= fanim.CountLoops))
                        {
                            fanim.Active = fanim.Looping;
                            fanim.TimeSinceTotalAnimationStarted = 0;
                            fanim.CountLoops = 0;
                            if (fanim.ResetAfterFade)
                            {
                                obj.Alpha = (fanim.Inverted) ? 1f : 0f;
                                obj.Visible = false;
                            }
                        }
                        else
                        {
                            fanim.CountLoops++;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Given the GameTime and an animation to update, this methods updates the frames of the animation.
        /// </summary>
        /// <param name="animation">The animation we want to update.</param>
        /// <param name="gameTime">GameTime</param>
        public void updateAnimation(SpriteAnimation animation, GameTime gameTime)
        {
            // Do not update if animation is not active
            if (!animation.Active) return;
            //reset animation if necessary
            if (animation.Reset)
            {
                //change current frame to first or last
                if (animation.Reverse) animation.CurrentFrame = animation.FrameCount - 1;
                else animation.CurrentFrame = 0;

                //deactivate animation
                animation.Active = false;
                animation.ElapsedTime = 0;
                return;
            }

            // Update elapsed time
            animation.ElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time we need to switch frames
            if (animation.ElapsedTime > animation.FrameTime)
            {
                // Move to the next frame
                animation.CurrentFrame += animation.AnimationDirection;

                // Reached upper ending of our animation
                if (animation.CurrentFrame == animation.FrameCount - 1)
                {
                    // If we are in reverse mode set frameStep to go backwards
                    if (animation.Reverse)
                    {
                        animation.AnimationDirection = -1;
                        if (animation.WaitOnReverse) animation.Active = false;
                    }
                    // If we in normal mode reset currentFrame to zero
                    else
                    {
                        animation.CurrentFrame = 0;
                        // If we are not looping deactivate the animation
                        if (animation.Looping == false)
                            animation.Active = false;
                    }
                }
                else
                {
                    // Reached lower ending of our animation
                    if (animation.CurrentFrame == 0)
                    {
                        // Set frameStep back to normal
                        animation.AnimationDirection = 1;
                        // If we are not looping deactivate the animation
                        if (animation.Looping == false)
                            animation.Active = false;
                    }
                }

                // Reset the elapsed time to zero
                animation.ElapsedTime = 0;
            }
        }

        /// <summary>
        /// Updates the route of the object and his position.
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        /// <param name="obj">Objects we want to update.</param>
        public void updateWaypointsAndPosition(GameTime gameTime, PathAnimation path)
        {
            path.updatePosition(gameTime);
        }


        #endregion

    }
}
