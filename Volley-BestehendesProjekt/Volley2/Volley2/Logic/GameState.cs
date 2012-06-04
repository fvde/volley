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
using Volley2.InOutput;
using Volley2.Objects;
using Volley2.Menu;

namespace Volley2.Logic
{
    public class GameState
    {
        // Variables

        private Game1 game1;

        private Random random;
        public Random Random
        {
            get { return random; }
            set { random = value; }
        }

        private Rumble rumble;
        public Rumble Rumble
        {
            get { return rumble; }
            set { rumble = value; }
        }

        // contains Resolution and state
        private State state;
        public State State
        {
            get { return state; }
            set { state = value;}
        }

        // Objects
        private GameObjects gameObjects;
        public GameObjects GameObjects
        {
            get { return gameObjects; }
            set { gameObjects = value; }
        }

        private MenuObjects menuObjects;
        public MenuObjects MenuObjects
        {
            get { return menuObjects; }
            set { menuObjects = value; }
        }
        


        // Constructors

        public GameState(Game1 game1, Vector2 screenResolution, Random random)
        {
            this.game1 = game1;
            state = new State(new Vector3(1,1,1), screenResolution);
            this.random = random;
            rumble = new InOutput.Rumble();

            gameObjects = new GameObjects();
            menuObjects = new MenuObjects();

        }


        // Methods

        /// <summary>
        /// Loads all content
        /// </summary>
        public void LoadContent()
        {

            GraphicsEngine.LoadContent(game1, this);

        }

        /// <summary>
        /// Updates GameState
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            rumble.Update(gameTime);

            InputEngine.Update(gameTime, this);
            PhysicsEngine.Update(gameTime, this);

            if (state.Index.X == 0)
                game1.Exit();

        }


        /// <summary>
        /// Defines what has to be drawn in every State
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            GraphicsEngine.Draw(gameTime, spriteBatch, this);

        }




    }
}
