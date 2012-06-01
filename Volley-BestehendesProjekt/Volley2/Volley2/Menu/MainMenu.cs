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
    public class MainMenu
    {

        // Variables

        private Texture2D backgroundGraphics;
        public Texture2D BackgroundGraphics
        {
            get { return backgroundGraphics; }
            set { backgroundGraphics = value; }
        }

        private SpriteFont menuFont1;
        public SpriteFont MenuFont1
        {
            get { return menuFont1; }
            set { menuFont1 = value; }
        }

        private Button[] buttonsState1_1;
        public Button[] ButtonState1_1
        {
            get { return buttonsState1_1; }
            set { buttonsState1_1 = value; }
        }

        private Button[] buttonsState1_2;
        public Button[] ButtonState1_2
        {
            get { return buttonsState1_2; }
            set { buttonsState1_2 = value; }
        }

        private Button[] buttonsState1_3;
        public Button[] ButtonState1_3
        {
            get { return buttonsState1_3; }
            set { buttonsState1_3 = value; }
        }

        private Button[] buttonsState1_4;
        public Button[] ButtonState1_4
        {
            get { return buttonsState1_4; }
            set { buttonsState1_4 = value; }
        }

        private Button[] buttonsState1_5;
        public Button[] ButtonState1_5
        {
            get { return buttonsState1_5; }
            set { buttonsState1_5 = value; }
        }

        private Headline[] headlines;
        public Headline[] Headlines
        {
            get { return headlines; }
            set { headlines = value; }
        }

        // Constructors

        public MainMenu()
        {
            // Create 4 buttons for main menu top level
            buttonsState1_1 = new Button[4];
            for (int i = 0; i < buttonsState1_1.Length; i++)
                buttonsState1_1[i] = new Button();

            // Create 2 buttons for main menu story menu
            buttonsState1_2 = new Button[2];
            for (int i = 0; i < buttonsState1_2.Length; i++)
                buttonsState1_2[i] = new Button();

            // Create 2 buttons for main menu quickplay menu
            buttonsState1_3 = new Button[2];
            for (int i = 0; i < buttonsState1_3.Length; i++)
                buttonsState1_3[i] = new Button();

            // Create 3 buttons for main menu options menu
            buttonsState1_4 = new Button[3];
            for (int i = 0; i < buttonsState1_4.Length; i++)
                buttonsState1_4[i] = new Button();

            // Create 2 buttons for main menu quit menu
            buttonsState1_5 = new Button[2];
            for (int i = 0; i < buttonsState1_5.Length; i++)
                buttonsState1_5[i] = new Button();

            // Create 5 headlines for different menu levels
            headlines = new Headline[5];
            for (int i = 0; i < headlines.Length; i++)
                headlines[i] = new Headline();

        }
    }
}
