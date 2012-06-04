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
    public class MenuObjects
    {
        // Variables

        private MainMenu mainMenu;
        public MainMenu MainMenu
        {
            get { return mainMenu; }
            set { mainMenu = value; }
        }

        private OptionsMenu optionsMenu;
        public OptionsMenu OptionsMenu
        {
            get { return optionsMenu; }
            set { optionsMenu = value; }
        }

        private GameMenu gameMenu;
        public GameMenu GameMenu
        {
            get { return gameMenu; }
            set { gameMenu = value; }
        }


        // Constructors
        public MenuObjects()
        {
            mainMenu = new MainMenu();
            optionsMenu = new OptionsMenu();
            gameMenu = new GameMenu();
        }

    }
}
