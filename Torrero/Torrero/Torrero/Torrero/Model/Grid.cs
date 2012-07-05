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
using System.Linq;
using Microsoft.Xna.Framework;
using Torrero.Controller;

namespace Torrero.Model
{
    public class GameGrid
    {
        // All getters, setters.
        #region Properties

        // Get all tiles in the Grid
        public IEnumerable<Tile> Tiles
        {
            get 
            {
                foreach (Tile[] tileRow in tiles)
                {
                    for (int i = 0; i < tileRow.Length; i++)
                    {
                        yield return tileRow[i];
                    }
                }       
            }
        }

        /// <summary>
        /// Height of the grid.
        /// </summary>
        public int Height
        {
            get { return tiles.Count; }
        }

        /// <summary>
        /// Width of the grid.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Stores the crossing which is nearest to the Player.
        /// </summary>
        public Tile NearestCrossing
        {
            set { nearestCrossing = value; }
            get { return nearestCrossing; }
        }

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        private Queue<Tile[]> tiles;

        private int width;

        private Random random;

        /// <summary>
        /// Stores the crossing which is nearest to the Player.
        /// </summary>
        private Tile nearestCrossing;        

        #endregion

        // Object constructors.
        #region Initialization

        /// <summary>
        /// Creates a new Grid
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        public GameGrid(int width)
        {
            this.width = width;

            tiles = new Queue<Tile[]>();
            random = new Random();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the tile at the specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile getTile(int x, int y)
        {
            if (x >= width || x < 0 || y >= Height || y < 0)
            {
                return null;
            }
            else
            {
                return tiles.ElementAt(y)[x];
            }
        }

        /// <summary>
        /// Returns the row at the specified y position.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public IEnumerable<Tile> getRow(int y)
        {
            return tiles.ElementAt(y);
        }

        /// <summary>
        /// Returns the tile above the specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile getNeighborAbove(int x, int y)
        {
            return getTile(x, y + 1);
        }

        /// <summary>
        /// Returns the tile below the specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile getNeighborBelow(int x, int y)
        {
            return getTile(x, y - 1);
        }

        /// <summary>
        /// Returns the tile to the right of the specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile getNeighborRight(int x, int y)
        {
            return getTile(x + 1, y);
        }

        /// <summary>
        /// Returns the tile to the left of the specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile getNeighborLeft(int x, int y)
        {
            return getTile(x - 1, y);
        }


        /// <summary>
        /// Returns the the coordinates of the Tile whith specific screen coordinates.
        /// </summary>
        /// <param name="x">Ref where x value is stored.</param>
        /// <param name="y">Ref where y value is stored.</param>
        /// <param name="pos">Position to check.</param>
        public void getTileCoordsAtPosition(ref int x, ref int y, Vector2 pos)
        {
            int count = 0;
            foreach(Tile[] tileRow in tiles)
            {
                //check if Players y position is lower than the next tiles y position
                if (pos.Y < tileRow[0].BottomLeftPosition.Y + TorreroConstants.TileSize)
                {
                    y = count;
                    x = (int) ((pos.X + TorreroConstants.TileSizeHalf) / TorreroConstants.TileSize);
                    return;
                }
                count++;
            }
        }

        /// <summary>
        /// Finds the Tile at the given screen coordinates.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <returns>The Tile which is at the given coordinates.</returns>
        public Tile getTileAtPosition(Vector2 pos)
        {
            int x = 0, y = 0;
            getTileCoordsAtPosition(ref x, ref y, pos);
            return getTile(x, y);
        }


        /// <summary>
        /// Returns a random free tile, moving up, left or right (from the location).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile getRandomFreeTile(int x, int y)
        {
            int maximumAttempts = 5;
            Tile temp;

            while (maximumAttempts > 0)
            {
                maximumAttempts--;
                temp = getTileInRandomDirection(x, y);

                if (temp != null)
                {
                    if (temp is Obstacle)
                    {
                        return temp;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the next tile in the direction
        /// </summary>
        /// <param name="current"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        public Tile getNextTileInDirection(Tile current, Tile previous)
        {
            // Up
            if (current.Y > previous.Y)
            {
                return getTile(current.X, current.Y + 1);
            }
            // Left
            else if (current.X < previous.X)
            {
                return getTile(current.X - 1, current.Y);
            }
            // Right
            else
            {
                return getTile(current.X + 1, current.Y);
            }
        }

        /// <summary>
        /// Gets a tile to the left, right or above.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Tile getTileInRandomDirection(int x, int y)
        {
            switch (random.Next(3))
            {
                case 0:
                    {
                        return getNeighborLeft(x, y);
                    }
                case 1:
                    {
                        return getNeighborAbove(x, y);
                    }
                default:
                    {
                        return getNeighborRight(x, y);
                    }
            }
        }

        /// <summary>
        /// Updates the x and y positions after throwing away rows.
        /// </summary>
        public void updateXYPositions()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    getTile(x, y).X = x;
                    getTile(x, y).Y = y;  
                }
            }
        }

        /// <summary>
        /// Sets this element to be a street.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setStreet(int x, int y)
        {
            tiles.ElementAt(y)[x] = new Street(x, y, tiles.ElementAt(y)[x].BottomLeftPosition, StorageManager.getTextureByName("street_vertical"));
        }

        /// <summary>
        /// Adds a new row of obstacles.
        /// </summary>
        public void addObstacleRow()
        {
            Tile[] newRow = new Tile[width];
            float yPosition = 0.0f;

            if (Height > 0)
            {
                yPosition = getTile(0, Height - 1).BottomLeftPosition.Y + TorreroConstants.TileSize;
            }

            for (int pos = 0; pos < width; pos++)
            {               
                newRow[pos] = new Obstacle(pos, tiles.Count, new Vector2(pos * TorreroConstants.TileSize - TorreroConstants.TileSizeHalf, yPosition), StorageManager.getTextureByName("obstacle"));
            }

            tiles.Enqueue(newRow);
        }

        /// <summary>
        /// Returns true if the player has already passed the lowest row of tiles.
        /// </summary>
        /// <returns></returns>
        public bool lowestRowHasBeenPassed()
        {
            return (getTile(0,0).BottomLeftPosition.Y < -TorreroConstants.TileSize);
        }

        /// <summary>
        /// Removes the lowest row from the queue.
        /// </summary>
        public void discardLowestRow()
        {
            tiles.Dequeue();
        }

        public Tile findNextPathTile(Tile currentTile, Tile lastTile)
        {
            Tile t = getNeighborAbove(currentTile.X, currentTile.Y);

            if (t is Street && t != lastTile) return t;
            t = getNeighborLeft(currentTile.X, currentTile.Y);
            if (t is Street && t != lastTile) return t;
            t = getNeighborRight(currentTile.X, currentTile.Y);
            if (t is Street && t != lastTile) return t;
            t = getNeighborBelow(currentTile.X, currentTile.Y);
            if (t is Street && t != lastTile) return t;

            return lastTile;
        }

        /// <summary>
        /// Finds the next crossing.
        /// </summary>
        /// <param name="x">X value of the Tile you want to check.</param>
        /// <param name="y">Y value of the Tile you want to check.</param>
        /// <param name="direction">Search direction. 0 = right, 1 = left, 2 = top, 3 = bottom. This is necessary to avoid infinite loops.</param>
        public void findNearestCrossing(int x, int y, int direction)
        {            
            if (!isStreet(x, y) || nearestCrossing != null) return;
            System.Diagnostics.Debug.WriteLine("check " + x + " " + y);

            //check if there are 3 streets around the current tile => current tile is a crossing
            if (isCrossing(x, y))
            {
                System.Diagnostics.Debug.WriteLine("found " + x + " " + y);
                nearestCrossing = getTile(x, y);
                return;
            }
            else
            {
                //check tile left, right top and bottom
                switch (direction)
                {
                    case 0:
                        findNearestCrossing(x, y + 1, 2);
                        findNearestCrossing(x, y - 1, 3);
                        findNearestCrossing(x + 1, y, 0);                        
                        break;
                    case 1:
                        findNearestCrossing(x, y + 1, 2);
                        findNearestCrossing(x, y - 1, 3);
                        findNearestCrossing(x - 1, y, 1);
                        break;
                    case 2:
                        findNearestCrossing(x, y + 1, 2);
                        findNearestCrossing(x + 1, y, 0);
                        findNearestCrossing(x - 1, y, 1);
                        break;
                    case 3:
                        findNearestCrossing(x, y - 1, 3);
                        findNearestCrossing(x + 1, y, 0);
                        findNearestCrossing(x - 1, y, 1);
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if this street is a crossing.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool isCrossing(int x, int y)
        {
            int streetCounter = 0;

            if (isStreet(x + 1, y))
            {
                streetCounter++;
            }

            if (isStreet(x - 1, y))
            {
                streetCounter++;
            }

            if (isStreet(x, y + 1))
            {
                streetCounter++;
            }

            if (isStreet(x, y - 1))
            {
                streetCounter++;
            }

            return streetCounter > 2;
        }

        /// <summary>
        /// Checks if the tile with the given coordinates is a Street.
        /// </summary>
        /// <param name="x">X value of the Tile.</param>
        /// <param name="y">Y value of the Tile.</param>
        /// <returns>true if it is a Street, false otherwise.</returns>
        public bool isStreet(int x, int y)
        {
            return isStreet(getTile(x, y));
        }

        /// <summary>
        /// Checks if the tile with the given coordinates is a Street.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool isStreet(Tile t)
        {
            return t != null && t is Street;
        }

        /// <summary>
        /// Returns true if the coodrinates point to an obstacle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool isObstacle (int x, int y)
        {
            return isObstacle(getTile(x, y));
        }

        /// <summary>
        /// Returns true if the coodrinates point to an obstacle.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool isObstacle(Tile t)
        {
            return t != null && t is Obstacle;
        }


        /// <summary>
        /// Sets all street texture until the row n-1.
        /// </summary>
        /// <param name="blockSize"></param>
        public void setStreetTextures(int blockSize)
        {
            for (int row = Height - blockSize - 1; row < Height; row++)
            {
                foreach (Tile t in getRow(row))
                {
                    if (t is Street)
                    {
                        chooseStreetTexture(t);
                    }
                }
            }
        }

        private void chooseStreetTexture(Tile t)
        {
            if (isStreetHorizontal(t))
            {
                t.Texture = StorageManager.getTextureByName("street_horizontal");
            }
            else if (isStreetVertical(t))
            {
                t.Texture = StorageManager.getTextureByName("street_vertical");
            }
            else if (isTurnDownLeft(t))
            {
                t.Texture = StorageManager.getTextureByName("turn_down_left");
            }
            else if (isTurnRightDown(t))
            {
                t.Texture = StorageManager.getTextureByName("turn_right_down");
            }
            else if (isTurnUpLeft(t))
            {
                t.Texture = StorageManager.getTextureByName("turn_up_left");
            }
            else if (isTurnUpRight(t))
            {
                t.Texture = StorageManager.getTextureByName("turn_up_right");
            }
            else if (isDeadendDown(t))
            {
                t.Texture = StorageManager.getTextureByName("deadend_down");
            }
            else if (isDeadendLeft(t))
            {
                t.Texture = StorageManager.getTextureByName("deadend_left");
            }
            else if (isDeadendRight(t))
            {
                t.Texture = StorageManager.getTextureByName("deadend_right");
            }
            else if (isCrossingRightDownLeft(t))
            {
                t.Texture = StorageManager.getTextureByName("crossing_right_down_left");
            }
            else if (isCrossingUpDownLeft(t))
            {
                t.Texture = StorageManager.getTextureByName("crossing_up_down_left");
            }
            else if (isCrossingUpRightDown(t))
            {
                t.Texture = StorageManager.getTextureByName("crossing_up_right_down");
            }
            else if (isCrossingUpRightLeft(t))
            {
                t.Texture = StorageManager.getTextureByName("crossing_up_right_left");
            }
            else if (is4WayCrossing(t))
            {
                t.Texture = StorageManager.getTextureByName("crossing_up_right_down_left");
            }
            else
            {
                t.Texture = StorageManager.getTextureByName("street");
            }
        }

        #region Street Types
        
        /// <summary>
        /// Is vertical street.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isStreetHorizontal(Tile t){
            return 
                isObstacle(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isObstacle(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is vertical street.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isStreetVertical(Tile t)
        {
            return
                isStreet(getNeighborAbove(t.X, t.Y)) &&
                isObstacle(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isObstacle(getNeighborLeft(t.X, t.Y));
        }


        /// <summary>
        /// Is crossing_right_down_left
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isCrossingRightDownLeft(Tile t)
        {
            return
                isObstacle(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is crossing_up_down_left
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isCrossingUpDownLeft(Tile t)
        {
            return
                isStreet(getNeighborAbove(t.X, t.Y)) &&
                isObstacle(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is crossing_up_right_down
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isCrossingUpRightDown(Tile t)
        {
            return
                isStreet(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isObstacle(getNeighborLeft(t.X, t.Y));
        }


        /// <summary>
        /// Is crossing_up_right_left
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isCrossingUpRightLeft(Tile t)
        {
            return
                isStreet(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isObstacle(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is 4 way crossing
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool is4WayCrossing(Tile t)
        {
            return
                isStreet(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is deadend_down
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isDeadendDown(Tile t)
        {
            return
                isObstacle(getNeighborAbove(t.X, t.Y)) &&
                isObstacle(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isObstacle(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is deadend_left
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isDeadendLeft(Tile t)
        {
            return
                isObstacle(getNeighborAbove(t.X, t.Y)) &&
                isObstacle(getNeighborRight(t.X, t.Y)) &&
                isObstacle(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is deadend_right
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isDeadendRight(Tile t)
        {
            return
                isObstacle(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isObstacle(getNeighborBelow(t.X, t.Y)) &&
                isObstacle(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is turn_down_left
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isTurnDownLeft(Tile t)
        {
            return
                isObstacle(getNeighborAbove(t.X, t.Y)) &&
                isObstacle(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is turn_right_down
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isTurnRightDown(Tile t)
        {
            return
                isObstacle(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isStreet(getNeighborBelow(t.X, t.Y)) &&
                isObstacle(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is turn_up_left
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isTurnUpLeft(Tile t)
        {
            return
                isStreet(getNeighborAbove(t.X, t.Y)) &&
                isObstacle(getNeighborRight(t.X, t.Y)) &&
                isObstacle(getNeighborBelow(t.X, t.Y)) &&
                isStreet(getNeighborLeft(t.X, t.Y));
        }

        /// <summary>
        /// Is turn_up_right
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isTurnUpRight(Tile t)
        {
            return
                isStreet(getNeighborAbove(t.X, t.Y)) &&
                isStreet(getNeighborRight(t.X, t.Y)) &&
                isObstacle(getNeighborBelow(t.X, t.Y)) &&
                isObstacle(getNeighborLeft(t.X, t.Y));
        }

        #endregion

        #endregion
    }
}
