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
using Volley2.Logic;

namespace Volley2.Objects
{
    public class GameObjects
    {

        // Variables
        private Ball ball;
        public Ball Ball
        {
            get { return ball; }
            set { ball = value; }
        }

        private Net net;
        public Net Net
        {
            get { return net; }
            set { net = value; }
        }

        private PlayerArray playerArray;
        public PlayerArray PlayerArray
        {
            get { return playerArray; }
            set { playerArray = value; }
        }

        private Texture2D backgroundGraphics;
        public Texture2D BackgroundGraphics
        {
            get { return backgroundGraphics; }
            set { backgroundGraphics = value; }
        }


        // Constructors

        public GameObjects()
        {
            ball = new Ball();
            net = new Net();
            playerArray = new PlayerArray();
        }


    }
}
