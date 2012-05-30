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
    public class UI
    {

        // Variables
        private ScoreIndicator scoreIndicator;
        public ScoreIndicator ScoreIndicator
        {
            get { return scoreIndicator; }
            set { scoreIndicator = value; }
        }

        private TimeIndicator timeIndicator;
        public TimeIndicator TimeIndicator
        {
            get { return timeIndicator; }
            set { timeIndicator = value; }
        }


        // Constructors

        public UI()
        {
            scoreIndicator = new ScoreIndicator();
            TimeIndicator = new TimeIndicator();
        }


    }
}
