using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ManhattanMorning.Model.GameObject
{
    class Waterfall : LayerInterface
    {


        private int speed;
        private int textureHeight;
        private int textureWidth;
        private int counter;
        private int stopCounter;
        public Vector2 Position;
        public Vector2 Size;
        public Texture2D waterfallTex;
        public Texture2D waterfallBottomTex;
        public Texture2D waterfallTipTex;
        public Texture2D waterfallHeadTex;
        public Texture2D waterfallStencilTex;
        private int layer;
        private String name = "Waterfall";
        public int Laufzeit = 2100;
        public int Layer
        {
            get{return this.layer;}
            set{this.layer = value;}
        }
        public String Name
        {
            get{return this.name;}
        }

       
        public int Counter
        {
            set { this.counter = value; }
            get { return this.counter; }
        }
        public int StopCounter
        {
            set { this.stopCounter = value; }
            get { return this.stopCounter; }
        }

        public int tWidth
        {
            set { this.textureWidth = value; }
            get { return this.textureWidth; }
        }
        public int tHeight
        {
            set { this.textureHeight = value; }
            get { return this.textureHeight; }
        }
        public int Speed
        {
            set { this.speed = value; }
            get { return this.speed; }
        }

        public Waterfall()
        {
            this.waterfallTex = Game1.Instance.Content.Load<Texture2D>(@"Textures\Levels\Default\Player_blue");
           
         
        }

    }
}