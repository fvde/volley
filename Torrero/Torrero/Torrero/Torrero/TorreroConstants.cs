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

namespace Torrero
{
    public class TorreroConstants
    {

        /// <summary>
        /// The maximum distance between player and nextTile position. If the difference is smaller, the players position will snap to the tiles position.
        /// The movingTreshhold is also used when checking if a GameObject is completely on a Tile.
        /// </summary>
        public const int movingTreshhold = 4;

        /// <summary>
        /// The Viewport heigth minus the current Tilesize. Used for drawing conversions.
        /// </summary>
        public const float ViewportHeightWithTile = 800 - TileSize;

        /// <summary>
        /// Size of all tiles ingame.
        /// </summary>
        public const int TileSize = 60;

        /// <summary>
        /// Half size of all tiles ingame.
        /// </summary>
        public const int TileSizeHalf = TileSize / 2;

        /// <summary>
        /// Number of rows that the grid must contain.
        /// </summary>
        public const int RowNumber = 16;

        /// <summary>
        /// Number of columns: ATTENTION the first and the last are just for visual effects and to give the player room to maneuver!
        /// </summary>
        public const int ColumnNumber = 9;

        /// <summary>
        /// Minimum of rows that are created when calculating a new block
        /// </summary>
        public const int MinimumBlockSize = 2;

        /// <summary>
        /// Maximum of rows that are created when calculating a new block
        /// </summary>
        public const int MaximumBlockSize = 4;

        /// <summary>
        /// Horizontal maximum speed of the player.
        /// </summary>
        public const float HorizontalMaximumSpeed = 250;

        /// <summary>
        /// Vertical maximum speed of the player.
        /// </summary>
        public const float VerticalMaximumSpeed = 250f;

        /// <summary>
        /// Horizontal maximum speed of the player.
        /// </summary>
        public const float HorizontalSpeed = 120.0f;

        /// <summary>
        /// Vertical speed of the player.
        /// </summary>
        public const float VerticalSpeed = 120.0f;

        /// <summary>
        /// When the touch location is closer then this value to the player, the player will move forward.
        /// </summary>
        public const int StraightMovementVariation = 10;

        /// <summary>
        /// Likelyhood of starting a new path.
        /// </summary>
        public static float CrossingProbability = 0.4f;

        /// <summary>
        /// Likelyhood of turning in pathes.
        /// </summary>
        public static float TurnProbability = 0.2f;

        /// <summary>
        /// Likelyhood of continuing a side path
        /// </summary>
        public static float ContinueSidePathProbability = 0.7f;

        /// <summary>
        /// Every interval the difficulty is increased by (1-Probability) * DifficultyGrowthFactor for all probabilities and directly for all speedsss.
        /// </summary>
        public const float DifficultyGrowthFactor = 0.05f;

        /// <summary>
        /// Determines how often the difficulty is increased (ms)
        /// </summary>
        public const int DifficultyInterval = 3000;
    }
}
