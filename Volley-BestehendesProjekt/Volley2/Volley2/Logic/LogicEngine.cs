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

namespace Volley2.Logic
{
    public static class LogicEngine
    {

        // Methods


        /// <summary>
        /// Resets all player
        /// </summary>
        /// <param name="gameState"></param>
        public static void resetPlayerPositions(GameState gameState)
        {
            resetPlayerPositions(gameState, gameState.GameObjects.PlayerArray.AllPlayer);
        }

        /// <summary>
        /// Resets given player
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="player"></param>
        public static void resetPlayerPositions(GameState gameState, Player[] player)
        {
            for (int i = 0; i < player.Length; i++)
                if (player[i].AssociatedTeam == Player.Team.Left)
                    player[i].Position = new Vector2(gameState.State.SafeArea.X + (gameState.State.SafeArea.Width / 4),
                        gameState.State.SafeArea.Y + gameState.State.SafeArea.Height - player[i].Size.Y);
                else if (player[i].AssociatedTeam == Player.Team.Neutral)
                    player[i].Position = new Vector2(0, 0);
                else if (player[i].AssociatedTeam == Player.Team.Right)
                    player[i].Position = new Vector2(gameState.State.SafeArea.X + 3 * (gameState.State.SafeArea.Width / 4),
                        gameState.State.SafeArea.Y + gameState.State.SafeArea.Height - player[i].Size.Y);
        }
            

    }
}
