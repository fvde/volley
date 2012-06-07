using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Volley2.InOutput
{
    public class Rumble
    {
        // Variables

        private int[] remainingTime = { 0, 0, 0, 0 };


        // Constructors
        public Rumble()
        {

        }

        // Methods

        /// <summary>
        /// Triggers a new rumble action
        /// </summary>
        /// <param name="gamePadNumber"></param>
        /// <param name="milliseconds"></param>
        /// <param name="strengthLeft"></param>
        /// <param name="strengthRight"></param>
        public void set(PlayerIndex gamepad, int milliseconds, float strengthLeft, float strengthRight)
        {
            if (gamepad == PlayerIndex.One)
                remainingTime[0] = milliseconds;
            else if (gamepad == PlayerIndex.Two)
                remainingTime[1] = milliseconds;
            else if (gamepad == PlayerIndex.Three)
                remainingTime[2] = milliseconds;
            else if (gamepad == PlayerIndex.Four)
                remainingTime[3] = milliseconds;

            GamePad.SetVibration(gamepad, strengthLeft, strengthRight);
        }

        /// <summary>
        /// Checks if one of the gamepad should stop rumble
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //  Update remaining times
            for (int i = 0; i < remainingTime.Length; i++)
            {
                if (remainingTime[i] > 0)
                {
                    remainingTime[i] -= gameTime.ElapsedGameTime.Milliseconds;
                    if (remainingTime[i] < 0)
                        remainingTime[i] = 0;
                }
            }

            // Rumble
            if (remainingTime[0] <= 0)
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            if (remainingTime[1] <= 0)
                GamePad.SetVibration(PlayerIndex.Two, 0, 0);
            if (remainingTime[2] <= 0)
                GamePad.SetVibration(PlayerIndex.Three, 0, 0);
            if (remainingTime[3] <= 0)
                GamePad.SetVibration(PlayerIndex.Four, 0, 0);
        }
            
    }
}
