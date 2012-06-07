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
using Volley2.Objects;
using Volley2.Menu;

namespace Volley2.InOutput
{
    public static class GraphicsEngine
    {
        // constants

        // percental size of safe area for size of main menu headline
        private static Vector2 mainMenu_HeadlineSize = new Vector2(0.35f, 0.35f);
        // percental y-starting-point for headlines
        private static float mainMenu_HeadlineStartingPoint = 0.002f;

        // percental size of safe area for size of main menu buttons
        private static Vector2 mainMenu_ButtonSize = new Vector2(0.29f, 0.14f);
        // percental y-starting-point for buttons
        private static float mainMenu_ButtonStartingPoint = 0.26f;
        // percental y-distance between main menu buttons
        private static float mainMenu_ButtonDistance = 0.17f;




        // Methods


        // Loading


        public static void LoadContent(Game1 game1, GameState gameState)
        {

            if (gameState.State.Index.X == 1)
                loadMainMenu(game1, gameState);
            else if (gameState.State.Index.X == 2)
                loadStoryMode(game1, gameState);
            else if (gameState.State.Index.X == 3)
                loadQuickplayMode(game1, gameState, gameState.GameObjects.PlayerArray.NumberOfGamepadPlayers());
            else if (gameState.State.Index.X == 4)
                loadOptionsMenu(game1, gameState);

        }


