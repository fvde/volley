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
    public class ScoreIndicator
    {
        
        // Variables

        private Texture2D graphics;
        public Texture2D Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        private SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        private Vector4 score;
        public Vector4 Score
        {
            get { return score; }
            set { score = value; }
        }

        // Constructors

        public ScoreIndicator()
        {
            size = new Vector2(200, 100);
        }


        // Methods

        public void SetScore(Vector2 newScore)
        {
            int temp1 = (int)score.X;
            int temp2 = (int)newScore.X;
            if (temp2 > 9)
            {
                temp1 = temp2 / 10;
                temp2 = temp2 % 10;
            }

            int temp3 = (int)score.Z;
            int temp4 = (int)newScore.Y;
            if (temp4 > 9)
            {
                temp3 = temp4 / 10;
                temp4 = temp4 % 10;
            }

            score = new Vector4(temp1, temp2, temp3, temp4);
        }


    }
}
