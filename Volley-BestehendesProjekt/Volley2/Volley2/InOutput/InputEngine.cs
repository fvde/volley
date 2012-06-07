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
    public static class InputEngine
    {

        // Constants

        // minimal period of time between two input (milliseconds)
        static int inputWaitingTime = 200;


        // Methods

        public static void Update(GameTime gameTime, GameState gameState)
        {

            checkConnectedGamepads(gameState);

            // update input waiting times for each gamepad
            for (int i = 0; i < gameState.GameObjects.PlayerArray.GamepadPlayers().Length; i++)
            {
                if (gameState.GameObjects.PlayerArray.GamepadPlayers()[i].Gamepad.WaitingTimeForNextInput > 0)
                {
                    gameState.GameObjects.PlayerArray.GamepadPlayers()[i].Gamepad.WaitingTimeForNextInput -= gameTime.ElapsedGameTime.Milliseconds;
                    if (gameState.GameObjects.PlayerArray.GamepadPlayers()[i].Gamepad.WaitingTimeForNextInput < 0)
                        gameState.GameObjects.PlayerArray.GamepadPlayers()[i].Gamepad.WaitingTimeForNextInput = 0;
                }
            }

            if (gameState.State.Index.X == 1)
                updateMainMenu(gameTime, gameState);
            else if (gameState.State.Index.X == 2)
                updateStory(gameTime, gameState);
            else if (gameState.State.Index.X == 3)
                updateQuickPlay(gameTime, gameState);
            else if (gameState.State.Index.X == 4)
                updateOptionsMenu(gameTime, gameState);

        }

        /// <summary>
        /// Checks which gamepads are connected/ disconnected now
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        private static void checkConnectedGamepads(GameState gameState)
        {
           
            for (int i = 0; i < gameState.GameObjects.PlayerArray.AllPlayer.Length; i++)
            {
                if (GamePad.GetState(gameState.GameObjects.PlayerArray.AllPlayer[i].Gamepad.Index).IsConnected)
                {
                    if (gameState.GameObjects.PlayerArray.AllPlayer[i].PlayerStatus == Player.Status.InActive)
                    {
                        // case if a gamepad hasn't already been connected but is it now
                        gameState.GameObjects.PlayerArray.AllPlayer[i].PlayerStatus = Player.Status.Gamepad;
                        System.Diagnostics.Debug.WriteLine("|PlayerArray| Gamepad[" + i + "] connected (" +
                            gameState.GameObjects.PlayerArray.NumberOfGamepadPlayers().ToString() + " Gamepads connected, now)");
                        // Enter team management menu when in quickplay mode
                        if (gameState.State.Index.X == 3)
                            gameState.State.Index = new Vector3(3, gameState.State.Index.Y, 2);
                    }
                }
                else
                {
                    if (gameState.GameObjects.PlayerArray.AllPlayer[i].PlayerStatus == Player.Status.Gamepad)
                    {
                        // case if a gamepad has been connected and is now disconnected
                        gameState.GameObjects.PlayerArray.AllPlayer[i].PlayerStatus = Player.Status.InActive;
                        System.Diagnostics.Debug.WriteLine("|PlayerArray| Gamepad[" + i + "] disconnected (" +
                            gameState.GameObjects.PlayerArray.NumberOfGamepadPlayers().ToString() + " Gamepads connected, now)");
                        // Enter team management menu when in quickplay mode
                        if (gameState.State.Index.X == 3)
                            gameState.State.Index = new Vector3(3, gameState.State.Index.Y, 2);
                    }
                }
            }

        }

        /// <summary>
        /// Process input in main menu
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gameState"></param>
        /// <returns></returns>
        private static void updateMainMenu(GameTime gameTime, GameState gameState)
        {

            Player[] player = gameState.GameObjects.PlayerArray.GamepadPlayers();

            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].Gamepad.WaitingTimeForNextInput == 0)
                {
                    // main menu's top level
                    if (gameState.State.Index.Y == 1)
                    {
                        // Exit game immediately by pressing back button
                        if (GamePad.GetState(player[i].Gamepad.Index).Buttons.Back == ButtonState.Pressed)
                            gameState.State.Index = new Vector3(0, 0, 0);

                        // Select next button by pressing "down"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Down == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y < -0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y < -0.8f))
                        {
                            int z = (int)gameState.State.Index.Z + 1;
                            if (z > 4)
                                z = 1;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select previous button by pressing "up"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Up == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y > 0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y > 0.8f))
                        {
                            int z = (int)gameState.State.Index.Z - 1;
                            if (z < 1)
                                z = 4;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Enter menu item by pressing "A" or D-Pad-Right
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.A == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Right == ButtonState.Pressed))
                        {
                            if (gameState.State.Index.Z == 1)
                            {
                                gameState.State.Index = new Vector3(1, 2, 1);
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }
                            else if (gameState.State.Index.Z == 2)
                            {
                                gameState.State.Index = new Vector3(1, 3, 1);
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }
                            else if (gameState.State.Index.Z == 3)
                            {
                                gameState.State.Index = new Vector3(1, 4, 1);
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }
                            else if (gameState.State.Index.Z == 4)
                            {
                                gameState.State.Index = new Vector3(1, 5, 2);
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }
                        }

                    }

                    // Story menu first level
                    else if (gameState.State.Index.Y == 2)
                    {
                        // Back to top level main menu by pressing B button or D-Pad-Left
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.B == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Left == ButtonState.Pressed))
                        {
                            gameState.State.Index = new Vector3(1, 1, 1);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select next button by pressing "down"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Down == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y < -0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y < -0.8f))
                        {
                            int z = (int)gameState.State.Index.Z + 1;
                            if (z > 2)
                                z = 1;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select previous button by pressing "up"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Up == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y > 0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y > 0.8f))
                        {
                            int z = (int)gameState.State.Index.Z - 1;
                            if (z < 1)
                                z = 2;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Enter menu item by pressing "A" or D-Pad-Right
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.A == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Right == ButtonState.Pressed))
                        {
                            if (gameState.State.Index.Z == 1)
                            {
                                // Continue to New Story
                            }
                            else if (gameState.State.Index.Z == 2)
                            {
                                // Continue to Continue Story
                            }

                        }
                    }

                    // Quickplay menu first level
                    else if (gameState.State.Index.Y == 3)
                    {
                        // Back to top level main menu by pressing B button or D-Pad-Left
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.B == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Left == ButtonState.Pressed))
                        {
                            gameState.State.Index = new Vector3(1, 1, 2);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select next button by pressing "down"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Down == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y < -0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y < -0.8f))
                        {
                            int z = (int)gameState.State.Index.Z + 1;
                            if (z > 2)
                                z = 1;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select previous button by pressing "up"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Up == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y > 0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y > 0.8f))
                        {
                            int z = (int)gameState.State.Index.Z - 1;
                            if (z < 1)
                                z = 2;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Enter menu item by pressing "A" or D-Pad-Right
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.A == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Right == ButtonState.Pressed))
                        {
                            if (gameState.State.Index.Z == 1)
                            {
                                // Continue to 1 vs 1
                                gameState.State.Index = new Vector3(3, 1, 1);
                                gameState.LoadContent();
                            }
                            else if (gameState.State.Index.Z == 2)
                            {
                                // Continue to C2 vs 2
                            }

                        }
                    }

                    // Options menu first level
                    else if (gameState.State.Index.Y == 4)
                    {
                        // Back to top level main menu by pressing B button or D-Pad-Left
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.B == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Left == ButtonState.Pressed))
                        {
                            gameState.State.Index = new Vector3(1, 1, 3);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select next button by pressing "down"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Down == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y < -0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y < -0.8f))
                        {
                            int z = (int)gameState.State.Index.Z + 1;
                            if (z > 3)
                                z = 1;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select previous button by pressing "up"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Up == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y > 0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y > 0.8f))
                        {
                            int z = (int)gameState.State.Index.Z - 1;
                            if (z < 1)
                                z = 3;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Enter menu item by pressing "A" or D-Pad-Right
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.A == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Right == ButtonState.Pressed))
                        {
                            if (gameState.State.Index.Z == 1)
                            {
                                // Gamepad options
                                gameState.State.Index = new Vector3(4, 1, i);
                                gameState.LoadContent();
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }
                            else if (gameState.State.Index.Z == 2)
                            {
                                // Graphics options
                                gameState.State.Index = new Vector3(4, 2, 1);
                                gameState.LoadContent();
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }
                            else if (gameState.State.Index.Z == 3)
                            {
                                // Sound Options
                                gameState.State.Index = new Vector3(4, 3, 1);
                                gameState.LoadContent();
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }

                        }
                    }

                    // Quit menu first level
                    else if (gameState.State.Index.Y == 5)
                    {
                        // Back to top level main menu by pressing B button or D-Pad-Left
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.B == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Left == ButtonState.Pressed))
                        {
                            gameState.State.Index = new Vector3(1, 1, 4);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select next button by pressing "down"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Down == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y < -0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y < -0.8f))
                        {
                            int z = (int)gameState.State.Index.Z + 1;
                            if (z > 2)
                                z = 1;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Select previous button by pressing "up"
                        if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Up == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y > 0.8f) ||
                            (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.Y > 0.8f))
                        {
                            int z = (int)gameState.State.Index.Z - 1;
                            if (z < 1)
                                z = 2;
                            gameState.State.Index = new Vector3(gameState.State.Index.X, gameState.State.Index.Y, z);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }

                        // Enter menu item by pressing "A" or D-Pad-Right
                        if ((GamePad.GetState(player[i].Gamepad.Index).Buttons.A == ButtonState.Pressed) ||
                            (GamePad.GetState(player[i].Gamepad.Index).DPad.Right == ButtonState.Pressed))
                        {
                            if (gameState.State.Index.Z == 1)
                            {
                                gameState.State.Index = new Vector3(0, 0, 0);
                            }
                            else if (gameState.State.Index.Z == 2)
                            {
                                gameState.State.Index = new Vector3(1, 1, 4);
                                player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                            }

                        }
                    }

                }
            }

        }

        private static void updateStory(GameTime gameTime, GameState gameState)
        {

        }

        /// <summary>
        /// Process input in QuickPlay
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gameState"></param>
        /// <returns></returns>
        private static void updateQuickPlay(GameTime gameTime, GameState gameState)
        {

            Player[] player = gameState.GameObjects.PlayerArray.GamepadPlayers();

            // Check input when game is actually running
            if (gameState.State.Index.Z == 1)
            {
                for (int i = 0; i < player.Length; i++)
                {
                    // Back to main menu by pressing back button
                    if (GamePad.GetState(player[i].Gamepad.Index).Buttons.Back == ButtonState.Pressed)
                    {
                        gameState.State.Index = new Vector3(1, 3, 1);
                        gameState.LoadContent();
                        player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                    }

                    // Enter game menu by pressing start button
                    if (GamePad.GetState(player[i].Gamepad.Index).Buttons.Start == ButtonState.Pressed)
                    {
                        if (player[i].Gamepad.WaitingTimeForNextInput == 0)
                        {
                            gameState.State.Index = new Vector3(3, gameState.State.Index.Y, 3);
                            player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                        }
                    }

                    // Jump
                    //if (GamePad.GetState(player[i].Gamepad.Index).IsButtonDown(player[i].Gamepad.JumpButton))
                    if (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.Y > 0.8f)
                    {
                        PhysicsEngine.LetPlayerJump(gameState, player[i], 1);
                    }

                    // Move player and hand
                    if (player[i].Gamepad.MoveHandStick == Gamepad.ThumbStickSide.Left)
                    {
                        PhysicsEngine.SetPlayerSpeed(gameState, player[i], GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right);
                        PhysicsEngine.SetHandPosition(gameState, player[i], GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left);
                    }
                    else
                    {
                        PhysicsEngine.SetPlayerSpeed(gameState, player[i], GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left);
                        PhysicsEngine.SetHandPosition(gameState, player[i], GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right);
                    }

                    // For Debugging: Resetting ball position
                    if (GamePad.GetState(player[i].Gamepad.Index).Buttons.Y == ButtonState.Pressed)
                    {
                        if (player[i].Gamepad.WaitingTimeForNextInput <= 0)
                        {
                            gameState.GameObjects.Ball.Position = new Vector2((gameState.State.SafeArea.X + (0.5f * gameState.State.SafeArea.Width) - 
                                (0.5f * gameState.GameObjects.Ball.Size.X)), gameState.State.SafeArea.Y);
                            gameState.GameObjects.Ball.Speed = new Vector2(gameState.Random.Next(-20, 20), 1);

                            if (gameState.GameObjects.Ball.Speed.X == 0)
                                gameState.GameObjects.Ball.Speed = new Vector2(2, 1);
                        }
                    }

                }
            }

            else if (gameState.State.Index.Z == 2)
            {
                checkTeamManagement(gameTime, gameState, player);
            }
            else if (gameState.State.Index.Z == 3)
            {
                checkGameMenu(gameTime, gameState, player);
            }





        }


        private static void updateOptionsMenu(GameTime gameTime, GameState gameState)
        {

             Player[] player = gameState.GameObjects.PlayerArray.GamepadPlayers();

            for (int i = 0; i < player.Length; i++)
            {

                // Back to main menu by pressing B button
                if (GamePad.GetState(player[i].Gamepad.Index).Buttons.B == ButtonState.Pressed)
                {
                    if (gameState.State.Index.Y == 1)
                        gameState.State.Index = new Vector3(1, 4, 1);
                    else if (gameState.State.Index.Y == 2)
                        gameState.State.Index = new Vector3(1, 4, 2);
                    else
                        gameState.State.Index = new Vector3(1, 4, 3);

                    gameState.LoadContent();
                    player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;
                }


            }
        }


        // Helper classes

        private static void checkGameMenu(GameTime gameTime, GameState gameState, Player[] player)
        {

            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].Gamepad.WaitingTimeForNextInput == 0)
                {

                    // Leave game menu by pressing start button
                    if (GamePad.GetState(player[i].Gamepad.Index).Buttons.Start == ButtonState.Pressed)
                    {

                        gameState.State.Index = new Vector3(3, gameState.State.Index.Y, 1);
                        player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;

                    }

                }
            }

        }


        private static void checkTeamManagement(GameTime gameTime, GameState gameState, Player[] player)
        {

            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].Gamepad.WaitingTimeForNextInput == 0)
                {

                    // Leave team management by pressing A button
                    if (GamePad.GetState(player[i].Gamepad.Index).Buttons.A == ButtonState.Pressed)
                    {

                        gameState.State.Index = new Vector3(3, gameState.State.Index.Y, 1);
                        LogicEngine.resetPlayerPositions(gameState);
                        player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;

                    }

                    // Change teams
                    if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Left == ButtonState.Pressed) || (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.X < (-0.8f))
                        || (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.X < (-0.8f)))
                    {
                        if ((int)player[i].AssociatedTeam > 0)
                            player[i].AssociatedTeam--;

                        player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;

                    }

                    if ((GamePad.GetState(player[i].Gamepad.Index).DPad.Right == ButtonState.Pressed) || (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Left.X > 0.8f)
                        || (GamePad.GetState(player[i].Gamepad.Index).ThumbSticks.Right.X > 0.8f))
                    {
                        if ((int)player[i].AssociatedTeam < 2)
                            player[i].AssociatedTeam++;

                        player[i].Gamepad.WaitingTimeForNextInput = inputWaitingTime;

                    }

                }
            }

        }








    }
}
