using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using ManhattanMorning.Model;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Controller;
using ManhattanMorning.Misc.Curves;

namespace ManhattanMorning.Misc
{
    public class PathAnimation
    {

        #region Members
        private DrawableObject correspondingObject;
        private Path path;
        private float animationDuration = 1000f;
        private bool active;
        private float elapsedTime;
        private bool activateRoation = false;
        private bool playOnce = false;
        
        #endregion

        #region Properties

        /// <summary>
        /// Sets the Object that has the Path so that we dont have to pass the corresponding
        /// Object all the time.
        /// </summary>
        public DrawableObject CorrespondingObject
        {
            get { return correspondingObject; }
            set
            {
                correspondingObject = value;
            }
        }

        public Path Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public bool ActivateRotation
        {
            get { return this.activateRoation; }
            set { this.activateRoation = value; }
        }

        public float ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }

        public bool PlayOnce
        {
            get { return playOnce; }
            set { playOnce = value; }
        }

        /// <summary>
        /// The duration of the animation in milliseconds
        /// </summary>
        public float AnimationDuration { get { return this.animationDuration; } set { this.animationDuration = value; } }
 
        /// <summary>
        /// Used to set the waypoints active or inactive. In inactive state the object will be drawn but does not move.
        /// </summary>
        public bool Active { get { return active; } set { active = value; } }

        #endregion

        #region Init

        public PathAnimation(Path path)
        {
            this.path = path;
            this.active = true;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Updates the position of the Object that is linked to the PathAnimation
        /// </summary>
        /// <param name="gameTime">the current game time</param>
        public void updatePosition(GameTime gameTime)
        {
            if (elapsedTime > animationDuration) return;

            elapsedTime += (float)(gameTime.ElapsedGameTime.TotalMilliseconds);
            float t =  elapsedTime / animationDuration;
            Vector2 newPosition = path.evaluate(t);
            newPosition = newPosition - correspondingObject.Size * 0.5f;
            correspondingObject.Position = newPosition;

            if (activateRoation)
            {
                Vector2 dir = path.getDirectionAtValue(t);
                Vector2 neutral = new Vector2(1, 0);
                correspondingObject.Rotation = (dir.Y > neutral.Y ? 1f : -1f) * (float)Math.Acos(Vector2.Dot(dir, neutral));
            }

            if(t >= 1 && !playOnce) elapsedTime = 0;
        }

        /// <summary>
        /// Resets the objects position to the starting point of its path.
        /// </summary>
        public void resetPosition(){
            elapsedTime = 0f;
        }

        #endregion
    }
}