        private static void loadMainMenu(Game1 game1, GameState gameState)
        {
            
            gameState.MenuObjects = new Menu.MenuObjects();

            gameState.MenuObjects.MainMenu.BackgroundGraphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Background2");

            // Load graphics
            gameState.MenuObjects.MainMenu.ButtonState1_1[0].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_Story");
            gameState.MenuObjects.MainMenu.ButtonState1_1[1].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_QuickPlay");
            gameState.MenuObjects.MainMenu.ButtonState1_1[2].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_Options");
            gameState.MenuObjects.MainMenu.ButtonState1_1[3].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_Quit");

            gameState.MenuObjects.MainMenu.ButtonState1_2[0].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_NewStory");
            gameState.MenuObjects.MainMenu.ButtonState1_2[1].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_ContinueStory");

            gameState.MenuObjects.MainMenu.ButtonState1_3[0].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_1vs1");
            gameState.MenuObjects.MainMenu.ButtonState1_3[1].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_2vs2");

            gameState.MenuObjects.MainMenu.ButtonState1_4[0].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_Gamepad");
            gameState.MenuObjects.MainMenu.ButtonState1_4[1].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_Graphics");
            gameState.MenuObjects.MainMenu.ButtonState1_4[2].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_Sound");

            gameState.MenuObjects.MainMenu.ButtonState1_5[0].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_Yes");
            gameState.MenuObjects.MainMenu.ButtonState1_5[1].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Button_No");

            gameState.MenuObjects.MainMenu.Headlines[0].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Headline_Volley");
            gameState.MenuObjects.MainMenu.Headlines[1].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Headline_Story");
            gameState.MenuObjects.MainMenu.Headlines[2].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Headline_QuickPlay");
            gameState.MenuObjects.MainMenu.Headlines[3].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Headline_Options");
            gameState.MenuObjects.MainMenu.Headlines[4].Graphics = game1.Content.Load<Texture2D>(@"Graphics\MainMenu\mainMenu_Headline_Quit");

            // Set size and position
            for (int i = 0; i < gameState.MenuObjects.MainMenu.ButtonState1_1.Length; i++)
            {
                gameState.MenuObjects.MainMenu.ButtonState1_1[i].Size = new Vector2((mainMenu_ButtonSize.X * gameState.State.SafeArea.Width),
                    (mainMenu_ButtonSize.Y * gameState.State.SafeArea.Height));
                gameState.MenuObjects.MainMenu.ButtonState1_1[i].Position = new Vector2(
                    ((gameState.State.SafeArea.X + gameState.State.SafeArea.Width / 2) - (gameState.MenuObjects.MainMenu.ButtonState1_1[i].Size.X / 2)),
                    (gameState.State.SafeArea.Y + (mainMenu_ButtonStartingPoint * gameState.State.SafeArea.Height) + 
                    (i * (mainMenu_ButtonDistance * gameState.State.SafeArea.Height))));
            }

            for (int i = 0; i < gameState.MenuObjects.MainMenu.ButtonState1_2.Length; i++)
            {
                gameState.MenuObjects.MainMenu.ButtonState1_2[i].Size = new Vector2((mainMenu_ButtonSize.X * gameState.State.SafeArea.Width),
                    (mainMenu_ButtonSize.Y * gameState.State.SafeArea.Height));
                gameState.MenuObjects.MainMenu.ButtonState1_2[i].Position = new Vector2(
                    ((gameState.State.SafeArea.X + gameState.State.SafeArea.Width / 2) - (gameState.MenuObjects.MainMenu.ButtonState1_2[i].Size.X / 2)),
                    (gameState.State.SafeArea.Y + (mainMenu_ButtonStartingPoint * gameState.State.SafeArea.Height) +
                    (i * (mainMenu_ButtonDistance * gameState.State.SafeArea.Height))));
            }

            for (int i = 0; i < gameState.MenuObjects.MainMenu.ButtonState1_3.Length; i++)
            {
                gameState.MenuObjects.MainMenu.ButtonState1_3[i].Size = new Vector2((mainMenu_ButtonSize.X * gameState.State.SafeArea.Width),
                    (mainMenu_ButtonSize.Y * gameState.State.SafeArea.Height));
                gameState.MenuObjects.MainMenu.ButtonState1_3[i].Position = new Vector2(
                    ((gameState.State.SafeArea.X + gameState.State.SafeArea.Width / 2) - (gameState.MenuObjects.MainMenu.ButtonState1_3[i].Size.X / 2)),
                    (gameState.State.SafeArea.Y + (mainMenu_ButtonStartingPoint * gameState.State.SafeArea.Height) +
                    (i * (mainMenu_ButtonDistance * gameState.State.SafeArea.Height))));
            }

            for (int i = 0; i < gameState.MenuObjects.MainMenu.ButtonState1_4.Length; i++)
            {
                gameState.MenuObjects.MainMenu.ButtonState1_4[i].Size = new Vector2((mainMenu_ButtonSize.X * gameState.State.SafeArea.Width),
                    (mainMenu_ButtonSize.Y * gameState.State.SafeArea.Height));
                gameState.MenuObjects.MainMenu.ButtonState1_4[i].Position = new Vector2(
                    ((gameState.State.SafeArea.X + gameState.State.SafeArea.Width / 2) - (gameState.MenuObjects.MainMenu.ButtonState1_4[i].Size.X / 2)),
                    (gameState.State.SafeArea.Y + (mainMenu_ButtonStartingPoint * gameState.State.SafeArea.Height) +
                    (i * (mainMenu_ButtonDistance * gameState.State.SafeArea.Height))));
            }

            for (int i = 0; i < gameState.MenuObjects.MainMenu.ButtonState1_5.Length; i++)
            {
                gameState.MenuObjects.MainMenu.ButtonState1_5[i].Size = new Vector2((mainMenu_ButtonSize.X * gameState.State.SafeArea.Width),
                    (mainMenu_ButtonSize.Y * gameState.State.SafeArea.Height));
                gameState.MenuObjects.MainMenu.ButtonState1_5[i].Position = new Vector2(
                    ((gameState.State.SafeArea.X + gameState.State.SafeArea.Width / 2) - (gameState.MenuObjects.MainMenu.ButtonState1_5[i].Size.X / 2)),
                    (gameState.State.SafeArea.Y + (mainMenu_ButtonStartingPoint * gameState.State.SafeArea.Height) +
                    (i * (mainMenu_ButtonDistance * gameState.State.SafeArea.Height))));
            }

            for (int i = 0; i < gameState.MenuObjects.MainMenu.Headlines.Length; i++)
            {
                gameState.MenuObjects.MainMenu.Headlines[i].Size = new Vector2((mainMenu_HeadlineSize.X * gameState.State.SafeArea.Width),
                    (mainMenu_HeadlineSize.Y * gameState.State.SafeArea.Height));
                gameState.MenuObjects.MainMenu.Headlines[i].Position = new Vector2(
                    ((gameState.State.SafeArea.X + gameState.State.SafeArea.Width / 2) - (gameState.MenuObjects.MainMenu.Headlines[i].Size.X / 2)),
                    (gameState.State.SafeArea.Y + (mainMenu_HeadlineStartingPoint * gameState.State.SafeArea.Height)));
            }
            
            System.Diagnostics.Debug.WriteLine("|GraphicsEngine| MainMenu loaded successfully");
        }

