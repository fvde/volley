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
using Volley2.Objects;
using Volley2.InOutput;

namespace Volley2.Logic
{
    public static class PhysicsEngine
    {

        // Constants

        // Turn effects on/off 
        static bool enableGravity = true;
        static bool enableFriction = true;
        static bool enableBallHandCollision = true;
        static bool enableBallNetCollision = true;
        static bool enablePlayerNetCollision = true;

        // defines intensity (value range: 0 - 1(highest))
        static float gravityIntensity = 0.8f;
        // defines intensity (value range: 1 - 0(highest)
        static float frictionIntensity = 0.85f;

        // defines maximum speed of player in x-direction
        static float maxSpeed_Player_X = 7.0f;
        // defines jump speed for jumpStyle1
        static float speed_jumpStyle1 = 11f;

        // defines maximum range of hand
        static float maxRange_Hand = 50f;
        

        // Methods


        // ---- main methods ----

        public static void Update(GameTime gameTime, GameState gameState)
        {


            // Just update when game is actually running
            if (((gameState.State.Index.X == 3) && (gameState.State.Index.Z == 1)))
            {

                if (enableGravity)
                    updateGravityInflucence(gameState);
                if (enableFriction)
                    updateFrictionInfluence(gameState);

                movePlayer(gameState);
                moveHand(gameState);
                moveBall(gameState);

                checkBorders(gameState);

                if (enablePlayerNetCollision)
                    checkPlayerNetCollision(gameState);
                if (enableBallNetCollision)
                    checkBallNetCollision(gameState);
                if (enableBallHandCollision)
                    checkBallHandCollision(gameState);


            }



        }


        // ---- Speed and Movement ----

        // Player

        public static void SetPlayerSpeed(GameState gameState, Player player, Vector2 direction)
        {
            // if player and stick both point to the right
            if ((direction.X > 0) &&  (player.Speed.X >= 0))
            {
                // speed up where applicable
                if ((direction.X * maxSpeed_Player_X) > player.Speed.X)
                    player.Speed = new Vector2((direction.X * maxSpeed_Player_X), player.Speed.Y);
            }

            // if player goes left and stick points right
            else if ((direction.X > 0) && (player.Speed.X < 0))
            {
                player.Speed = new Vector2((direction.X * maxSpeed_Player_X) + player.Speed.X, player.Speed.Y);
            }

            // if player and stick both point to the left
            else if ((direction.X < 0) &&  (player.Speed.X <= 0)) 
            {
                // speed up where applicable
                if ((direction.X * maxSpeed_Player_X) < player.Speed.X)
                    player.Speed = new Vector2((direction.X * maxSpeed_Player_X), player.Speed.Y);
            }

            // if player goes right and stick points left
            else if ((direction.X < 0) && (player.Speed.X > 0))
            {
                player.Speed = new Vector2((direction.X * maxSpeed_Player_X) + player.Speed.X, player.Speed.Y);
            }
            

        }

        public static void LetPlayerJump(GameState gameState, Player player, int jumpStyle)
        {
            if (jumpStyle == 1)
                if (isOnGround(gameState, player))
                    player.Speed = new Vector2(player.Speed.X, -speed_jumpStyle1);
        }


        private static void movePlayer(GameState gameState)
        {
            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();
            for (int i = 0; i < player.Length; i++)
            {
                player[i].LastPosition = player[i].Position;
                player[i].Position += player[i].Speed;
            }
        }


        // Hand

        public static void SetHandPosition(GameState gameState, Player player, Vector2 direction)
        {
            player.Hand.PositionOffset = new Vector2((direction.X * maxRange_Hand) , (direction.Y * -1 * maxRange_Hand));
        }

        private static void moveHand(GameState gameState)
        {
            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();
            for (int i = 0; i < player.Length; i++)
            {
                player[i].Hand.LastPosition = player[i].Hand.Position;

                player[i].Hand.Position = new Vector2((player[i].Position.X + player[i].Hand.PositionOffset.X - (0.5f * player[i].Hand.Size.X) + 
                    (0.5f * player[i].Size.X)),
                    (player[i].Position.Y + player[i].Hand.PositionOffset.Y + (player[i].Size.Y * 0.2f)));
            }
        }


        // Ball

        private static void moveBall(GameState gameState)
        {
            Ball ball = gameState.GameObjects.Ball;

            ball.LastPosition = ball.Position;
            ball.Position += ball.Speed;
        }



