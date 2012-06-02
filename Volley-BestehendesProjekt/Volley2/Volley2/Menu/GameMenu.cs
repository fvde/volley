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
    public class GameMenu
    {

        // Variables

        private Texture2D backgroundGraphics;
        public Texture2D BackgroundGraphics
        {
            get { return backgroundGraphics; }
            set { backgroundGraphics = value; }
        }

        // 4 Controller graphics with player title for team management
        private Texture2D[] controllerGraphics;
        public Texture2D[] ControllerGraphics
        {
            get { return controllerGraphics; }
            set { controllerGraphics = value; }
        }

        // Contains UI Elements
        private UI uiElements;
        public UI UIElements
        {
            get { return uiElements; }
            set { uiElements = value; }
        }


        // Constructors

        public GameMenu()
        {
            controllerGraphics = new Texture2D[4];
            uiElements = new UI();
        }


    }
}
