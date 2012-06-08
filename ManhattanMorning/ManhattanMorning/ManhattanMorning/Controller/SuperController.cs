using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using ManhattanMorning.Misc;
using ManhattanMorning.Model;
using ManhattanMorning.View;
using ManhattanMorning.Controller.AI;

using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model.Menu;
using ManhattanMorning.Model.HUD;
using ManhattanMorning.Misc.Logic;
using ManhattanMorning.Misc.Levels;
using ManhattanMorning.Misc.Tasks;

namespace ManhattanMorning.Controller
{

    #region enums

    /// <summary>
    /// Enum which defines position in program logic
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Program is in the main menu
        /// </summary>
        MainMenu,
        /// <summary>
        /// The ingame menu is open
        /// </summary>
        IngameMenu,
        /// <summary>
        /// A gameInstance is running
        /// </summary>
        Ingame
    }

    #endregion

    /// <summary>
    /// Controller that controls the program flow
    /// it has one MenuInstance and one GameInstance and keeps
    /// track in which state the game is
    /// </summary>
    class SuperController
    {
        
        #region Properties

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static SuperController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SuperController();
                }
                return instance;
            }
        }
        
        /// <summary>
        /// Instance for the main menu
        /// </summary>
        public MainMenuInstance MainMenuInstance
        {
            get
            {
                if (mainMenuInstance == null)
                {
                    Logger.Instance.log(Sender.Game1, "No menu loaded yet", PriorityLevel.Priority_5);
                    throw new Exception("No menu loaded yet");
                }
                else
                {
                    return mainMenuInstance;
                }
            }
            set { this.mainMenuInstance = value; }

        }

        /// <summary>
        /// Instance for the ingame menu
        /// </summary>
        public IngameMenuInstance IngameMenuInstance
        {
            get
            {
                if (ingameMenuInstance == null)
                {
                    Logger.Instance.log(Sender.Game1, "No ingame menu loaded yet", PriorityLevel.Priority_5);
                    throw new Exception("No ingame menu loaded yet");
                }
                else
                {
                    return ingameMenuInstance;
                }
            }
            set { this.ingameMenuInstance = value; }
        }

        /// Game1 always has 1 GameInstance and 1 MenuInstance
        /// This is the instance for the current game
        public GameInstance GameInstance
        {
            get { return gameInstance; }
        }

        /// <summary>
        /// Game state which stores the state in the whole game
        /// </summary>
        public GameState GameState
        {
            get { return gameState; }
        }

        #endregion

        #region Members

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        private static SuperController instance;

        /// <summary>
        /// Game state which stores the state in the whole game
        /// </summary>
        private GameState gameState;

        /// <summary>
        /// Game1 always has 1 GameInstance and 1 MenuInstance
        /// This is the instance for the current game
        /// </summary>
        private GameInstance gameInstance;

        /// <summary>
        /// The main menu instance
        /// </summary>
        private MainMenuInstance mainMenuInstance;

        /// <summary>
        /// The ingame menu instance
        /// </summary>
        private IngameMenuInstance ingameMenuInstance;

        /// <summary>
        /// Includes all the implemented levels
        /// </summary>
        private Levels levels;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes SuperController
        /// hidden because of Singleton Pattern
        /// </summary>
        private SuperController()
        {

        }

        /// <summary>
        /// Initializes the SuperController
        /// </summary>
        public void initialize()
        {

            // load textures for runtime use
            StorageManager.Instance.loadTexturesForDictionary();

            // Create the levels instance
            levels = new Levels();

            // Start necessary controllers
            TaskManager.Instance.pause(false);
            AnimationManager.Instance.pause(false);

            // Create the menu instance
            mainMenuInstance = new MainMenuInstance(levels);
            ingameMenuInstance = new IngameMenuInstance();

            //init intro video
            Game1.Instance.Video = mainMenuInstance.IntroVideo;
        }

        #endregion

        #region Methods

        #region Main Methods

        /// <summary>
        /// Allows the game to run all the logic in the controllers
        /// </summary>
        /// <param name="gameTime">time since last update (in ms)</param>
        public void Update(GameTime gameTime)
        {

            // Controllers which always have to be updated
            if (gameState == GameState.Ingame)
                InputManager.Instance.updateIngameInput(gameTime, gameInstance.GameObjects.GetPlayer());
            else if (gameState == GameState.MainMenu)
            {
                InputManager.Instance.updateMainMenuInput(gameTime);
                mainMenuInstance.update(gameTime, InputManager.Instance.GamepadArray);
            }
            else if (gameState == GameState.IngameMenu)
            {
                InputManager.Instance.updateIngameMenuInput(gameTime);
                ingameMenuInstance.update(gameTime);
            }

            InputManager.Instance.update(gameTime);
            TaskManager.Instance.update(gameTime);

            SoundManager.Instance.update(gameTime);
            Graphics.Instance.update(gameTime);


            // Controllers which only have to be updated when ingame
            if (gameState == GameState.Ingame)
            {
                AI.AI.Instance.update(gameInstance.GameObjects.GetBalls(),gameTime);
                Physics.Instance.update(gameTime, gameInstance.GameObjects.GetActiveObjects());
                GameLogic.Instance.update(gameTime, gameInstance.LevelTime, gameInstance.Score, gameInstance.GameObjects.GetBalls());
                gameInstance.SpecialbarInstance.update(gameTime);
                AnimationManager.Instance.update(gameTime, gameInstance.GameObjects);
                ParticleSystemsManager.Instance.update(gameTime);
                PowerUpManager.Instance.update(gameTime,gameInstance.GameObjects, gameInstance.ActivePowerUps);
                
            }

            // Controllers which have to be updated when in MainMenu
            if (gameState == GameState.MainMenu)
            {
                AnimationManager.Instance.update(gameTime, mainMenuInstance.MenuObjectList);
            }

            // Controllers which have to be updated when in IngameMenu
            if (gameState == GameState.IngameMenu)
            {
                AnimationManager.Instance.update(gameTime, ingameMenuInstance.MenuObjectList);

            }

        }

        /// <summary>
        /// Allows the game to draw all the objects
        /// </summary>
        /// <param name="gameTime">time since last update (in ms)</param>
        public void Draw(GameTime gameTime)
        {
            if (gameState == GameState.MainMenu)
            {
                // if program is in MainMenu
                Graphics.Instance.drawMenu(gameTime, mainMenuInstance.MenuObjectList);
            }
            else if (gameState == GameState.Ingame)
            {
                if (Game1.VideoPlaying)
                {
                    Graphics.Instance.drawVideo(gameTime);
                    return;
                }
                else
                {
                    // if program is Ingame
                    Graphics.Instance.drawGame(gameTime, gameInstance.GameObjects);
                }
            }
            else
            {
                // if program is IngameMenu
                Graphics.Instance.drawGame(gameTime, gameInstance.GameObjects);
                Graphics.Instance.drawMenu(gameTime, ingameMenuInstance.MenuObjectList);
            }

        }

        #endregion

        #region SwitchStates

        /// <summary>
        /// Manages transition from MainMenu to Ingame
        /// </summary>
        public void switchFromMainMenuToIngame(String levelName, WinCondition winCondition, List<PlayerRepresentationMainMenu> leftTeam, List<PlayerRepresentationMainMenu> rightTeam)
        {
            Graphics.Instance.fadeColor(Color.Black, Color.White, 1600, false);

            // Remove all menu sounds
            SoundManager.Instance.discardMenuSounds();
            LoadLevel(levelName, winCondition, leftTeam, rightTeam);

            // Fade
            SoundManager.Instance.completeyFadeOutIn();

            // Change gamestate
            gameState = GameState.Ingame;

            pauseControllers(false);

            //play video
            if ((bool)SettingsManager.Instance.get("LevelVideo"))
            {
                if (levelName == "Beach")
                {
                    Game1.playVideo(gameInstance.BeachVideo);
                }
                else if (levelName == "Forest")
                {
                    Game1.playVideo(gameInstance.ForestVideo);
                }
                else if (levelName == "Maya")
                {
                    Game1.playVideo(gameInstance.MayaVideo);
                }
                else
                {
                    // Missing video!
                }
            }
        }

        /// <summary>
        /// Manages transition from Ingame to IngameMenu
        /// </summary>
        public void switchFromIngameToIngameMenu(int winner)
        {

            // Pause ingame sounds
            SoundManager.Instance.pauseIngameSounds(true);

            // Fade
            SoundManager.Instance.completeyFadeOutIn();

            // Change gamestate
            gameState = GameState.IngameMenu;

            // Display menu
            ingameMenuInstance.activateMenu(winner);

            // Pause controller
            pauseControllers(true);
        }

        /// <summary>
        /// Manages transition from IngameMenu to Ingame
        /// </summary>
        public void switchFromIngameMenuToIngame()
        {

            // Resume ingame sounds
            SoundManager.Instance.pauseIngameSounds(false);

            // Remove all menu sounds
            SoundManager.Instance.discardMenuSounds();

            // Fade
            SoundManager.Instance.completeyFadeOutIn();

            // Change gamestate
            gameState = GameState.Ingame;            

            // Reactivate controller
            pauseControllers(false);

        }

        /// <summary>
        /// Manages transition from IngameMenu to MainMenu
        /// </summary>
        public void switchFromIngameMenuToMainMenu()
        {

            // Remove all sounds
            SoundManager.Instance.discardMenuSounds();
            SoundManager.Instance.discardIngameSounds();

            // Fade
            SoundManager.Instance.completeyFadeOutIn();

            // Change gamestate
            gameState = GameState.MainMenu;

            pauseControllers(true);
            clearControllers();
            gameInstance = null;

            mainMenuInstance.activateMenu();
        }

        /// <summary>
        /// restarts the game when in ingame menu and pressing
        /// revanche or restart
        /// </summary>
        public void restartGame()
        {

            // Remove all sounds
            SoundManager.Instance.discardMenuSounds();
            SoundManager.Instance.discardIngameSounds();

            clearControllers();

            LoadLevel();
        
            // Change gamestate
            gameState = GameState.Ingame;

            pauseControllers(false);

        }

        /// <summary>
        /// Reloads the level with the parameters from the last GameInstance
        /// that still has to be loaded
        /// </summary>
        private void LoadLevel()
        {

            // load the level with these parameters
            LoadLevel(gameInstance.LevelName, gameInstance.WinCondition, gameInstance.PlayerTeam1, gameInstance.PlayerTeam2);

        }

        /// <summary>
        /// Loads a level with the given parameters and initializes the controllers
        /// (but they're still paused)
        /// </summary>
        /// <param name="levelName">The name of the level</param>
        /// <param name="winCondition">The win condition of the level</param>
        /// <param name="leftTeam">All the playerrepresentations of the left team</param>
        /// <param name="rightTeam">All the playerrepresentations of the right team</param>
        private void LoadLevel(String levelName, WinCondition winCondition, List<PlayerRepresentationMainMenu> leftTeam, List<PlayerRepresentationMainMenu> rightTeam)
        {

            this.gameInstance = new Misc.GameInstance(levelName, winCondition, leftTeam, rightTeam);

            // If it's a 2vs2 game, resize levelSize
            float resizeFactor = 1.0f;
            if ((leftTeam.Count > 1) || (rightTeam.Count > 1))
                resizeFactor = (float)SettingsManager.Instance.get("ResizeFactor2vs2Game");
            StorageManager.Instance.loadStaticLevelData(levels.ImplementedLevels()[levelName], gameInstance, resizeFactor);

            // Create world because objects are loaded into physics world, too
            Physics.Instance.initialize(gameInstance.LevelSize);

            // load levelobjects
            StorageManager.Instance.loadLevelObjects(levels.ImplementedLevels()[levelName], gameInstance, leftTeam, rightTeam, resizeFactor);
            Physics.Instance.SetBorders((Border)SuperController.Instance.getObjectByName("middleBorder"),
                                        (Border)SuperController.Instance.getObjectByName("leftSideNetHandBorder"),
                                        (Border)SuperController.Instance.getObjectByName("rightSideNetHandBorder"),
                                        (Border)SuperController.Instance.getObjectByName("rightSideHandDistanceBorder"),
                                        (Border)SuperController.Instance.getObjectByName("leftSideHandDistanceBorder"));

            // load HUD
            StorageManager.Instance.LoadScoreboard(winCondition);
            LayerList<HUD> tempList = StorageManager.Instance.LoadSpecialbar(gameInstance.LevelSize);
            tempList.Add((HUD)gameInstance.GameObjects.GetObjectByName("Scoreboard"));
            gameInstance.SpecialbarInstance = new Specialbar(tempList);

            // Set time and score
            if (winCondition is TimeLimit_WinCondition)
            {
                gameInstance.LevelTime = ((TimeLimit_WinCondition)winCondition).TimeLimit;
            }
            else
            {
                gameInstance.LevelTime = new TimeSpan(0, 0, 0);
            }
            gameInstance.Score = Vector2.Zero;

            // task that creates ball after some specified time
            GameLogic.Instance.createBall(gameInstance.BallResetPosition, 1000);

            // Create a label above each player that is shown in the first settings so that everbody knows which his player is
            foreach (Player player in getAllPlayers())
            {
                // Define the position in relation to the player
                Vector2 offset = new Vector2(0, - 0.8f * player.Size.Y);

                // Create the label
                PassiveObject playerLabel = new PassiveObject("PlayerLabel", true, gameInstance.getPlayerRepresentation(player.PlayerIndex).PlayerPicture.Texture, null, null,
                    new Vector2(0.5f * player.Size.X, 0.5f * player.Size.Y), Vector2.Zero, 60, MeasurementUnit.Meter);

                playerLabel.Offset = offset;

                // Add a fading animation
                playerLabel.FadingAnimation = new FadingAnimation(false, true, 4000, true, 500);

                // Attach the label to the player and add it to the gameObjects
                player.attachObject(playerLabel);
                addGameObjectToGameInstance(playerLabel);

            }

            TaskManager.Instance.addTask(new SoundTask(2000, SoundIndicator.startWhistle, (int)IngameSound.StartWhistle));

            // set meterToPixel factor in settingsManager
            SettingsManager.Instance.set("meterToPixel", Game1.Instance.GraphicsDevice.Viewport.Width / gameInstance.LevelSize.X);

            // Initialize all controllers

            InputManager.Instance.initialize(gameInstance.GameObjects.GetPlayer());
            TaskManager.Instance.initialize();

            LayerList<HUD> relevantHUDObjects = new LayerList<HUD>();
            if (winCondition is TimeLimit_WinCondition)
            {
                relevantHUDObjects.Add((HUD)gameInstance.GameObjects.GetObjectByName("Clock_highlightedRightHalf"));
                relevantHUDObjects.Add((HUD)gameInstance.GameObjects.GetObjectByName("Clock_highlightedHalf"));
                relevantHUDObjects.Add((HUD)gameInstance.GameObjects.GetObjectByName("Clock_unhighlightedHalf"));
            }
            else
            {
                relevantHUDObjects.Add((HUD)gameInstance.GameObjects.GetObjectByName("ScoreRect_highlighted"));
            }

            GameLogic.Instance.initialize(winCondition, gameInstance.LevelSize, gameInstance.BallResetPosition,
                relevantHUDObjects, (Net)gameInstance.GameObjects.GetObjectByName("net"));

            ParticleSystemsManager.Instance.initialize();
            AI.AI.Instance.initialize(gameInstance.GameObjects.GetPlayer(), gameInstance.LevelSize);
            AnimationManager.Instance.initialize();
            PowerUpManager.Instance.initialize();

        }

        #endregion

        #region Pause/Reset game

        /// <summary>
        /// Resets all relevant controllers
        /// </summary>
        private void clearControllers()
        {
            AnimationManager.Instance.clear();
            GameLogic.Instance.clear();
            AI.AI.Instance.clear();
            InputManager.Instance.clear();
            ParticleSystemsManager.Instance.clear();
            Physics.Instance.clear();
            PowerUpManager.Instance.clear();
            TaskManager.Instance.clear();
            Graphics.Instance.reset();
        }

        /// <summary>
        /// Pauses all relevant controllers (e.g. when open ingame menu)
        /// </summary>
        /// <param name="on">if true, controllers pause, else not</param>
        public void pauseControllers(bool on)
        {
            if (on)
            {
                //AnimationManager.Instance.pause(true);
                GameLogic.Instance.pause(true);
                AI.AI.Instance.pause(true);
                ParticleSystemsManager.Instance.pause(true);
                Physics.Instance.pause(true);
                PowerUpManager.Instance.pause(true);
                TaskManager.Instance.pause(true);
                Graphics.Instance.pause(true);
            }
            else
            {
                //AnimationManager.Instance.pause(false);
                GameLogic.Instance.pause(false);
                AI.AI.Instance.pause(false);
                ParticleSystemsManager.Instance.pause(false);
                Physics.Instance.pause(false);
                PowerUpManager.Instance.pause(false);
                TaskManager.Instance.pause(false);
                Graphics.Instance.pause(false);
            }
        }


        /// <summary>
        /// Adds a new GameObject to this LevelInstance.
        /// </summary>
        /// <param name="g"></param>
        public void addGameObjectToGameInstance(LayerInterface g)
        {
            if (g != null)
            {
                gameInstance.GameObjects.Add(g);
            }
        }

        /// <summary>
        /// Removes a gameObject from the list of gameObjects in this level.
        /// </summary>
        /// <param name="l"></param>
        public void removeGameObjectFromGameInstance(LayerInterface l)
        {
            //uncouple link between objects
            if (l is DrawableObject)
            {
                DrawableObject g = l as DrawableObject;
                if (g.AttachedTo != null) g.AttachedTo.uncoupleObject(g);
                foreach (LayerInterface d in g.AttachedObjects)
                {
                    g.AttachedTo = null;
                }

                foreach (DrawableObject d in g.AttachedObjects)
                {
                    gameInstance.GameObjects.remove(d);
                }
            }            

            gameInstance.GameObjects.remove(l);
            if (l is ActiveObject)
            {
                Physics.Instance.removeObjectFromPhysicSimulation((ActiveObject)l);
            }
            l = null;
        }

        #endregion

        #region HUD

        /// <summary>
        /// Increases/Decreases the specialbar filling
        /// </summary>
        /// <param name="team">1:left team, 2:right team</param>
        /// <param name="amount">the amount it should be changed</param>
        public void setSpecialbarFilling(int team, float amount)
        {

            Vector2 changeOffset;
            if (team == 1)
                changeOffset = new Vector2(amount, 0);
            else
                changeOffset = new Vector2(0, amount);

            gameInstance.SpecialbarInstance.processNewSpecialbarFilling(changeOffset);

        }

        /// <summary>
        /// Returns the necessary HUD element
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <returns>The HUD Element</returns>
        public HUD getHUDElementByName(String name)
        {
            return (gameInstance.GameObjects.GetObjectByName(name) is HUD) ? gameInstance.GameObjects.GetObjectByName(name) as HUD : null;
        }

        #endregion

        #region Powerups

        /// <summary>
        /// Spawns a positive powerup in the left or in the right half of the level
        /// </summary>
        /// <param name="team">1 = left half, 2 = right half</param>
        public void SpawnPositivePowerup(int team)
        {

            Vector2 newPosition;

            // Define a position or left team
            if (team == 1)
                newPosition = new Vector2((float)(new Random()).Next(0, ((int)gameInstance.LevelSize.X / 2) - 1), -1);
            // Define a position for right team
            else
                newPosition = new Vector2((float)(new Random()).Next((((int)gameInstance.LevelSize.X / 2) + 1), 
                    ((int)gameInstance.LevelSize.X - 1)), -1);

            // Spawn powerup
            PowerUpManager.Instance.createRandomPowerUp(newPosition, PowerUpVersion.Positive);

        }


        #endregion

        #region Set GameInstance parameters

        /// <summary>
        /// Increases the GameInstance's Score for the left team
        /// </summary>
        public void increaseLeftScore()
        {
            gameInstance.Score = new Vector2(gameInstance.Score.X + 1, gameInstance.Score.Y);
        }

        /// <summary>
        /// Increases the GameInstance's Score for the right team
        /// </summary>
        public void increaseRightScore()
        {
            gameInstance.Score = new Vector2(gameInstance.Score.X, gameInstance.Score.Y + 1);
        }

        /// <summary>
        /// Sets the leveltime to a new timespan
        /// </summary>
        /// <param name="levelTime">The new time value</param>
        public void setLevelTime(TimeSpan levelTime)
        {
            gameInstance.LevelTime = levelTime;
        }

        #endregion

        #region Get game objects

        /// <summary>
        /// returns a list of players of the defined team
        /// </summary>
        /// <param name="team">1: left team, 2: right team</param>
        /// <returns>the player list</returns>
        public List<Player> getPlayerOfTeam(int team)
        {

            List<Player> returnList = new List<Player>();

            if (team == 1)
            {
                foreach (PlayerRepresentationMainMenu playerRepresentation in gameInstance.PlayerTeam1)
                    returnList.Add(gameInstance.GameObjects.GetPlayer(playerRepresentation.PlayerIndex));
            }
            else
            {
                foreach (PlayerRepresentationMainMenu playerRepresentation in gameInstance.PlayerTeam2)
                    returnList.Add(gameInstance.GameObjects.GetPlayer(playerRepresentation.PlayerIndex));
            }

            return returnList;
        }

        /// <summary>
        /// Gets all players.
        /// </summary>
        /// <returns>List of Players.</returns>
        public List<Player> getAllPlayers()
        {
            return gameInstance.GameObjects.GetPlayer();
        }

        /// <summary>
        /// Goes through all available balls and returns the one that
        /// has the smallest distance to the given position
        /// </summary>
        /// <param name="position">The position to which every ball is compared</param>
        /// <returns>the nearest ball or null if there is no ball right now</returns>
        public Ball getNearestBall(Vector2 position)
        {
            // return null if there is no ball right now
            if (gameInstance.GameObjects.GetBalls().Count == 0)
                return null;

            // Take the first ball
            Ball returnBall = (Ball)gameInstance.GameObjects.GetBalls()[0];

            // Compare any other ball
            for (int i = 1; i < gameInstance.GameObjects.GetBalls().Count; i++)
            {
                float oldDistance = Vector2.Distance(returnBall.Position, position);
                float newDistance = Vector2.Distance(gameInstance.GameObjects.GetBalls()[i].Position, position);

                // If the distance is smaller, save the latest ball as return ball
                if (newDistance < oldDistance)
                    returnBall = gameInstance.GameObjects.GetBalls()[i];
            }

            return returnBall;
        }

        /// <summary>
        /// Returns all Balls in the game.
        /// </summary>
        /// <returns></returns>
        public List<Ball> getAllBalls()
        {
            return gameInstance.GameObjects.GetBalls();
        }

        /// <summary>
        /// Returns all Powerups in the level.
        /// </summary>
        /// <value>
        /// a List of all Powerups in the level.
        /// </value>
        public List<PowerUp> getAllPowerups()
        {
            return gameInstance.GameObjects.GetPowerup();
        }

        /// <summary>
        /// Returns the first item in the List which name equals the given String.
        /// </summary>
        /// <param name="name">Name of the Object.</param>
        /// <returns>The object with the specified name.</returns>
        public LayerInterface getObjectByName(String name)
        {
            return gameInstance.GameObjects.GetObjectByName(name);
        }

        /// <summary>
        /// Returns all Lights in the List which name equals the given String.
        /// </summary>
        /// <param name="name">Name of the Lights.</param>
        /// <returns>The object with the specified name.</returns>
        public List<Light> getLightsByName(String name)
        {
            return gameInstance.GameObjects.GetLightsByName(name);
        }

        #endregion

        #endregion

    }
}