        // --- Border and Net collision

        private static void checkBorders(GameState gameState)
        {
            // Check player
            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();
            for (int i = 0; i < player.Length; i++)
            {
                // left border
                if (player[i].Position.X < gameState.State.SafeArea.X)
                {
                    player[i].Position = new Vector2(gameState.State.SafeArea.X, player[i].Position.Y);
                }

                // right border
                if (player[i].Position.X > (gameState.State.SafeArea.X + gameState.State.SafeArea.Width))
                {
                    player[i].Position = new Vector2((gameState.State.SafeArea.X + gameState.State.SafeArea.Width), player[i].Position.Y);
                }

                // bottom
                if ((player[i].Position.Y  + player[i].Size.Y) > (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height))
                {
                    player[i].Position = new Vector2(player[i].Position.X, (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height - player[i].Size.Y ));
                }

                // top
                if (player[i].Position.Y < gameState.State.SafeArea.Y)
                {
                    player[i].Position = new Vector2(player[i].Position.X, gameState.State.SafeArea.Y);
                }

            }


            // Check ball
            Ball ball = gameState.GameObjects.Ball;

            // left border
            if (ball.Position.X < gameState.State.SafeArea.X)
            {
                ball.Position = new Vector2(gameState.State.SafeArea.X, ball.Position.Y);
                ball.Speed = new Vector2((ball.Speed.X * -1), ball.Speed.Y);
            }

            // right border
            if (ball.Position.X > (gameState.State.SafeArea.X + gameState.State.SafeArea.Width))
            {
                ball.Position = new Vector2((gameState.State.SafeArea.X + gameState.State.SafeArea.Width), ball.Position.Y);
                ball.Speed = new Vector2((ball.Speed.X * -1), ball.Speed.Y);
            }

            // bottom
            if ((ball.Position.Y + ball.Size.Y) > (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height))
            {
                if ((ball.Position.X + 0.5f * ball.Size.X) > (gameState.State.SafeArea.X + (0.5f * gameState.State.SafeArea.Width)))
                {
                    if (gameState.GameObjects.PlayerArray.RightTeamPlayer().Length > 0)
                    {
                        gameState.State.Score = new Vector2(gameState.State.Score.X + 1, gameState.State.Score.Y);

                        // let gamepads rumble for loosing team
                        foreach (Player p in gameState.GameObjects.PlayerArray.RightTeamPlayer())
                        {
                            gameState.Rumble.set(p.Gamepad.Index, 500, 1, 1);
                        }

                        gameState.GameObjects.Ball.Position = new Vector2((gameState.State.SafeArea.X + (0.5f * gameState.State.SafeArea.Width) -
                            (0.5f * gameState.GameObjects.Ball.Size.X)), gameState.State.SafeArea.Y);
                        ball.Speed = new Vector2(0, 0);
                    }

                    else
                    {
                        // if no player is in team, let ball bounce back
                        ball.Position = new Vector2(ball.Position.X, (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height - ball.Size.Y));
                        ball.Speed = new Vector2(gameState.Random.Next(-15, 5), gameState.Random.Next(-20, -15));
                    }
                }   
                else
                {
                    if (gameState.GameObjects.PlayerArray.LeftTeamPlayer().Length > 0)
                    {
                        gameState.State.Score = new Vector2(gameState.State.Score.X, gameState.State.Score.Y + 1);

                        // let gamepads rumble for loosing team
                        foreach (Player p in gameState.GameObjects.PlayerArray.LeftTeamPlayer())
                        {
                            gameState.Rumble.set(p.Gamepad.Index, 500, 1, 1);
                        }

                        gameState.GameObjects.Ball.Position = new Vector2((gameState.State.SafeArea.X + (0.5f * gameState.State.SafeArea.Width) -
                            (0.5f * gameState.GameObjects.Ball.Size.X)), gameState.State.SafeArea.Y);
                        ball.Speed = new Vector2(0, 0);
                    }
                    else
                    {
                        // if no player is in team, let ball bounce back
                        ball.Position = new Vector2(ball.Position.X, (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height - ball.Size.Y));
                        ball.Speed = new Vector2(gameState.Random.Next(-5, 15), gameState.Random.Next(-20, -15));
                    }
                }


            }


        }

