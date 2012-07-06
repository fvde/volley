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
using Torrero.Model;

namespace Torrero.Controller
{
    public class GridGenerator
    {
        // All getters, setters.
        #region Properties

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random random;

        private List<int> previousExitsXPositions;

        #endregion

        // Object constructors.
        #region Initialization

        /// <summary>
        /// Creates a new GridGenerator
        /// </summary>
        public GridGenerator()
        {
            random = new Random();
            previousExitsXPositions = new List<int>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the grid.
        /// </summary>
        /// <param name="g"></param>
        public void update(GameInstance g)
        {
            // Row Management
            if (g.Grid.lowestRowHasBeenPassed())
            {
                g.Grid.discardLowestRow();
                g.Grid.updateXYPositions();
            }

            while (g.Grid.Height < TorreroConstants.RowNumber)
            {
                addGridBlock(g.Grid, random.Next(TorreroConstants.MaximumBlockSize - TorreroConstants.MinimumBlockSize) + TorreroConstants.MinimumBlockSize, 1.0f);
            }
        }

        /// <summary>
        /// Creates a new grid with the specified width.
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public GameGrid getNewGrid(int height, int width)
        {
            //TODO
            return createStartingGrid(height, width);
        }

        /// <summary>
        /// Create a grid for testing purposes
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private GameGrid createStartingGrid(int height, int width)
        {
            GameGrid testGrid = new GameGrid(width);

            int tempHeight = 0;
            while (tempHeight < height)
            {
                testGrid.addObstacleRow();
                testGrid.setStreet(4, tempHeight);
                tempHeight++;
            }

            previousExitsXPositions.Add(4);

            return testGrid;
        }

        /// <summary>
        /// Creates a new grid block and adds it to the Grid.
        /// </summary>
        /// <param name="previousExits">List of all exits from the previous block.</param>
        /// <param name="blockSize">Size of the new block.</param>
        /// <param name="difficulty">Difficulty of the new block.</param>
        private void addGridBlock(GameGrid grid, int blockSize, float difficulty)
        {
            int previousHeight = grid.Height;

            createObstacleRows(grid, blockSize);

            // Find a random exit from the last block
            int pickedExit = previousExitsXPositions[random.Next(previousExitsXPositions.Count)];
            previousExitsXPositions.Remove(pickedExit);
            Tile pickedEntry = grid.getNeighborAbove(pickedExit, previousHeight - 1);

            // Set the picked entry to be a street.
            grid.setStreet(pickedEntry.X, pickedEntry.Y);

            // Calculate the main path from this entry, return all crossings
            List<Tile> crossings = new List<Tile>();

            foreach (Tile crossing in createMainPath(grid, pickedEntry, blockSize, difficulty))
            {
                crossings.Add(crossing);
            }

            foreach (Tile c in crossings)
            {
                createSidePath(grid, c, blockSize, difficulty);
            }

            grid.setStreetTextures(blockSize);
        }

        /// <summary>
        /// Create an amount of obstacle rows.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="amount"></param>
        private void createObstacleRows(GameGrid grid, int amount)
        {
            while (amount > 0)
            {
                amount--;
                grid.addObstacleRow();
            }
        }

        /// <summary>
        /// Creates a new path that crosses the complete block. The method also returns all tiles that can be crossings.
        /// </summary>
        /// <param name="entry">The tile from which the new main path starts.</param>
        /// <param name="blockSize">The size of the current block.</param>
        /// <param name="difficulty">The difficulty of the path.</param>
        /// <returns>All tiles that will be crossings</returns>
        private IEnumerable<Tile> createMainPath(GameGrid grid, Tile entry, int blockSize, float difficulty)
        {
            Tile currentTile = entry;
            Tile previousTile = null;
            Tile attemptTile = null;

            while (currentTile.Y < entry.Y + blockSize)
            {
                if (previousTile == null || TorreroConstants.TurnProbability * difficulty > random.NextDouble())
                {
                    attemptTile = grid.getRandomFreeTile(currentTile.X, currentTile.Y);
                }
                else
                {
                    attemptTile = grid.getNextTileInDirection(currentTile, previousTile);
                }

                if (attemptTile == null)
                {
                    attemptTile = grid.getNeighborAbove(currentTile.X, currentTile.Y);

                    // If there are no tiles above the tile, break;
                    if (attemptTile == null)
                    {
                        grid.setStreet(currentTile.X, currentTile.Y);
                        break;
                    }
                }

                if (isValidPath(grid, attemptTile, currentTile))
                {
                    previousTile = currentTile;
                    currentTile = attemptTile;
                    grid.setStreet(currentTile.X, currentTile.Y);

                    if (TorreroConstants.CrossingProbability * difficulty > random.NextDouble())
                    {
                        yield return currentTile;
                    }
                }

            }

            previousExitsXPositions.Add(currentTile.X);
        }

        /// <summary>
        /// Creates a side path that can, but doesnt have to reach the main path again.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="start"></param>
        private void createSidePath(GameGrid grid, Tile start, int blockSize, float difficulty)
        {
            Tile currentTile = start;
            Tile previousTile = null;
            Tile attemptTile = null;

            while (currentTile.Y < start.Y + blockSize && TorreroConstants.ContinueSidePathProbability * difficulty > random.NextDouble())
            {
                if (previousTile == null || TorreroConstants.TurnProbability * difficulty > random.NextDouble())
                {
                    attemptTile = grid.getRandomFreeTile(currentTile.X, currentTile.Y);
                }
                else
                {
                    attemptTile = grid.getNextTileInDirection(currentTile, previousTile);
                }

                if (attemptTile == null)
                {
                    break;
                }

                if (isValidPath(grid, attemptTile, currentTile))
                {
                    previousTile = currentTile;
                    currentTile = attemptTile;
                    grid.setStreet(currentTile.X, currentTile.Y);
                }
            }
        }

        private bool isValidPath(GameGrid g, Tile target, Tile previous)
        {
            // Moving up
            if (target.Y > previous.Y)
            {
                return ((g.getNeighborLeft(target.X, target.Y) == null || g.getNeighborLeft(target.X, target.Y) is Obstacle )
                    && (g.getNeighborRight(target.X, target.Y) == null || g.getNeighborRight(target.X, target.Y) is Obstacle));
            }
            // Must be moving left or right
            else
            {
                return (target.X > 0 && target.X < TorreroConstants.ColumnNumber - 1) &&
                    (g.getNeighborBelow(target.X, target.Y) is Obstacle
                   && (g.getNeighborAbove(target.X, target.Y) == null || g.getNeighborAbove(target.X, target.Y) is Obstacle)
                   && (g.getNextTileInDirection(target, previous) == null || g.getNextTileInDirection(target, previous) is Obstacle));
            }
        }

        #endregion
    }
}
