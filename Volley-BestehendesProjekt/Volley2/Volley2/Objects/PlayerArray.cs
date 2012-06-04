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

namespace Volley2.Objects
{
    public class PlayerArray
    {
        // Variables

        private Player[] allPlayer;
        public Player[] AllPlayer
        {
            get { return allPlayer; }
            set { allPlayer = value; }
        }


        // Constructors

        public PlayerArray()
        {
            // Create all the players, first all of them are inactive
            allPlayer = new Player[4];
            allPlayer[0] = new Player(Player.Status.InActive, PlayerIndex.One, Player.Team.Left);
            allPlayer[1] = new Player(Player.Status.InActive, PlayerIndex.Two, Player.Team.Right);
            allPlayer[2] = new Player(Player.Status.InActive, PlayerIndex.Three, Player.Team.Neutral);
            allPlayer[3] = new Player(Player.Status.InActive, PlayerIndex.Four, Player.Team.Neutral);
        }


        // Methods

        /// <summary>
        /// Checks if new gamepads are connected/disconnected
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

            for (int i = 0; i < allPlayer.Length; i++)
            {
                if (GamePad.GetState(allPlayer[i].Gamepad.Index).IsConnected)
                {
                    if (allPlayer[i].PlayerStatus == Player.Status.InActive)
                    {
                        // case if a gamepad hasn't already been connected but is it now
                        allPlayer[i].PlayerStatus = Player.Status.Gamepad;
                        System.Diagnostics.Debug.WriteLine("|PlayerArray| Gamepad[" + i + "] connected (" + NumberOfGamepadPlayers().ToString() + " Gamepads connected, now)");
                    }
                }
                else
                {
                    if (allPlayer[i].PlayerStatus == Player.Status.Gamepad)
                    {
                        // case if a gamepad has been connected and is now disconnected
                        allPlayer[i].PlayerStatus = Player.Status.InActive;
                        System.Diagnostics.Debug.WriteLine("|PlayerArray| Gamepad[" + i + "] disconnected (" + NumberOfGamepadPlayers().ToString() + " Gamepads connected, now)");
                    }
                }
            }
        }

        /// <summary>
        /// returns an array with all gamepad players
        /// </summary>
        /// <returns></returns>
        public Player[] GamepadPlayers()
        {
            Player[] returnPlayers = new Player[NumberOfGamepadPlayers()];

            int tempCounter = 0;
            for (int i = 0; i < allPlayer.Length; i++)
                if (allPlayer[i].PlayerStatus == Player.Status.Gamepad)
                {
                    returnPlayers[tempCounter] = allPlayer[i];
                    tempCounter++;
                }

            return returnPlayers;
        }

        /// <summary>
        /// returns an array with all active players
        /// </summary>
        /// <returns></returns>
        public Player[] ActivePlayers()
        {
            Player[] returnPlayers = new Player[NumberOfActivePlayers()];

            int tempCounter = 0;
            for (int i = 0; i < allPlayer.Length; i++)
                if (allPlayer[i].PlayerStatus != Player.Status.InActive)
                {
                    returnPlayers[tempCounter] = allPlayer[i];
                    tempCounter++;
                }

            return returnPlayers;
        }

        /// <summary>
        /// returns an array with all players of the left team
        /// </summary>
        /// <returns></returns>
        public Player[] LeftTeamPlayer()
        {
            Player[] returnPlayers = new Player[2];

            int tempCounter = 0;
            for (int i = 0; i < allPlayer.Length; i++)
                if ((allPlayer[i].AssociatedTeam == Player.Team.Left) && (allPlayer[i].PlayerStatus != Player.Status.InActive))
                {
                    returnPlayers[tempCounter] = allPlayer[i];
                    tempCounter++;
                }

            if (tempCounter == 2)
                return returnPlayers;
            else if (tempCounter == 1)
            {
                Player[] returnPlayers2 = new Player[1];
                returnPlayers2[0] = returnPlayers[0];
                return returnPlayers2;
            }
            else
            {
                return new Player[0];
            }

        }

        /// <summary>
        /// returns an array with all players of the left team
        /// </summary>
        /// <returns></returns>
        public Player[] RightTeamPlayer()
        {
            Player[] returnPlayers = new Player[2];

            int tempCounter = 0;
            for (int i = 0; i < allPlayer.Length; i++)
                if ((allPlayer[i].AssociatedTeam == Player.Team.Right) && (allPlayer[i].PlayerStatus != Player.Status.InActive))
                {
                    returnPlayers[tempCounter] = allPlayer[i];
                    tempCounter++;
                }

            if (tempCounter == 2)
                return returnPlayers;
            else if (tempCounter == 1)
            {
                Player[] returnPlayers2 = new Player[1];
                returnPlayers2[0] = returnPlayers[0];
                return returnPlayers2;
            }
            else
            {
                return new Player[0];
            }
        }

        /// <summary>
        /// returns the total amount of connected gamepads
        /// </summary>
        /// <returns></returns>
        public int NumberOfGamepadPlayers()
        {
            int counter = 0;
            for (int i = 0; i < allPlayer.Length; i++)
                if (allPlayer[i].PlayerStatus == Player.Status.Gamepad)
                    counter++;
            return counter;
        }


        /// <summary>
        /// returns the total amount of KI players
        /// </summary>
        /// <returns></returns>
        public int NumberOfKIPlayers()
        {
            int counter = 0;
            for (int i = 0; i < allPlayer.Length; i++)
                if (allPlayer[i].PlayerStatus == Player.Status.KI)
                    counter++;
            return counter;
        }


        /// <summary>
        /// returns the total amount of inactive players
        /// </summary>
        /// <returns></returns>
        public int NumberOfInactivePlayers()
        {
            int counter = 0;
            for (int i = 0; i < allPlayer.Length; i++)
                if (allPlayer[i].PlayerStatus == Player.Status.InActive)
                    counter++;
            return counter;
        }

        /// <summary>
        /// returns the total amount of active players
        /// </summary>
        /// <returns></returns>
        public int NumberOfActivePlayers()
        {
            return (allPlayer.Length - NumberOfInactivePlayers());
        }





    }
}