        private static void checkPlayerNetCollision(GameState gameState)
        {

            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].destinationRectangle().Intersects(gameState.GameObjects.Net.destinationRectangle()))
                {
                    player[i].Position = new Vector2(player[i].LastPosition.X, player[i].Position.Y);
                }


            }


        }

        private static void checkBallNetCollision(GameState gameState)
        {
            Ball ball = gameState.GameObjects.Ball;
            Net net = gameState.GameObjects.Net;

            if (ball.destinationRectangle().Intersects(gameState.GameObjects.Net.destinationRectangle()))
            {
                // Move ball till there is no more intersection
                while (ball.destinationRectangle().Intersects(gameState.GameObjects.Net.destinationRectangle()))
                {
                    ball.Position = new Vector2((float)(ball.Position.X + (-0.1 * ball.Speed.X)), (float)(ball.Position.Y + (-0.1 * ball.Speed.Y)));
                }

                // Check if ball collides with net's top
                if (ball.Position.Y > net.Position.Y)
                {
                    // if not
                    ball.Speed = new Vector2((ball.Speed.X * (-1)), ball.Speed.Y);
                }
                else
                {
                    // if so
                    ball.Speed = new Vector2((ball.Speed.X * (-1)), (ball.Speed.Y * (-1)));
                }
            }

        }

        private static void checkBallHandCollision(GameState gameState)
        {
            Ball ball = gameState.GameObjects.Ball;
            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();

            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].Hand.DestinationRectangle().Intersects(ball.destinationRectangle()))
                {
                    // Move ball till there is no more intersection
                    while (player[i].Hand.DestinationRectangle().Intersects(ball.destinationRectangle()))
                    {
                        ball.Position = new Vector2((float)(ball.Position.X + (-0.1 * ball.Speed.X)), (float)(ball.Position.Y + (-0.1 * ball.Speed.Y)));
                    }

                    // Set ball's new speed
                    /*
                    Vector2 helper = ((ball.Position + (0.5f * ball.Size) - (player[i].Hand.Position + (0.5f * player[i].Hand.Size))) /
                        (0.5f * player[i].Hand.Size) + (0.5f * ball.Size)) * (float)(Math.PI / 2);

                    ball.Speed = new Vector2((float)((Math.Sin(-1 * Math.Abs(helper.X))) * ball.Speed.X), (float)((Math.Sin(Math.Abs(helper.X))) * ball.Speed.Y));*/
                    ball.Speed = new Vector2(ball.Speed.X * -1, ball.Speed.Y * -1);
                    ball.Speed += (player[i].Hand.Speed() * 0.1f);
                }

            }

        }

        // ---- Physics ----

        // Gravity
        private static void updateGravityInflucence(GameState gameState)
        {

            // on player
            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();

            for (int i = 0; i < player.Length; i++)
            {
                if (isOnGround(gameState, player[i]))
                {
                    // stops increasing y-Speed when on ground
                    if (player[i].Speed.Y > 0)
                        player[i].Speed = new Vector2(player[i].Speed.X, 0);
                }
                else
                {
                    // pushes player downwards
                    player[i].Speed += new Vector2(0, (1 * gravityIntensity));
                }
            }

            // on ball
            if (gameState.GameObjects.Ball.Speed.Y != 0)
                gameState.GameObjects.Ball.Speed += new Vector2(0, (1 * gravityIntensity));
        }

        // Friction (slows ball and player down when on ground
        private static void updateFrictionInfluence(GameState gameState)
        {

            // on player
            Player[] player = gameState.GameObjects.PlayerArray.ActivePlayers();

            for (int i = 0; i < player.Length; i++)
            {
                if (isOnGround(gameState, player[i]))
                {
                    // slow down x-speed when on ground
                    if (player[i].Speed.X != 0)
                    {
                        player[i].Speed = new Vector2((player[i].Speed.X * frictionIntensity), player[i].Speed.Y);

                        if ((player[i].Speed.X > -0.5f) && (player[i].Speed.X < 0.5f))
                            player[i].Speed = new Vector2(0, player[i].Speed.Y);
                    }
                }
            }

            // on ball
        }



        // ---- Helper Classes ----

        private static bool isOnGround(GameState gameState, Player player)
        {
            if ((player.Position.Y + player.Size.Y) > (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height - 2))
                return true;
            else
                return false;
        }

        private static bool isOnGround(GameState gameState, Ball ball)
        {
            if ((ball.Position.Y + ball.Size.Y) > (gameState.State.SafeArea.Y + gameState.State.SafeArea.Height - 2))
                return true;
            else
                return false;
        }

    }
}