        private static void loadStoryMode(Game1 game1, GameState gameState)
        {

            System.Diagnostics.Debug.WriteLine("|GraphicsEngine| StoryMode loaded successfully");

        }

        /// <summary>
        /// Load all the necessary graphics for quickplay mode
        /// </summary>
        /// <param name="game1"></param>
        /// <returns></returns>
        private static void loadQuickplayMode(Game1 game1,GameState gameState, int playerCount)
        {

            gameState.GameObjects = new GameObjects();

            LogicEngine.resetPlayerPositions(gameState);

            loadGameMenu(game1, gameState);
            
            gameState.GameObjects.BackgroundGraphics = game1.Content.Load<Texture2D>(@"Graphics\Quickplay\quickplay_background");

            
            gameState.GameObjects.Ball.Graphics = game1.Content.Load<Texture2D>(@"Graphics\Game\ball");
            gameState.GameObjects.Ball.Size = new Vector2((gameState.State.SafeArea.Width / 10), (gameState.State.SafeArea.Width / 10));
            gameState.GameObjects.Ball.Position = new Vector2((gameState.State.SafeArea.X + (0.5f * gameState.State.SafeArea.Width) -
                                (0.5f * gameState.GameObjects.Ball.Size.X)), gameState.State.SafeArea.Y);

            gameState.GameObjects.Net.Graphics = game1.Content.Load<Texture2D>(@"Graphics\Game\net");
            gameState.GameObjects.Net.Size = new Vector2((gameState.State.SafeArea.Width / 40f), (gameState.State.SafeArea.Height / 1.8f));
            gameState.GameObjects.Net.Position = new Vector2((float)((gameState.State.SafeArea.X + gameState.State.SafeArea.Width / 2) - (0.5 * gameState.GameObjects.Net.Size.X)),
                (float)(gameState.State.SafeArea.Y + (gameState.State.SafeArea.Height - gameState.GameObjects.Net.Size.Y)));

            Player[] player = gameState.GameObjects.PlayerArray.AllPlayer;
            LogicEngine.resetPlayerPositions(gameState);
            for (int i = 0; i < player.Length; i++)
            {
                // load player

                player[i].Graphics = game1.Content.Load<Texture2D>(@"Graphics\Game\player");
                player[i].Size = new Vector2(gameState.State.SafeArea.Width / 10, gameState.State.SafeArea.Height / 3);


                // load hand
                player[i].Hand.Graphics = game1.Content.Load<Texture2D>(@"Graphics\Game\hand");
                player[i].Hand.Position = new Vector2(0, 0);
                player[i].Hand.Size = new Vector2(gameState.State.SafeArea.Width / 20, gameState.State.SafeArea.Width / 20);
            }

            // For debugging, please delete
            gameState.MenuObjects.MainMenu.MenuFont1 = game1.Content.Load<SpriteFont>(@"Fonts\Console");

            System.Diagnostics.Debug.WriteLine("|GraphicsEngine| QuickplayMode loaded successfully, Number of Players:" + playerCount.ToString());

        }

        /// <summary>
        /// Load all the necessary graphics for options menu
        /// </summary>
        /// <param name="game1"></param>
        /// <returns></returns>
        private static void loadOptionsMenu(Game1 game1, GameState gameState)
        {

            gameState.MenuObjects.OptionsMenu = new Menu.OptionsMenu();

            // Load backgrounds
            gameState.MenuObjects.OptionsMenu.BackgroundGraphics = game1.Content.Load<Texture2D>(@"Graphics\OptionsMenu\optionsMenu_Background");
            gameState.MenuObjects.OptionsMenu.BackgroundGraphics2 = game1.Content.Load<Texture2D>(@"Graphics\OptionsMenu\optionsMenu_Background2");

            gameState.MenuObjects.OptionsMenu.GamePadGraphics = game1.Content.Load<Texture2D>(@"Graphics\OptionsMenu\Controller");

            // Load instructions and set position
            for (int i = 0; i < gameState.MenuObjects.OptionsMenu.Instructions.Length; i++)
            {
                loadButtonInstruction(game1, gameState, gameState.MenuObjects.OptionsMenu.Instructions[i]);
            }

            gameState.MenuObjects.OptionsMenu.Instructions[0].Position = new Vector2((gameState.State.SafeArea.X + gameState.State.SafeArea.Width * 0.62f),
                (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height * 0.87f));
            gameState.MenuObjects.OptionsMenu.Instructions[1].Position = new Vector2((gameState.State.SafeArea.X + gameState.State.SafeArea.Width * 0.84f),
                (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height * 0.87f));



            System.Diagnostics.Debug.WriteLine("|GraphicsEngine| OptionsMenu loaded successfully");

        }

