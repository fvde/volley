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

namespace Volley2.Logic
{
    public class State
    {
        // Variables

        // index specifies the state in which gameState is
        // [1, , ] Main Menu
        // [2, , ] Story
        // [3, , ] QuickPlay
        // [4, , ] Options
        private Vector3 index = new Vector3();
        public Vector3 Index
        {
            get { return index; }
            set
            {
                index = value;
            }
        }

        private Vector2 screenResolution = new Vector2();
        public Vector2 ScreenResolution
        {
            get { return screenResolution; }
        }

        // Defines safe area which can be different on every TV
        private Rectangle safeArea;
        public Rectangle SafeArea
        {
            get { return safeArea; }
            set { safeArea = value; }
        }

        // Represents game's score
        private Vector2 score;
        public Vector2 Score
        {
            get { return score; }
            set { score = value; }
        }

        // Represent's game's time
        private TimeSpan time;
        public TimeSpan Time
        {
            get { return time; }
            set { time = value; }
        }

        // Costructors

        public State(Vector3 index, Vector2 screenResolution)
        {
            this.Index = index;

            this.screenResolution = screenResolution;
            this.safeArea = new Rectangle((int)(screenResolution.X * 0.1), (int)(screenResolution.Y * 0.1), (int)(screenResolution.X * 0.8f), (int)(screenResolution.Y*0.8f));

            score = new Vector2(0, 0);
            
        }









    }
}
