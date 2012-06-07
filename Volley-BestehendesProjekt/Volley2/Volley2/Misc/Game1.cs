using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Volley2.Logic;

namespace Volley2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Constants

        // for debugging
        private Vector3 jumpToState = new Vector3(1, 1, 1);
        private bool activateSafeAreaIndicator = false;

        // Variables

        static Vector2 defaultScreenResolution = new Vector2(1280, 720);
        GameState gameState;

        // Texture for drawing safeArea
        Texture2D pixelTexture;

        GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set window resolution and title
            graphics.PreferredBackBufferWidth = (int)defaultScreenResolution.X;
            graphics.PreferredBackBufferHeight = (int)defaultScreenResolution.Y;
            Window.Title = "Volley Prototype";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("|Game1| Initialize()");

            Random rnd = new Random();
            gameState = new GameState(this, defaultScreenResolution, rnd);

            // jump to a certain state
            if (jumpToState != new Vector3(0, 0, 0))
                gameState.State.Index = jumpToState;

            System.Diagnostics.Debug.WriteLine("|Game1| Initialize() done ");
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            System.Diagnostics.Debug.WriteLine("|Game1| LoadContent()");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixelTexture = Content.Load<Texture2D>(@"Graphics\UI\SafeArea\pixelTexture");

            gameState.LoadContent();
            System.Diagnostics.Debug.WriteLine("|Game1| LoadContent() done");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gameState.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            gameState.Draw(gameTime, spriteBatch);

            // Indicate safeArea
            if (activateSafeAreaIndicator)
            {
                Vector2 upperLeft = new Vector2(gameState.State.SafeArea.X, gameState.State.SafeArea.Y);
                Vector2 upperRight = new Vector2(gameState.State.SafeArea.X + gameState.State.SafeArea.Width, gameState.State.SafeArea.Y);
                Vector2 bottomLeft = new Vector2(gameState.State.SafeArea.X, gameState.State.SafeArea.Y + gameState.State.SafeArea.Height);
                Vector2 bottomRight = new Vector2(gameState.State.SafeArea.X + gameState.State.SafeArea.Width,
                    gameState.State.SafeArea.Y + gameState.State.SafeArea.Height);
                DrawLine(upperLeft, upperRight, Color.White);
                DrawLine(upperLeft, bottomLeft, Color.White);
                DrawLine(bottomRight, bottomLeft, Color.White);
                DrawLine(bottomRight, upperRight, Color.White);
            }



            spriteBatch.End();

            base.Draw(gameTime);
        }

        // Helper Classes

        public void DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            float distance = Vector2.Distance(p1, p2);

            float angle = (float)Math.Atan2((double)(p2.Y - p1.Y), (double)(p2.X - p1.X));

            spriteBatch.Draw(pixelTexture, p1, null, color, angle, Vector2.Zero, new Vector2(distance, 1), SpriteEffects.None, 1);
        } 

    }
}
