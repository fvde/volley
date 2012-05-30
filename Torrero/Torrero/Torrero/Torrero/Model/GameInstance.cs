using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Torrero.Controller;

namespace Torrero.Model
{
    public class GameInstance
    {
        // All getters, setters.
        #region Properties

        // Queue containing all current tiles. Includes precalculated ones. Tiles that have been passed will be removed from this queue.
        public GameGrid Grid
        {
            get { return grid; }
        }

        // GameObjects like the Player, bulls and senoritas
        public List<GameObject> GameObjects
        {
            get { return gameObjects; }
        }

        // Distance covered in this game.
        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        // Current score.
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        // The player.
        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        // Queue containing all current tiles. Includes precalculated ones. Tiles that have been passed will be removed from this queue.
        private GameGrid grid;

        // GameObjects like the Player, bulls and senoritas
        private List<GameObject> gameObjects;

        // Distance covered in this game.
        private float distance;

        // Current score.
        private int score;

        // The player
        private Player player;

        #endregion

        // Object constructors.
        #region Initialization

        /// <summary>
        /// Creates a new GameInstance. Distance and score are 0 at the beginning.
        /// </summary>
        /// <param name="tiles"></param>
        public GameInstance(GameGrid grid)
        {
            this.grid = grid;
            distance = 0.0f;
            score = 0;

            // Initialize GameObjects
            gameObjects = new List<GameObject>();
            player = new Player(new Vector2(220, 200), new Vector2(42, 42), StorageManager.getTextureByName("player"));
            player.CurrentTile = grid.getTileAtPosition(player.CenterPosition);
            player.NextTile = grid.getNeighborAbove(player.CurrentTile.X, player.CurrentTile.Y);
            player.DirectionAtCrossing = MovingDirection.None;

            gameObjects.Add(player);
        }

        #endregion
    }
}
