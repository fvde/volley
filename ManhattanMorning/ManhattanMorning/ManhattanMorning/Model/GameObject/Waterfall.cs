using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;
using ManhattanMorning.Controller;

namespace ManhattanMorning.Model.GameObject
{
    public class Waterfall : LayerInterface
    {
        /// <summary>
        /// Warning Extremely ugly and not well implemented!!!
        /// </summary>

        private int speed;
        private bool paused = false;
        private bool active = false;

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
        public int tipHeight = 40;
        private int layer;
        private bool isStopped = false;
        private String name = "Waterfall";
        public int Laufzeit = 2100000;


        public bool IsStopped
        {
            get { return this.isStopped; }
            set { this.isStopped = value; }
        }

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
        public bool Active
        {
            set { this.active = value; }
            get { return this.active; }
        }



        // Methods
        public void update()
        {
            if (isStopped)
            {
                this.stopCounter = this.counter - this.Laufzeit;

                Console.WriteLine(this.stopCounter);
                if (this.stopCounter >= this.Size.Y)
                {

                    this.active = false;
                    this.counter = 0;
                    this.stopCounter = 0;
                }

            }
            if (this.paused == false && this.active == true)
            {
                this.counter += this.speed;
               
            }
    
        }

        public void start()
        {
            this.paused = false;
            this.Laufzeit = int.MaxValue;
            this.active = true;
        }

        public void stop()
        {
            if (this.IsStopped == false)
            {
                this.isStopped = true;
                this.Laufzeit = this.counter;
            }
        }

        public void pause()
        {
            this.paused = true;
        }

        public Waterfall(String name)
        {
            this.name = name;
            this.waterfallTex = StorageManager.Instance.getTextureByName("Waterfall-Tex-Main");
            this.waterfallBottomTex = StorageManager.Instance.getTextureByName("Waterfall-Tex-Bottom");
            this.waterfallHeadTex = StorageManager.Instance.getTextureByName("Waterfall-Tex-Head");
            this.waterfallTipTex = StorageManager.Instance.getTextureByName("Waterfall-Tex-Tip");
            this.waterfallStencilTex = StorageManager.Instance.getTextureByName("Waterfall-Tex-Stencil");
            this.Layer = 1;
            this.Speed = 7;
        }

    }
}