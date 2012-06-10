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
    public class Gamepad
    {

        // Variables

        private PlayerIndex index;
        public PlayerIndex Index
        {
            get { return index; }
            set { this.index = value; }
        }

        // a counter for every gamepad that limits inputs, especially in menus
        private int waitingTimeForNextInput = 0;
        public int WaitingTimeForNextInput
        {
            get { return waitingTimeForNextInput; }
            set { waitingTimeForNextInput = value; }
        }

        // assign buttons/sticks for all actions

        public enum ThumbStickSide { Left, Right }

        private ThumbStickSide movePlayerStick;
        public ThumbStickSide MovePlayerStick
        {
            get { return movePlayerStick; }
            set { movePlayerStick = value; }
        }

        private ThumbStickSide moveHandStick;
        public ThumbStickSide MoveHandStick
        {
            get { return moveHandStick; }
            set { moveHandStick = value; }
        }

        private Buttons jumpButton;
        public Buttons JumpButton
        {
            get { return jumpButton; }
            set { jumpButton = value; }
        }


        // Constructors

        public Gamepad(PlayerIndex index)
        {
            this.index = index;

            movePlayerStick = ThumbStickSide.Left;
            moveHandStick = ThumbStickSide.Right;
            jumpButton = Buttons.RightShoulder;

        }


    }
}
