using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Torrero.Model;

namespace Torrero.Controller
{
    public class Graphics
    {
        // All getters, setters.
        #region Properties

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        /// <summary>
        /// Spritebatch for drawing.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The current GraphicsDevice.
        /// </summary>
        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// Spritefont for drawing Strings.
        /// </summary>
        private SpriteFont font;

        private float elapsedGametime;

        private int frameCounter;

        float framesPerSecond;

        #endregion

        // Object constructors.
        #region Initialization

        /// <summary>
        /// Create a new graphics controller.
        /// </summary>
        public Graphics(ContentManager contentManager)
        {
            graphicsDevice = SharedGraphicsDeviceManager.Current.GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);

            font = contentManager.Load<SpriteFont>("SpriteFont1");
        }

        #endregion

        #region Draw
        /// <summary>
        /// Draws all objects.
        /// </summary>
        /// <param name="tiles">The game area in tiles.</param>
        public void drawGame(GameInstance gameInstance, TimeSpan gameTime, UIElementRenderer elementRenderer)
        {
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.Red);
            spriteBatch.Begin();
            
            //draw tiles
            foreach (Tile tile in gameInstance.Grid.Tiles)
            {
                spriteBatch.Draw(tile.Texture, translateObjectPositionToLowerLeft(tile.BottomLeftPosition), Color.White);
            }

            //draw gameObjects
            foreach (GameObject gameObj in gameInstance.GameObjects)
            {
                spriteBatch.Draw(gameObj.Texture,
                    translateObjectPositionToLowerRight(gameObj.BottomLeftPosition, gameObj.Size), Color.White);
            }

            //draw highscore
            spriteBatch.DrawString(font, "Score:" + Convert.ToString(gameInstance.Score), new Vector2(30f, 25f), Color.Gold);
            int x = 0, y = 0;
            gameInstance.Grid.getTileCoordsAtPosition(ref x, ref y, gameInstance.Player.CenterPosition);
            spriteBatch.DrawString(font, "Pos: X " + x + "Y " + y,
                new Vector2(30f, 5f), Color.Gold);

            //FPS
            drawFPS(gameTime);


            // Using the texture from the UIElementRenderer, 
            // draw the Silverlight controls to the screen.
            spriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Color.White);

            spriteBatch.End();
        }

        /// <summary>
        /// Draw the FPS to the screen.
        /// </summary>
        /// <param name="gameTime">Current GameTime.</param>
        private void drawFPS(TimeSpan gameTime)
        {
            elapsedGametime += (float)gameTime.TotalSeconds;
            if (elapsedGametime > 1.0f)
            {
                framesPerSecond = frameCounter / elapsedGametime;
                elapsedGametime -= 1f;
                frameCounter = 0;
            }
            else
            {
                frameCounter++;
            }
            spriteBatch.DrawString(font, Convert.ToString(framesPerSecond), new Vector2(10, 700), Color.IndianRed);
        }

        #endregion

        #region HelpherMethods

        /// <summary>
        /// Converts from upper left corner to bottom left corner.
        /// </summary>
        /// <param name="position">Position of the object.</param>
        /// <param name="width">Width of the object.</param>
        /// <param name="height">Heigth of the object.</param>
        /// <returns>Rectangle with translated position information.</returns>
        private Rectangle translateObjectPositionToLowerLeft(Vector2 position, int width, int height)
        {
            return new Rectangle((int)position.X, (int)Math.Floor(((float)graphicsDevice.Viewport.Height) - height - position.Y), width, height);
        }

        /// <summary>
        /// Converts from upper left corner to bottom left corner.
        /// </summary>
        /// <param name="position">Position of the object.</param>
        /// <param name="width">Width of the object.</param>
        /// <param name="height">Heigth of the object.</param>
        /// <returns>Rectangle with translated position information.</returns>
        private Rectangle translateObjectPositionToLowerLeft(Vector2 position)
        {
            return new Rectangle((int)position.X, (int)Math.Round(TorreroConstants.ViewportHeightWithTile - position.Y), TorreroConstants.TileSize, TorreroConstants.TileSize);
        }

        /// <summary>
        /// Converts from upper left corner to bottom left corner.
        /// </summary>
        /// <param name="position">Position of the object.</param>
        /// <param name="width">Width of the object.</param>
        /// <param name="height">Heigth of the object.</param>
        /// <returns>Rectangle with translated position information.</returns>
        private Rectangle translateObjectPositionToLowerRight(Vector2 position, Vector2 size)
        {
            return translateObjectPositionToLowerLeft(position, (int)size.X, (int)size.Y);
        }

        #endregion

    }
}
