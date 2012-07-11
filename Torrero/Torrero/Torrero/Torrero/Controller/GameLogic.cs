using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO.IsolatedStorage;
using System.Linq;

using Torrero.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace Torrero.Controller
{
    public enum MovingDirection
    {
        Left,
        Right,
        Top,
        None
    }

    public class GameLogic
    {
        // All getters, setters.
        #region Properties

        /// <summary>
        /// Necessary instance for Singleton Pattern
        /// </summary>
        public static GameLogic Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        /// <summary>
        /// Instance of GameLogic for Singleton.
        /// </summary>
        private static GameLogic instance;

        /// <summary>
        /// Settings with highscores.
        /// </summary>
        private IsolatedStorageSettings appSettings;

        /// <summary>
        /// The current GameInstance.
        /// </summary>
        private GameInstance g;

        /// <summary>
        /// True if the player already reached the next crossing.
        /// </summary>
        private bool reachedCrossing = false;

        private Tile reachedCrossingTile;

        /// <summary>
        /// Movement in x and y direction.
        /// </summary>
        private float xMovement = 0, yMovement = 0;

        /// <summary>
        /// Time since last difficulty update.
        /// </summary>
        private float timeSinceLastDifficultyUpdate = 0.0f;

        #endregion

        // Object constructors.
        #region Initialization

        /// <summary>
        /// Create a new gamelogic.
        /// </summary>
        public GameLogic(GameInstance gameInstance)
        {
            instance = this;

            appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!appSettings.Contains("highscoresLocal"))
            {
                appSettings["highscoresLocal"] = new LinkedList<HighscoreItem>();
            }
            g = gameInstance;
        }

        #endregion

        #region Methods

        public void update(TimeSpan t)
        {
            // Do stuff with it

            // TODOS:
            // 1) Move player, according to thumb position
            // 2) Move all objects down if player is moving up
            // 3) Update points and distance
            // 4) Check for collisions with object and/or bulls   
            // 5) Update difficulty

            // 1) & 2) & 3) Do Movement and calculate points            
            processMovement(t);

            // 4) Check colisions
            checkGameObjectsForCollisions(g);

            // 5) Update difficulty
            updateDifficulty(t.Milliseconds);
        }

        /// <summary>
        /// Updates the difficulty
        /// </summary>
        private void updateDifficulty(int milliseconds)
        {
            timeSinceLastDifficultyUpdate += milliseconds;

            if (timeSinceLastDifficultyUpdate < TorreroConstants.DifficultyInterval)
            {
                return;
            }
            else
            {
                timeSinceLastDifficultyUpdate = 0;

                // Percentages
                TorreroConstants.CrossingProbability += (1 - TorreroConstants.CrossingProbability) * TorreroConstants.DifficultyGrowthFactor;
                TorreroConstants.TurnProbability += (1 - TorreroConstants.TurnProbability) * TorreroConstants.DifficultyGrowthFactor;
                TorreroConstants.ContinueSidePathProbability += (1 - TorreroConstants.ContinueSidePathProbability) * TorreroConstants.DifficultyGrowthFactor;

                //Scalars
                TorreroConstants.HorizontalMaximumSpeed *= (1 + TorreroConstants.DifficultyGrowthFactor);
                TorreroConstants.VerticalSpeed *= (1 + TorreroConstants.DifficultyGrowthFactor);
            }

        }

        /// <summary>
        /// Move Player.
        /// </summary>
        /// <param name="t">The current time.</param>
        private void processMovement(TimeSpan t)
        {
            xMovement = getXMovement(g.Player, t.Milliseconds);
            yMovement = getYMovement(g.Player, t.Milliseconds);

            if (reachedCrossing && isCompletelyOutOfTile(g.Player, reachedCrossingTile))
            {
                //player left crossing => calculate new one
                reachedCrossingTile = null;
                reachedCrossing = false;
            }

            if (isCompletelyOnTile(g.Player, g.Player.NextTile) && !reachedCrossing)
            {
                //is on crossing
                if (g.Grid.NearestCrossing == g.Player.NextTile)
                {
                    xMovement = 0;
                    yMovement = 0;
                    reachedCrossing = true;
                    reachedCrossingTile = g.Grid.NearestCrossing;
                }
                //calculate next tile because player is not on a crossing
                if (!reachedCrossing)
                {
                    Tile temp = g.Player.NextTile;
                    g.Player.NextTile = g.Grid.findNextPathTile(g.Player.NextTile, g.Player.CurrentTile);
                    g.Player.CurrentTile = temp;
                }
            }
            else
            {
                g.Player.Left += xMovement;
                // Y Direction: Move everything else down
                foreach (Tile tile in g.Grid.Tiles)
                {
                    tile.BottomLeftPosition -= new Vector2(0.0f, yMovement);
                }
                foreach (GameObject gameObj in g.GameObjects)
                {
                    if (!(gameObj is Player))
                        gameObj.BottomLeftPosition -= new Vector2(0.0f, yMovement);
                }
                g.Distance += yMovement;
            }

            if (reachedCrossing && g.Player.DirectionAtCrossing != MovingDirection.None)
            {
                switch (g.Player.DirectionAtCrossing)
                {
                    case MovingDirection.Left:
                        g.Player.NextTile = g.Grid.getNeighborLeft(reachedCrossingTile.X, reachedCrossingTile.Y);
                        break;
                    case MovingDirection.Top:
                        g.Player.NextTile = g.Grid.getNeighborAbove(reachedCrossingTile.X, reachedCrossingTile.Y);
                        break;
                    case MovingDirection.Right:
                        g.Player.NextTile = g.Grid.getNeighborRight(reachedCrossingTile.X, reachedCrossingTile.Y);
                        break;
                }

                g.Player.CurrentTile = reachedCrossingTile;
                checkForNextCrossing(false, g.Player.NextTile.X, g.Player.NextTile.Y);

                g.Player.DirectionAtCrossing = MovingDirection.None;
            }

            // 3) Calculate score
            g.Score = (int)g.Distance;
        }

        public void checkForNextCrossing(bool stepUpdate)
        {
            checkForNextCrossing(stepUpdate, g.Player.CurrentTile.X, g.Player.CurrentTile.Y);
        }

        /// <summary>
        /// Finds the next crossing.
        /// </summary>
        public void checkForNextCrossing(bool stepUpdate, int x, int y)
        {
            if ((stepUpdate && g.Grid.NearestCrossing == null) || (!stepUpdate))
            {
                // check for next crossing in moving direction
                g.Grid.findNearestCrossing(x, y, 2);

                if(g.Grid.NearestCrossing != null)
                {
                    g.GameObjects.Add(new Senorita(g.Grid.NearestCrossing.BottomLeftPosition, new Vector2(TorreroConstants.TileSize), StorageManager.getTextureByName("senorita")));
                }
            }
        }

        /// <summary>
        /// Gets the movement in X direction.
        /// </summary>
        /// <param name="p">The Player.</param>
        /// <param name="milliseconds">Elapsed time in ms.</param>
        /// <returns>The movement in x direction.</returns>
        private float getXMovement(Player p, int milliseconds){
            float diff = p.NextTile.CenterPosition.X - p.CenterPosition.X;

            //snap player to tile pos
            if (Math.Abs(diff) < TorreroConstants.movingTreshhold)
                return diff;

            return TorreroConstants.VerticalSpeed * (milliseconds / 1000.0f) * Math.Sign(diff);
        }

        /// <summary>
        /// Gets the movement in Y direction.
        /// </summary>
        /// <param name="p">The Player.</param>
        /// <param name="milliseconds">Elapsed time in ms.</param>
        /// <returns>The movement in Y direction.</returns>
        private float getYMovement(Player p, int milliseconds)
        {
            float diff = p.NextTile.CenterPosition.Y - p.CenterPosition.Y;

            //snap player to tile pos
            if (Math.Abs(diff) < TorreroConstants.movingTreshhold)
                return diff;

            return TorreroConstants.HorizontalMaximumSpeed * (milliseconds / 1000.0f) * Math.Sign(diff);
        }

        /// <summary>
        /// Save highscore.
        /// </summary>
        /// <param name="score">The points the player achieved in one run.</param>
        /// <param name="name">The name of the player.</param>
        private void calculateHighscore(int score, String name)
        {

            // Getting LinkedList from Settings
            LinkedList<HighscoreItem> highscores = (LinkedList<HighscoreItem>)appSettings["highscoresLocal"];

            // Taking first Element to step through list
            LinkedListNode<HighscoreItem> curr = highscores.First;
            int count = 0;

            // searching for right spot to insert the new record
            while (curr != null && curr.Value.Score >= score)
            {
                count++;
                curr = curr.Next;
            }


            if (curr == null)
            {
                // We will not save more that 25 highscoreItems
                if (count < 25)
                    highscores.AddLast(new HighscoreItem(count + 1, name, score));
            }
            else
            {
                // We will not save more that 25 highscoreItems
                if (count < 25)
                {
                    highscores.AddBefore(curr, new HighscoreItem(count + 1, name, score));
                    curr.Value.Place++;
                    while (curr.Next != null)
                    {
                        // correcting the place of the following Records
                        curr = curr.Next;
                        curr.Value.Place++;
                    }
                }
            }

            while (highscores.Count > 25)
                highscores.RemoveLast();

            // set&save settings
            appSettings["highscoresLocal"] = highscores;
            appSettings.Save();
        }

        /// <summary>
        /// Checks if the movement is valid and returns true if it is so.
        /// </summary>
        /// <param name="g">GameInstance.</param>
        /// <param name="xMovement">Movement in x direction.</param>
        /// <returns>True if movement is valid.</returns>
        private bool checkValidXMovement(GameInstance g, float xMovement)
        {
            if (xMovement == 0f || g.Player.Left + xMovement < 0 + TorreroConstants.movingTreshhold || g.Player.Right + xMovement > 480 - TorreroConstants.movingTreshhold) return false;

            //add safety offset
            xMovement += Math.Sign(xMovement) * (TorreroConstants.movingTreshhold+1);

            int x=0, y=0;
            g.Grid.getTileCoordsAtPosition(ref x, ref y, g.Player.CenterPosition);            

            //creates a rectangle around the player
            Rectangle playerRect = new Rectangle((int) (g.Player.BottomLeftPosition.X + xMovement), (int) g.Player.BottomLeftPosition.Y, 
                                                 (int) g.Player.Size.X, (int) g.Player.Size.Y);

            //check which tiles may collide
            int[,] checkTiles = { { x + 1 * (int)Math.Sign(xMovement), y - 1 },
                                  { x + 1 * (int)Math.Sign(xMovement), y },
                                  { x + 1 * (int)Math.Sign(xMovement), y + 1 } };

            for(int i=0; i<3; i++)
            {
                Tile t = g.Grid.getTile(checkTiles[i, 0], checkTiles[i, 1]);
                if (!(t is Obstacle)) continue;                

                //create rectangle around tiles which are next to player and check if they intersect with the players rect
                Rectangle tileRect = new Rectangle((int)t.BottomLeftPosition.X, (int)t.BottomLeftPosition.Y,
                    TorreroConstants.TileSize, TorreroConstants.TileSize);

                if (tileRect.Intersects(playerRect))
                    return false;
            }           

            return true;
        }

        /// <summary>
        /// Checks if the movement is valid and returns true if it is so.
        /// </summary>
        /// <param name="g">GameInstance.</param>
        /// <param name="xMovement">Movement in y direction.</param>
        /// <returns>True if movement is valid.</returns>
        private bool checkValidYMovement(GameInstance g, float yMovement)
        {
            //add safety offset
            yMovement += TorreroConstants.movingTreshhold + 2;

            int x = 0, y = 0;
            g.Grid.getTileCoordsAtPosition(ref x, ref y, g.Player.CenterPosition);

            //creates a rectangle around the player
            Rectangle playerRect = new Rectangle((int)g.Player.BottomLeftPosition.X, (int) (g.Player.BottomLeftPosition.Y+yMovement),
                                                 (int)g.Player.Size.X, (int)g.Player.Size.Y);

            //check which tiles may collide
            int[,] checkTiles = { { x - 1, y + 1 }, { x, y + 1 }, { x + 1, y + 1 } };

            for (int i = 0; i < 3; i++)
            {
                Tile t = g.Grid.getTile(checkTiles[i, 0], checkTiles[i, 1]);

                if (!(t is Obstacle)) continue;

                //create rectangle around tiles which are next to player and check if they intersect with the players rect
                Rectangle tileRect = new Rectangle((int)t.BottomLeftPosition.X, (int)t.BottomLeftPosition.Y,
                    TorreroConstants.TileSize, TorreroConstants.TileSize);

                if (tileRect.Intersects(playerRect))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks collisions between all GameObjects.
        /// </summary>
        /// <param name="g">GameInstance that contains the objects.</param>
        private void checkGameObjectsForCollisions(GameInstance g)
        {
            if (g.GameObjects.Count < 2) return;

            foreach (GameObject o in g.GameObjects)
            {
                if ((g.Player.CenterPosition - o.CenterPosition).Length() <= 5)
                {
                    //do something on collision
                    if (o is Horse)
                    {

                    }
                    else
                    if (o is Senorita)
                    {

                    }
                    else
                    if (o is Bulls)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Checks if the GameObject completely intersects with the tile.
        /// </summary>
        /// <param name="o">The GameObject you want to check.</param>
        /// <param name="t">The tile you want to check.</param>
        /// <returns>True if the Object is completely on the Tile.</returns>
        private bool isCompletelyOnTile(GameObject o, Tile t)
        {
            if (o == null || t == null) return false;

            //check if tile and object share the same coordinates in grid and if object is completely on the tile
            if (o.Top < t.BottomLeftPosition.Y + TorreroConstants.TileSize && o.BottomLeftPosition.Y > t.BottomLeftPosition.Y &&
                o.Left > t.BottomLeftPosition.X && o.Right < t.BottomLeftPosition.X + TorreroConstants.TileSize) return true;
            return false;
        }

        /// <summary>
        /// Checks if the GameObject and the Tile don't overlap.
        /// </summary>
        /// <param name="o">The GameObject you want to check.</param>
        /// <param name="t">The tile you want to check.</param>
        /// <returns>True if the Object and the Tile don't overlap.</returns>
        private bool isCompletelyOutOfTile(GameObject o, Tile t)
        {
            if (o == null || t == null) return false;

            //check if tile and object share the same coordinates in grid and if object is completely on the tile
            if (o.Top < t.BottomLeftPosition.Y || o.BottomLeftPosition.Y > t.BottomLeftPosition.Y + TorreroConstants.TileSize ||
                o.Right < t.BottomLeftPosition.X || o.Left > t.BottomLeftPosition.X + TorreroConstants.TileSize) return true;
            return false;
        }

        #endregion
    }
}
