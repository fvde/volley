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

namespace Volley2.Menu
{
    public class OptionsMenu
    {

        // Variables

        private Texture2D backgroundGraphics;
        public Texture2D BackgroundGraphics
        {
            get { return backgroundGraphics; }
            set { backgroundGraphics = value; }
        }

        private Texture2D backgroundGraphics2;
        public Texture2D BackgroundGraphics2
        {
            get { return backgroundGraphics2; }
            set { backgroundGraphics2 = value; }
        }

        private Texture2D gamePadGraphics;
        public Texture2D GamePadGraphics
        {
            get { return gamePadGraphics; }
            set { gamePadGraphics = value; }
        }

        // Stores all instructions which are displayed in all option menues
        private ButtonInstruction[] instructions;
        public ButtonInstruction[] Instructions
        {
            get { return instructions; }
            set { instructions = value; }
        }


        // Constructors

        public OptionsMenu()
        {

            // Create instructions
            instructions = new ButtonInstruction[2];

            instructions[0] = new ButtonInstruction(ButtonInstruction.Button.B, "Save and Exit");
            instructions[1] = new ButtonInstruction(ButtonInstruction.Button.A, "Change");

        }




    }
}