        // Loading helper classes

        private static void loadGameMenu(Game1 game1, GameState gameState)
        {
            gameState.MenuObjects.GameMenu = new Menu.GameMenu();

            gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\scoreBoard");
            gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Size = new Vector2((gameState.State.SafeArea.Width / 5.1f), (gameState.State.SafeArea.Height / 6));
            gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Font = game1.Content.Load<SpriteFont>(@"Fonts\Score");

            gameState.MenuObjects.GameMenu.BackgroundGraphics = game1.Content.Load<Texture2D>(@"Graphics\OptionsMenu\optionsMenu_Background2");
            gameState.MenuObjects.GameMenu.ControllerGraphics[0] = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Controller_player1");
            gameState.MenuObjects.GameMenu.ControllerGraphics[1] = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Controller_player2");
            gameState.MenuObjects.GameMenu.ControllerGraphics[2] = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Controller_player3");
            gameState.MenuObjects.GameMenu.ControllerGraphics[3] = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Controller_player4");

        }

        private static void loadButtonInstruction(Game1 game1, GameState gameState, ButtonInstruction buttonInstruction)
        {
            buttonInstruction.Font = game1.Content.Load<SpriteFont>(@"Fonts\ButtonInstruction");

            // Depending on which button should be displayed, load the right one
            switch (buttonInstruction.ButtonType)
            {
                case ButtonInstruction.Button.A:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Button_A");
                    break;
                case ButtonInstruction.Button.B:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Button_B");
                    break;
                case ButtonInstruction.Button.X:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Button_X");
                    break;
                case ButtonInstruction.Button.Y:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Button_Y");
                    break;
                case ButtonInstruction.Button.LB:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Bumper_Left");
                    break;
                case ButtonInstruction.Button.LT:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Trigger_Left");
                    break;
                case ButtonInstruction.Button.RB:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Bumper_Right");
                    break;
                case ButtonInstruction.Button.RT:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Trigger_Right");
                    break;
                case ButtonInstruction.Button.Start:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Button_Start");
                    break;
                case ButtonInstruction.Button.Back:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Button_Back");
                    break;
                case ButtonInstruction.Button.DPad:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Dpad_All");
                    break;
                case ButtonInstruction.Button.LS:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Stick_Left");
                    break;
                case ButtonInstruction.Button.RS:
                    buttonInstruction.Graphics = game1.Content.Load<Texture2D>(@"Graphics\UI\ControllerGraphics\Xbox360_Stick_Right");
                    break;
            }

        }


        // Drawing

        /// <summary>
        /// Draws all the necessary graphics for every state
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {

            if (gameState.State.Index.X == 1)
                drawMainMenu(gameTime, spriteBatch, gameState);
            else if (gameState.State.Index.X == 2)
                drawStoryMode(gameTime, spriteBatch, gameState);
            else if (gameState.State.Index.X == 3)
                drawQuickplayMode(gameTime, spriteBatch, gameState);
            else if (gameState.State.Index.X == 4)
                drawOptionsMenu(gameTime, spriteBatch, gameState);

        }

