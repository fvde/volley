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
    public class ButtonInstruction
    {

        // Variables

        public enum Button { A, B, X, Y, LT, LB, RT, RB, Start, Back, DPad, LS, RS };
        private Button buttonType;
        public Button ButtonType
        {
            get { return buttonType; }
            set { buttonType = value; }
        }

        private Texture2D graphics;
        public Texture2D Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        private String text;
        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        private SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        // Constructors

        public ButtonInstruction(Button buttonType, String instruction)
        {
            this.buttonType = buttonType;
            this.Text = instruction;

        }
    }
}
