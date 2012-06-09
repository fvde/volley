using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Controller;
using ManhattanMorning.Controller.AI;
using ManhattanMorning.View;
using ManhattanMorning.Model;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model.ParticleSystem;
using ManhattanMorning.Model.HUD;
using ManhattanMorning.Misc.Logic;
using ManhattanMorning.Misc.Tasks;

using FarseerPhysics.Dynamics;


namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Represents the actual game
    /// </summary>
    public class GameInstance
    {

        #region Properties

        /// <summary>
        /// The name of the level
        /// </summary>
        public String LevelName { get { return levelName; } set { levelName = value; } }

        /// <summary>
        /// Score for left (X-Coordinate) and right (Y-Coordinate) team
        /// </summary>
        public Vector2 Score { 
            get { return score; } 
            set {

                score = value;

                // set digits in HUD

                #region left score

                HUD tempDigit = (HUD)gameObjects.GetObjectByName("Digit_left_1");
                int tempNumber = (int)(score.X / 10);
                switch (tempNumber)
                {
                    case 0:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_0");
                        break;
                    case 1:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_1");
                        break;
                    case 2:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_2");
                        break;
                    case 3:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_3");
                        break;
                    case 4:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_4");
                        break;
                    case 5:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_5");
                        break;
                    case 6:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_6");
                        break;
                    case 7:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_7");
                        break;
                    case 8:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_8");
                        break;
                    case 9:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_9");
                        break;
                }

                tempDigit = (HUD)gameObjects.GetObjectByName("Digit_left_2");
                tempNumber = (int)(score.X % 10);
                switch (tempNumber)
                {
                    case 0:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_0");
                        break;
                    case 1:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_1");
                        break;
                    case 2:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_2");
                        break;
                    case 3:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_3");
                        break;
                    case 4:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_4");
                        break;
                    case 5:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_5");
                        break;
                    case 6:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_6");
                        break;
                    case 7:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_7");
                        break;
                    case 8:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_8");
                        break;
                    case 9:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_9");
                        break;
                }

                #endregion

                #region right score

                tempDigit = (HUD)gameObjects.GetObjectByName("Digit_right_1");
                tempNumber = (int)(score.Y / 10);
                switch (tempNumber)
                {
                    case 0:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_0");
                        break;
                    case 1:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_1");
                        break;
                    case 2:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_2");
                        break;
                    case 3:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_3");
                        break;
                    case 4:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_4");
                        break;
                    case 5:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_5");
                        break;
                    case 6:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_6");
                        break;
                    case 7:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_7");
                        break;
                    case 8:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_8");
                        break;
                    case 9:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_9");
                        break;
                }

                tempDigit = (HUD)gameObjects.GetObjectByName("Digit_right_2");
                tempNumber = (int)(score.Y % 10);
                switch (tempNumber)
                {
                    case 0:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_0");
                        break;
                    case 1:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_1");
                        break;
                    case 2:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_2");
                        break;
                    case 3:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_3");
                        break;
                    case 4:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_4");
                        break;
                    case 5:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_5");
                        break;
                    case 6:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_6");
                        break;
                    case 7:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_7");
                        break;
                    case 8:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_8");
                        break;
                    case 9:
                        tempDigit.Texture = StorageManager.Instance.getTextureByName("texture_digit_9");
                        break;
                }



                #endregion

                // adjust size of the specialbars
                specialbarInstance.processNewSpecialbarSize(value);
            } 
        }

        /// <summary>
        /// Size of the level (in meters)
        /// </summary>
        public Vector2 LevelSize { get { return levelSize; } set { levelSize = value; } }
     
        /// <summary>
        /// All the representations of the player which belong to team1
        /// </summary>
        public List<PlayerRepresentationMainMenu> PlayerTeam1 { get { return playerTeam1; } }

        /// <summary>
        /// All the representations of the player which belong to team2
        /// </summary>
        public List<PlayerRepresentationMainMenu> PlayerTeam2 { get { return playerTeam2; } }

        /// <summary>
        /// The spawning position of the ball (in meters)
        /// </summary>
        public Vector2 BallResetPosition { get { return ballResetPosition; } set { ballResetPosition = value; } }

        /// <summary>
        /// The spawning position of team 1 (in meters)
        /// </summary>
        public Vector2 Team1ResetPosition { get { return team1ResetPosition; } set { team1ResetPosition = value; } }

        /// <summary>
        /// The spawning position of team 2 (in meters)
        /// </summary>
        public Vector2 Team2ResetPosition { get { return team2ResetPosition; } set { team2ResetPosition = value; } }

        /// <summary>
        /// List of all game objects
        /// </summary>
        public LayerList<LayerInterface> GameObjects { get { return gameObjects; } }

        /// <summary>
        /// List of all powerups that have been picked up and are active.
        /// </summary>
         public List<PowerUp> ActivePowerUps { get { return activePowerUps; } }

        /// <summary>
        /// List with all particle systems in a level
        /// </summary>
        public List<ParticleSystem> ParticleSystems { get { return particleSystems; } set { particleSystems = value; } }

        /// <summary>
        /// The win condition for the game
        /// </summary>
        public WinCondition WinCondition { get { return winCondition; } set { winCondition = value; } }

        /// <summary>
        /// time in a level, can have 2 possible meanings:
        /// 1. if level has a time limit, it counts down to zero
        /// 2. otherwise it stores the elapsed time in a level
        /// </summary>
        public TimeSpan LevelTime { get { return levelTime; } set { levelTime = value; } }

        /// <summary>
        /// Represents the specialbar in this GameInstance
        /// </summary>
        public Specialbar SpecialbarInstance
        {
            get { return specialbarInstance; }
            set { this.specialbarInstance = value; }
        }

        /// <summary>
        /// The Intro Video.
        /// </summary>
        public Video IntroVideo
        {
            get { return introVideo; }
            set { introVideo = value; }
        }

        /// <summary>
        /// The Intro Video.
        /// </summary>
        public Video BeachVideo
        {
            get { return beachVideo; }
            set { beachVideo = value; }
        }

        /// <summary>
        /// The Intro Video.
        /// </summary>
        public Video ForestVideo
        {
            get { return forestVideo; }
            set { forestVideo = value; }
        }

        /// <summary>
        /// The Intro Video.
        /// </summary>
        public Video MayaVideo
        {
            get { return mayaVideo; }
            set { mayaVideo = value; }
        }

        #endregion


        #region Members

        /// <summary>
        /// The name of the level
        /// </summary>
        private String levelName;

        /// <summary>
        /// Score for left (X-Coordinate) and right (Y-Coordinate) team
        /// </summary>
        private Vector2 score;

        /// <summary>
        /// Size of the level (in meters)
        /// </summary>
        private Vector2 levelSize;

        /// <summary>
        /// The spawning position of the ball
        /// </summary>
        private Vector2 ballResetPosition;

        /// <summary>
        /// The spawning position of team 1
        /// </summary>
        private Vector2 team1ResetPosition;

        /// <summary>
        /// The spawning position of team 2
        /// </summary>
        private Vector2 team2ResetPosition;

        /// <summary>
        /// All the representations of the player which belong to team1
        /// </summary>
        private List<PlayerRepresentationMainMenu> playerTeam1;

        /// <summary>
        /// All the representations of the player which belong to team2
        /// </summary>
        private List<PlayerRepresentationMainMenu> playerTeam2;

        /// <summary>
        /// List of all game objects
        /// </summary>
        private LayerList<LayerInterface> gameObjects;

        /// <summary>
        /// List of all Waterfalls
        /// </summary>
        private List<Waterfall> waterfallList;


        /// <summary>
        /// List of all active Powerups
        /// </summary>
        private List<PowerUp> activePowerUps;

        /// <summary>
        /// List with all particle systems in a level
        /// </summary>
         private List<ParticleSystem> particleSystems;

        /// <summary>
        /// The win condition for this specific instance of the game
        /// </summary>
        private WinCondition winCondition;

        /// <summary>
        /// time in a level, can have 2 possible meanings:
        /// 1. if level has a time limit, it counts down to zero
        /// 2. otherwise it stores the elapsed time in a level
        /// </summary>
        private TimeSpan levelTime;

        /// <summary>
        /// Represents the specialbar in this GameInstance
        /// </summary>
        private Specialbar specialbarInstance;

        /// <summary>
        /// The Intro Video.
        /// </summary>
        private Video introVideo;

        /// <summary>
        /// The Intro Video.
        /// </summary>
        private Video beachVideo;

        /// <summary>
        /// The Intro Video.
        /// </summary>
        private Video forestVideo;

        /// <summary>
        /// The Intro Video.
        /// </summary>
        private Video mayaVideo;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a new game instance with the
        /// according level
        /// </summary>
        /// <param name="levelName">The name of the level</param>
        public GameInstance(String levelName, WinCondition winCondition, List<PlayerRepresentationMainMenu> PlayerTeam1, 
            List<PlayerRepresentationMainMenu> PlayerTeam2)
        {

            // set all necessary variables
            this.winCondition = winCondition;
            this.levelName = levelName;

            this.playerTeam1 = PlayerTeam1;
            this.playerTeam2 = PlayerTeam2;

            activePowerUps = new List<PowerUp>();
            gameObjects = new LayerList<LayerInterface>();
            waterfallList = new List<Waterfall>();

        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the PlayerRepresentation for an index
        /// </summary>
        /// <param name="index">The index of the PlayerRepresentation</param>
        /// <returns>The PlayerRepresentation</returns>
        public PlayerRepresentationMainMenu getPlayerRepresentation(int index)
        {

            foreach (PlayerRepresentationMainMenu player in playerTeam1)
                if (player.PlayerIndex == index)
                    return player;

            foreach (PlayerRepresentationMainMenu player in playerTeam2)
                if (player.PlayerIndex == index)
                    return player;


            return null;
        }

        #endregion

    }
}