        /// <summary>
        /// displays main menu
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static void drawMainMenu(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {

            // draw background
            spriteBatch.Draw(gameState.MenuObjects.MainMenu.BackgroundGraphics, new Rectangle(0, 0, (int)gameState.State.ScreenResolution.X, (int)gameState.State.ScreenResolution.Y), Color.White);

            
            // Draw top level
            if (gameState.State.Index.Y == 1)
            {
                // Draw headline
                spriteBatch.Draw(gameState.MenuObjects.MainMenu.Headlines[0].Graphics,
                    gameState.MenuObjects.MainMenu.Headlines[0].DestinationRectangle(), Color.White);

                // Draw buttons
                if (gameState.State.Index == new Vector3(1, 1, 1))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[0].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[0].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 1, 2))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[1].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[1].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 1, 3))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[2].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[2].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[2].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[2].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 1, 4))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[3].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[3].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_1[3].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_1[3].DestinationRectangle(), Color.White);

            }
                
            // Draw first level Story menu
            else if (gameState.State.Index.Y == 2)
            {
                // Draw headline
                spriteBatch.Draw(gameState.MenuObjects.MainMenu.Headlines[1].Graphics,
                    gameState.MenuObjects.MainMenu.Headlines[1].DestinationRectangle(), Color.White);

                // Draw buttons
                if (gameState.State.Index == new Vector3(1, 2, 1))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_2[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_2[0].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_2[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_2[0].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 2, 2))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_2[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_2[1].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_2[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_2[1].DestinationRectangle(), Color.White);

            }
            // Draw first level QuickPlay menu
            else if (gameState.State.Index.Y == 3)
            {
                // Draw headline
                spriteBatch.Draw(gameState.MenuObjects.MainMenu.Headlines[2].Graphics,
                    gameState.MenuObjects.MainMenu.Headlines[2].DestinationRectangle(), Color.White);

                // Draw buttons
                if (gameState.State.Index == new Vector3(1, 3, 1))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_3[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_3[0].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_3[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_3[0].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 3, 2))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_3[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_3[1].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_3[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_3[1].DestinationRectangle(), Color.White);
            }
            // Draw first level Options
            else if (gameState.State.Index.Y == 4)
            {
                // Draw headline
                spriteBatch.Draw(gameState.MenuObjects.MainMenu.Headlines[3].Graphics,
                    gameState.MenuObjects.MainMenu.Headlines[3].DestinationRectangle(), Color.White);

                // Draw buttons
                if (gameState.State.Index == new Vector3(1, 4, 1))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_4[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_4[0].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_4[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_4[0].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 4, 2))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_4[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_4[1].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_4[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_4[1].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 4, 3))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_4[2].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_4[2].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_4[2].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_4[2].DestinationRectangle(), Color.White);
            }
            // Draw first level Quit
            else if (gameState.State.Index.Y == 5)
            {
                // Draw headline
                spriteBatch.Draw(gameState.MenuObjects.MainMenu.Headlines[4].Graphics,
                    gameState.MenuObjects.MainMenu.Headlines[4].DestinationRectangle(), Color.White);

                // Draw buttons
                if (gameState.State.Index == new Vector3(1, 5, 1))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_5[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_5[0].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_5[0].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_5[0].DestinationRectangle(), Color.White);

                if (gameState.State.Index == new Vector3(1, 5, 2))
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_5[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_5[1].DestinationRectangle(), Color.Pink);
                else
                    spriteBatch.Draw(gameState.MenuObjects.MainMenu.ButtonState1_5[1].Graphics,
                        gameState.MenuObjects.MainMenu.ButtonState1_5[1].DestinationRectangle(), Color.White);
            }

        }


        /// <summary>
        /// displays Story Mode
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static void drawStoryMode(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            
        }


        /// <summary>
        /// displays Quickplay Mode
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static void drawQuickplayMode(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            // Draw background
            spriteBatch.Draw(gameState.GameObjects.BackgroundGraphics, new Rectangle(0, 0, (int)gameState.State.ScreenResolution.X, (int)gameState.State.ScreenResolution.Y),
                Color.White);


            // Display player

            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();
            for (int i = 0; i < player.Length; i++)
                if (player[i].AssociatedTeam != Player.Team.Neutral)
                {
                    spriteBatch.Draw(player[i].Graphics, player[i].destinationRectangle(), Color.White);
                    spriteBatch.Draw(player[i].Hand.Graphics, player[i].Hand.DestinationRectangle(), Color.White);
                }

            // Display Ball, Net
            spriteBatch.Draw(gameState.GameObjects.Ball.Graphics, gameState.GameObjects.Ball.destinationRectangle(), Color.White);
            spriteBatch.Draw(gameState.GameObjects.Net.Graphics, gameState.GameObjects.Net.destinationRectangle(), Color.White);

            drawUI(gameTime, spriteBatch, gameState);


            // Display special events like Menues, Countdown etc.
            String stateText = "";
            if (gameState.State.Index.Z == 1)
                stateText = "Game is running";
            else if (gameState.State.Index.Z == 2)
                drawTeamMenu(gameTime, spriteBatch, gameState);
            else if (gameState.State.Index.Z == 3)
                drawGameMenu(gameTime, spriteBatch, gameState);
            else if (gameState.State.Index.Z == 4)
                drawWinner(gameTime, spriteBatch, gameState);
            else if (gameState.State.Index.Z == 5)
                drawCountdown(gameTime, spriteBatch, gameState);
            else
                stateText = "!Wrong state!";

            spriteBatch.DrawString(gameState.MenuObjects.MainMenu.MenuFont1, stateText, new Vector2(gameState.State.SafeArea.X, gameState.State.SafeArea.Y), Color.BlanchedAlmond);


        }


        /// <summary>
        /// displays options menu
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static void drawOptionsMenu(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {

            // Draw backgrounds
            Rectangle destinationRectangle = new Rectangle(0, 0, (int)gameState.State.ScreenResolution.X, (int)gameState.State.ScreenResolution.Y);
            spriteBatch.Draw(gameState.MenuObjects.OptionsMenu.BackgroundGraphics, destinationRectangle, Color.White);

            destinationRectangle = new Rectangle(((int)(gameState.State.SafeArea.X + (int)gameState.State.SafeArea.Width * 0.02f)), ((int)(gameState.State.SafeArea.Y +
                (int)gameState.State.SafeArea.Height * 0.04f)), (int)(gameState.State.SafeArea.Width * 0.96f), (int)(gameState.State.SafeArea.Height * 0.92f));
            spriteBatch.Draw(gameState.MenuObjects.OptionsMenu.BackgroundGraphics2, destinationRectangle, new Color(0, 0, 0, 150));

            // Draw gamepad options
            if (gameState.State.Index.Y == 1)
            {

                destinationRectangle = new Rectangle((int)(gameState.State.SafeArea.X + gameState.State.SafeArea.Width * 0.42f), (int)(gameState.State.SafeArea.Y +
                    gameState.State.SafeArea.Height * 0.21f), (int)(gameState.State.SafeArea.Width * 0.5), (int)(gameState.State.SafeArea.Height * 0.6));
                spriteBatch.Draw(gameState.MenuObjects.OptionsMenu.GamePadGraphics, destinationRectangle, Color.White);

                drawbuttonInstruction(gameTime, spriteBatch, gameState, gameState.MenuObjects.OptionsMenu.Instructions[0]);
                drawbuttonInstruction(gameTime, spriteBatch, gameState, gameState.MenuObjects.OptionsMenu.Instructions[1]);

            }

            // Draw graphics options
            if (gameState.State.Index.Y == 2)
            {

                drawbuttonInstruction(gameTime, spriteBatch, gameState, gameState.MenuObjects.OptionsMenu.Instructions[0]);
                drawbuttonInstruction(gameTime, spriteBatch, gameState, gameState.MenuObjects.OptionsMenu.Instructions[1]);

            }

            // Draw sound options
            if (gameState.State.Index.Y == 3)
            {

                drawbuttonInstruction(gameTime, spriteBatch, gameState, gameState.MenuObjects.OptionsMenu.Instructions[0]);
                drawbuttonInstruction(gameTime, spriteBatch, gameState, gameState.MenuObjects.OptionsMenu.Instructions[1]);

            }

        }

        // drawing helper classes

        private static void drawUI(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            // Display Score
            gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.SetScore(gameState.State.Score);

            Rectangle destinationRectangle = new Rectangle((int)(gameState.State.SafeArea.X + (gameState.State.SafeArea.Width - gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Size.X) / 2),
                gameState.State.SafeArea.Y,
                (int)gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Size.X, (int)gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Size.Y);
            spriteBatch.Draw(gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Graphics, destinationRectangle, Color.White);
            spriteBatch.DrawString(gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Font,
                gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Score.X.ToString(), new Vector2((int)destinationRectangle.X + 18, (int)destinationRectangle.Y + 10), Color.YellowGreen);
            spriteBatch.DrawString(gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Font,
                gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Score.Y.ToString(), new Vector2((int)destinationRectangle.X + 51, (int)destinationRectangle.Y + 10), Color.YellowGreen);
            spriteBatch.DrawString(gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Font,
                gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Score.Z.ToString(), new Vector2((int)destinationRectangle.X + 116, (int)destinationRectangle.Y + 10), Color.YellowGreen);
            spriteBatch.DrawString(gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Font,
                gameState.MenuObjects.GameMenu.UIElements.ScoreIndicator.Score.W.ToString(), new Vector2((int)destinationRectangle.X + 149, (int)destinationRectangle.Y + 10), Color.YellowGreen);
        }

        private static void drawbuttonInstruction(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState, ButtonInstruction buttonInstruction)
        {

            Rectangle destinationRectangle = new Rectangle((int)buttonInstruction.Position.X, (int)buttonInstruction.Position.Y, 
                (int)(gameState.State.SafeArea.Width * 0.05), (int)(gameState.State.SafeArea.Width * 0.05));
            spriteBatch.Draw(buttonInstruction.Graphics, destinationRectangle, Color.White);

            Vector2 position = new Vector2((buttonInstruction.Position.X +(gameState.State.SafeArea.Width * 0.05f)),
                (buttonInstruction.Position.Y + 12));
            spriteBatch.DrawString(buttonInstruction.Font, buttonInstruction.Text, position, Color.White);

        }

        private static void drawGameMenu(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {

            spriteBatch.DrawString(gameState.MenuObjects.MainMenu.MenuFont1, "GameMenu", new Vector2(gameState.State.SafeArea.X, gameState.State.SafeArea.Y), Color.BlanchedAlmond);

        }

        private static void drawTeamMenu(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {

            // Draw background
            Rectangle destinationRectangle = new Rectangle(((int)(gameState.State.SafeArea.X + (int)gameState.State.SafeArea.Width * 0.02f)), ((int)(gameState.State.SafeArea.Y +
                (int)gameState.State.SafeArea.Height * 0.04f)), (int)(gameState.State.SafeArea.Width * 0.96f), (int)(gameState.State.SafeArea.Height * 0.92f));
            spriteBatch.Draw(gameState.MenuObjects.GameMenu.BackgroundGraphics, destinationRectangle, new Color(0, 0, 0, 200));

            // Draw title
            spriteBatch.DrawString(gameState.MenuObjects.MainMenu.MenuFont1, "TeamMenu", new Vector2(gameState.State.SafeArea.X + (gameState.State.SafeArea.Width / 2) - 35, 
                gameState.State.SafeArea.Y + 40), Color.BlanchedAlmond);
            spriteBatch.DrawString(gameState.MenuObjects.MainMenu.MenuFont1, "Team 1", new Vector2(gameState.State.SafeArea.X + 200, gameState.State.SafeArea.Y + 60), Color.BlanchedAlmond);
            spriteBatch.DrawString(gameState.MenuObjects.MainMenu.MenuFont1, "Team 2", new Vector2(gameState.State.SafeArea.X + 770, gameState.State.SafeArea.Y + 60), Color.BlanchedAlmond);

            // Draw Controller_player
            for (int i = 0; i < gameState.MenuObjects.GameMenu.ControllerGraphics.Length; i++)
            {
                Color color = new Color(255, 255, 255, 255);
                if (gameState.GameObjects.PlayerArray.AllPlayer[i].PlayerStatus == Player.Status.InActive)
                    color = new Color(20, 20, 20, 20);

                Rectangle destinationRectangle2 = new Rectangle(gameState.State.SafeArea.X + 170 + (int)gameState.GameObjects.PlayerArray.AllPlayer[i].AssociatedTeam * 285,
                    gameState.State.SafeArea.Y + 100 + i * 105, 110, 100);

                spriteBatch.Draw(gameState.MenuObjects.GameMenu.ControllerGraphics[i], destinationRectangle2, color);
            }

            


        }

        private static void drawWinner(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {

            spriteBatch.DrawString(gameState.MenuObjects.MainMenu.MenuFont1, "Winner", new Vector2(gameState.State.SafeArea.X, gameState.State.SafeArea.Y), Color.BlanchedAlmond);

        }

        private static void drawCountdown(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {

            spriteBatch.DrawString(gameState.MenuObjects.MainMenu.MenuFont1, "Countdown", new Vector2(gameState.State.SafeArea.X, gameState.State.SafeArea.Y), Color.BlanchedAlmond);

        }

    }
}
