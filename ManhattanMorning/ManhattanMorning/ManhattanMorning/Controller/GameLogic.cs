using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;
using ManhattanMorning.Misc.Logic;
using ManhattanMorning.Misc.Tasks;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model;
using ManhattanMorning.Model.HUD;
using ManhattanMorning.View;
using ManhattanMorning.Misc.Curves;
using FarseerPhysics.Dynamics;


namespace ManhattanMorning.Controller
{
    public enum BonusIndicator
    {
        smash,
        teamplay,
        directPoint,
        pointStreak,
        control
    }

    /// <summary>
    /// Class that works as kind of referee:
    /// - Checks if one team has won
    /// - Checks if one team scores
    /// - Sets also game mechanics like allow player to jump
    /// - Creates Powerups at the right time
    /// ...
    /// </summary>
    public class GameLogic: IController, IObserver
    {

        #region Properties

        /// <summary>
        /// Necessary instance for Singleton Pattern
        /// </summary>
        public static GameLogic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameLogic();
                }
                return instance;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Necessary instance for Singleton Pattern
        /// </summary>
        private static GameLogic instance = null;

        /// <summary>
        /// List that stores all collision between two update() cycles
        /// </summary>
        private List<ActiveObject[]> collisionList;

        /// <summary>
        /// Indicates if GameLogic pauses
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// Creates random values
        /// </summary>
        private Random random;

        /// <summary>
        /// Switch powerup creation on/off
        /// </summary>
        private bool enablePowerups;

        /// <summary>
        /// Defines how many percent the different spawning times can vary from their original value (0 - 1)
        /// </summary>
        private float maxSpreading_spawnTimes;

        /// <summary>
        /// Defines after how much time at an average new neutral Powerups spawn (in ms)
        /// </summary>
        private int averageSpawnTime_neutralPowerup;

        /// <summary>
        /// Time for next spawn of a neutral powerup (in ms)
        /// </summary>
        private int timeForNextSpawn_neutralPowerup;

        /// <summary>
        /// Stores the amount of hits for a team. Additional info is provided by lastPlayerThatHitBall.
        /// </summary>
        private int countBallCollisionsForTeam;

        /// <summary>
        /// Stores which player was the last that hit the ball.
        /// </summary>
        private Player lastPlayerThatHitBall;

        /// <summary>
        /// For testing purposes. Allows adjusting how many points a good or bad action gets.
        /// </summary>
        private float energyMultiplicator;

        /// <summary>
        /// Stores the team who made a point. Used for hot streak energy bonus.
        /// </summary>
        private int lastTeamThatScored;

        /// <summary>
        /// Stores how many points have been made by a single team in a row.
        /// </summary>
        private int pointsInARow;

        /// <summary>
        /// Stores all active powerUp icon in a list to organize them.
        /// </summary>
        private Queue<HUD> powerupDisplay_t1, powerupDisplay_t2;

        /// <summary>
        /// Stores the currently active neutral powerUp icon.
        /// </summary>
        private HUD powerupDisplay_neutral;

        /// <summary>
        /// Meter to pixel factor to calculate object sizes.
        /// </summary>
        private float metertopixel;
        
        /// <summary>
        /// WinCondition of the GameInstance
        /// </summary>
        private WinCondition winCondition;

        /// <summary>
        /// LevelSize of the GameInstance
        /// </summary>
        private Vector2 levelSize;

        /// <summary>
        /// BallResetPosition in the current GameInstance
        /// </summary>
        public Vector2 ballResetPosition;

        /// <summary>
        /// References to all HUD objects which lie in the gameobjects
        /// and have to be updated by the gamelogic
        /// </summary>
        private LayerList<HUD> relevantHUDObjects;

        /// <summary>
        /// Reference to the net which lies in the gameobjects
        /// </summary>
        private Net net;

        /// <summary>
        /// The time since a player hit the ball.
        /// </summary>
        private double timeLastHit;

        /// <summary>
        /// The GameTime.
        /// </summary>
        private GameTime gameTime;

        /// <summary>
        /// Holds the stones which are currently blocking.
        /// </summary>
        private List<int> activeMayaStones = new List<int>();

        /// <summary>
        /// Holds the waterfalls
        /// </summary>
        private List<Waterfall> waterfalls = new List<Waterfall>();

        /// <summary>
        /// The bodies of the stones in maya level.
        /// </summary>
        private Body[] stoneBlocks = new Body[4];

        /// <summary>
        /// Indicates if a countdown texture (1-5) was already displayed
        /// </summary>
        private bool[] activatedCountdowns = new bool[10];
        
        /// <summary>
        /// Indicates if the time expired and the game ist tied ( on TimeLimit_WinningCondition)
        /// </summary>
        private bool matchBallTimeWinningCondition = false;

        /// <summary>
        /// Indicates if the matchballSound is played already, when there is a matchball!
        private bool matchBallSound = false;


        #endregion


        #region Initialization

        /// <summary>
        /// Necessary for interface, but mustn't be called
        /// </summary>
        public void initialize()
        {
            throw new Exception("Wrong InputManager initialization");
        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        /// <param name="winCondition">The winCondition of the new GameInstance</param>
        /// <param name="levelSize">The levelSize of the new GameInstance</param>
        /// <param name="ballResetPosition">The ballResetPosition of the new GameInstance</param>
        /// <param name="relevantHUDObjects">Reference to HUD objects the gameLogic has to update</param>
        /// <param name="net">The net of the new GameInstance</param>
        public void initialize(WinCondition winCondition, Vector2 levelSize, Vector2 ballResetPosition, LayerList<HUD> relevantHUDObjects, Net net)
        {
            // save all parameters
            this.winCondition = winCondition;
            this.levelSize = levelSize;
            this.ballResetPosition = ballResetPosition;
            this.relevantHUDObjects = relevantHUDObjects;
            this.net = net;

            // Initialize all necessary members
            collisionList = new List<ActiveObject[]>();
            random = new Random();
            powerupDisplay_t1 = new Queue<HUD>();
            powerupDisplay_t2 = new Queue<HUD>();

            matchBallTimeWinningCondition = false;

            //reset energy system
            countBallCollisionsForTeam = 0;
            lastPlayerThatHitBall = null;
            lastTeamThatScored = 0;
            pointsInARow = 0;

            // Load values out of SettingsManager
            SettingsManager.Instance.registerObserver(this);
            enablePowerups = (bool)SettingsManager.Instance.get("enablePowerups");
            maxSpreading_spawnTimes = (float)SettingsManager.Instance.get("maxSpreading_spawnTimes");
            averageSpawnTime_neutralPowerup = (int)SettingsManager.Instance.get("averageSpawnTime_neutralPowerup");
            energyMultiplicator = (float)SettingsManager.Instance.get("PositivePowerupsMultiplier"); 
            metertopixel = (float)SettingsManager.Instance.get("meterToPixel");

            // Set first spawn times for powerups
            timeForNextSpawn_neutralPowerup = random.Next((int)(averageSpawnTime_neutralPowerup - maxSpreading_spawnTimes * averageSpawnTime_neutralPowerup),
                (int)(averageSpawnTime_neutralPowerup + maxSpreading_spawnTimes * averageSpawnTime_neutralPowerup));

        }

        /// <summary>
        /// Initializes GameLogic
        /// </summary>
        private GameLogic()
        {
            
        }



        #endregion

        #region Methods

        /// <summary>
        /// Main method that is called every cycle
        /// all the logic is handled in this class
        /// </summary>
        /// <param name="gameTime">Time to calculate some functions</param>
        /// <param name="levelTime">Leveltime in the GameInstance</param>
        /// <param name="score">The score of the GameInstance</param>
        /// <param name="ballList">List with all balls that exist at the moment</param>
        public void update(GameTime gameTime, TimeSpan levelTime, Vector2 score, LayerList<Ball> ballList)
        {
            this.gameTime = gameTime;

            //if paused, nothing happens
            if (!isPaused)
            {
                getTasks();

                // update level time
                updateLevelTime(gameTime, levelTime);

                // process all collisions
                processCollisions();

                //checks if new powerups should be spawned
                if (enablePowerups)
                    handlePowerupCreation(gameTime);

                // check win conditions
                checkWinConditions(score, levelTime);

                // update HUD elements
                updateHUD(ballList, levelTime, score);

            }

        }

        /// <summary>
        /// Does all necessary action, to bring controller back to the state after initialization
        /// </summary>
        public void clear()
        {
            // Delete all necessary members
            winCondition = null;
            levelSize = Vector2.Zero;
            ballResetPosition = Vector2.Zero;
            relevantHUDObjects = null;
            net = null;

            powerupDisplay_t1 = null;
            powerupDisplay_t2 = null;
            powerupDisplay_neutral = null;

            timeLastHit = 0f;
            lastPlayerThatHitBall = null;
            countBallCollisionsForTeam = 0;
            lastTeamThatScored = 0;
            pointsInARow = 0;

            activeMayaStones.Clear();
            stoneBlocks.Initialize();

            activatedCountdowns = new bool[10];

        }


        /// <summary>
        /// Forces controller to pause, has to make sure that all timers etc. pause
        /// </summary>
        /// <param name="on">true: controller has to pause, false: controller has to work </param>
        public void pause(bool on)
        {
            isPaused = on;
        }

        /// <summary>
        /// Adds a collision to the list which will be processed in every loop
        /// </summary>
        /// <param name="firstObject">First object that collided</param>
        /// <param name="secondObject">The second object that was part of the collision</param>
        public void addCollision(ActiveObject firstObject, ActiveObject secondObject)
        {

            // if not paused
            if (!isPaused)
            {
                // save collisions in list so that they be processed in next update()
                collisionList.Add(new ActiveObject[2] { firstObject, secondObject });
            }

        }


        #region HelperMethods

        /// <summary>
        /// Updates the leveltime in a game instance
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        /// <param name="levelTime">The levelTime of the GameInstance</param>
        private void updateLevelTime(GameTime gameTime, TimeSpan levelTime)
        {

                // if win condition is a time limit, count down
                if (winCondition is TimeLimit_WinCondition)
                {
                    SuperController.Instance.setLevelTime(levelTime.Subtract(gameTime.ElapsedGameTime));
                }
                // otherwise count up
                else
                {
                    SuperController.Instance.setLevelTime(levelTime.Add(gameTime.ElapsedGameTime));
                }
            
        }

        /// <summary>
        /// Checks if a win condition is triggered
        /// </summary>
        /// <param name="score">The score of the GameInstance</param>
        /// <param name="levelTime">The level time of the GameInstance</param>
        private void checkWinConditions(Vector2 score, TimeSpan levelTime)
        {

            // if it's a ScoreLime_WinCondition
            if (winCondition is ScoreLimit_WinCondition)
            {
                if (score.X == ((ScoreLimit_WinCondition)winCondition).WinningScore - 1 ||
                    score.Y == ((ScoreLimit_WinCondition)winCondition).WinningScore - 1)
                {
                    activateMatchballFlag(score);
                }

                // check if one of both teams has reached the score limit
                // left team
                if (score.X >=
                    ((ScoreLimit_WinCondition)winCondition).WinningScore)
                {
                    // left team wins
                    levelEnds(1);
                }
                // right team
                if (score.Y >=
                    ((ScoreLimit_WinCondition)winCondition).WinningScore)
                {
                    // right team wins
                    levelEnds(2);
                }

            }

            // if it's a TimeLimit_WinCondition
            else if (winCondition is TimeLimit_WinCondition)
            {
                // check if time has reached zero
                if (levelTime.TotalMilliseconds < 0)
                {

                    // Check which team has a higher score
                    if (score.X > score.Y)
                    {
                        // left team wins
                        levelEnds(1);
                    }
                    else if (score.X < score.Y)
                    {
                        // right team wins
                        levelEnds(2);
                    }
                    else
                    {
                        activateMatchballFlag(score);
                        this.winCondition = new ScoreLimit_WinCondition((int)score.Y + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Updates all necessary HUD elements
        /// </summary>
        /// <param name="ballList">List with all balls</param>
        /// <param name="levelTime">The levelTime of the GameInstance</param>
        /// <param name="score">The score of the GameInstance</param>
        private void updateHUD(LayerList<Ball> ballList, TimeSpan levelTime, Vector2 score)
        {

            #region Ballindicator

            if (ballList.Count > 0)
            {
                foreach (Ball ball in ballList)
                {
                    // Define size of BallIndicator the higher the ball the smaller the indicator
                    ball.BallIndicator.Size = Graphics.Instance.convertUnits(ball.Size, MeasurementUnit.Meter, MeasurementUnit.PercentOfScreen) *
                        (1f / MathHelper.Clamp(1f, (-0.5f * ball.Position.Y), 100));
                    ball.BallIndicator.Size = new Vector2(ball.BallIndicator.Size.X, 0.02f);

                    // We need the level size to convert from Ball's meters to HUD's percent

                    // Calculate offset ball-ballindicator
                    float xSizeBallIndicator = ball.Size.X * (1f / MathHelper.Clamp(1f, (-1f * ball.Position.Y), 100));
                    float xOffset = (ball.Size.X - xSizeBallIndicator) / 2f;

                    // Calculate position according to the camera
                    Vector2 transformedBallPosition = new Vector2(ball.Position.X + xOffset, ball.Position.Y) * metertopixel;

                    transformedBallPosition.X = transformedBallPosition.X - Graphics.Instance.Camera.ViewPortWidth + Graphics.Instance.Camera.Position.X;
                    transformedBallPosition.Y = transformedBallPosition.Y - Graphics.Instance.Camera.ViewPortHeight + Graphics.Instance.Camera.Position.Y;

                    transformedBallPosition = transformedBallPosition * Graphics.Instance.Camera.Zoom;

                    transformedBallPosition.X = transformedBallPosition.X + Graphics.Instance.Camera.ViewPortWidth - Graphics.Instance.Camera.Position.X;
                    transformedBallPosition.Y = transformedBallPosition.Y + Graphics.Instance.Camera.ViewPortHeight - Graphics.Instance.Camera.Position.Y;

                    // check if ball isn't visible
                    if (transformedBallPosition.Y < 0)
                    {
                        ball.BallIndicator.Visible = true;

                        // set position
                        ball.BallIndicator.Position = new Vector2(Graphics.Instance.convertUnits(transformedBallPosition,
                            MeasurementUnit.Pixel, MeasurementUnit.PercentOfScreen).X, 0);

                    }
                    else
                    {
                        // ball is visible at the moment, so no  ballIndicator is necessary
                        ball.BallIndicator.Visible = false;
                    }
                }

            }

            #endregion

            #region Clock & Score Rect

            if (this.matchBallTimeWinningCondition)
            {
                return;
            }

            if (winCondition is TimeLimit_WinCondition)
            {
                // CLOCK

                if (levelTime.TotalMilliseconds > (((TimeLimit_WinCondition)winCondition).TimeLimit.TotalMilliseconds / 2))
                {
                    // clock is filled less than half
                    ((HUD)relevantHUDObjects.GetObjectByName("Clock_highlightedRightHalf")).Visible = false;
                    ((HUD)relevantHUDObjects.GetObjectByName("Clock_unhighlightedHalf")).Visible = true;
                }
                else
                {
                    // clock is filled more than half
                    ((HUD)relevantHUDObjects.GetObjectByName("Clock_highlightedRightHalf")).Visible = true;
                    ((HUD)relevantHUDObjects.GetObjectByName("Clock_unhighlightedHalf")).Visible = false;
                }

                // calculate rotation
                float rotation = (float)(-2 * Math.PI);
                rotation *= (float)(levelTime.TotalMilliseconds / (((TimeLimit_WinCondition)winCondition).TimeLimit.TotalMilliseconds));

                ((HUD)relevantHUDObjects.GetObjectByName("Clock_highlightedHalf")).Rotation = rotation;

                

                // Display countdown for the last 10 seconds
                for (int i = 10; i > 0; i--)
                {
                    if ((levelTime.TotalMilliseconds < i*1000) && (activatedCountdowns[i-1] == false))
                    {

                        if (i == 10)
                        {
                            TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.countdown, (int)IngameSound.Countdown));
                        }

                        FadingAnimation fadingAnimation = new FadingAnimation(false, false, 0, true, 900, Model.ParticleSystem.FadeMode.Quadratic);
                        fadingAnimation.Inverted = true;
                        HUD countdownTexture = new HUD("CountdownTexture", true, StorageManager.Instance.getTextureByName("texture_countdown_" + i), null, new Vector2(0.06f, 0.06f * 1.7778f),
                            new Vector2(0.47f, 0.03f), 79, MeasurementUnit.PercentOfScreen);
                        countdownTexture.FadingAnimation = fadingAnimation;
                        SuperController.Instance.addGameObjectToGameInstance(countdownTexture);

                        // Weiter: Task zum Löschen von Countdown
                        TaskManager.Instance.addTask(new GameLogicTask(900, countdownTexture));

                        // Let gamepads rumble
                        InputManager.Instance.setRumble(300, new Vector2(0.3f, 0.3f));

                        // Let clock blink
                        ((HUD)relevantHUDObjects.GetObjectByName("Clock_highlightedRightHalf")).FadingAnimation = new FadingAnimation(false, true, 100, true, 200);
                        ((HUD)relevantHUDObjects.GetObjectByName("Clock_highlightedHalf")).FadingAnimation = new FadingAnimation(false, true, 100, true, 200);

                        activatedCountdowns[i-1] = true;
                    }
                }

            }
            else
            {
                // SCORE RECT

                // calculate percentage of highlighted rect
                float percentage = (Math.Max(score.X, score.Y) / ((ScoreLimit_WinCondition)winCondition).WinningScore);
                // calculate new Size
                Vector2 size = new Vector2(0.055f, (0.05f * 1.7778f * percentage));

                ((HUD)relevantHUDObjects.GetObjectByName("ScoreRect_highlighted")).Size = size;
                ((HUD)relevantHUDObjects.GetObjectByName("ScoreRect_highlighted")).Position = new Vector2(0.471f, (0.0398f + (0.05f * 1.7778f - size.Y)));

            }

            #endregion

        }




        /// <summary>
        /// Takes all the collisions of the collision list and checks which objects collided
        /// then it calls the according methods to process the collision
        /// </summary>
        private void processCollisions()
        {

            // Get every element in list
            foreach (ActiveObject[] activeObject in collisionList)
            {

                // ignore collision if it's the wrong format
                if (activeObject.Length != 2)
                    continue;

                // Ball with border
                if (((activeObject[0] is Ball) && (activeObject[1] is Border)) ||
                    ((activeObject[0] is Border) && (activeObject[1] is Ball)))
                {

                    // Store ball and border
                    Ball ball;
                    Border border;
                    if (activeObject[0] is Ball)
                    {
                        ball = (Ball)activeObject[0];
                        border = (Border)activeObject[1];
                    }
                    else
                    {
                        border = (Border)activeObject[0];
                        ball = (Ball)activeObject[1];
                    }

                    processCollision_Ball_Border(ball, border);

                }

                // Player with border
                else if (((activeObject[0] is Player) && (activeObject[1] is Border)) ||
                        ((activeObject[0] is Border) && (activeObject[1] is Player)))
                {

                    // Store player and border
                    Player player;
                    Border border;
                    if (activeObject[0] is Player)
                    {
                        player = (Player)activeObject[0];
                        border = (Border)activeObject[1];
                    }
                    else
                    {
                        border = (Border)activeObject[0];
                        player = (Player)activeObject[1];
                    }

                    processCollision_Player_Border(player, border);

                }

                // Player with ball
                else if (((activeObject[0] is Player) && (activeObject[1] is Ball)) ||
                    ((activeObject[0] is Ball) && (activeObject[1] is Player)))
                {

                    // Store player and ball
                    Player player;
                    Ball ball;
                    if (activeObject[0] is Player)
                    {
                        player = (Player)activeObject[0];
                        ball = (Ball)activeObject[1];
                    }
                    else
                    {
                        ball = (Ball)activeObject[0];
                        player = (Player)activeObject[1];
                    }

                    processCollision_Player_Ball(player, ball);

                }

                // Ball with net
                else if (((activeObject[0] is Ball) && (activeObject[1] is Net)) ||
                    ((activeObject[0] is Net) && (activeObject[1] is Ball)))
                {

                    // Store ball and net
                    Ball ball;
                    Net net;
                    if (activeObject[0] is Ball)
                    {
                        ball = (Ball)activeObject[0];
                        net = (Net)activeObject[1];
                    }
                    else
                    {
                        net = (Net)activeObject[0];
                        ball = (Ball)activeObject[1];
                    }

                    processCollision_Ball_Net(ball, net);

                }

                // Player with powerup
                else if (((activeObject[0] is Player) && (activeObject[1] is PowerUp)) ||
                    ((activeObject[0] is PowerUp) && (activeObject[1] is Player)))
                {

                    // Store player and powerup
                    Player player;
                    PowerUp powerup;
                    if (activeObject[0] is Player)
                    {
                        player = (Player)activeObject[0];
                        powerup = (PowerUp)activeObject[1];
                    }
                    else
                    {
                        powerup = (PowerUp)activeObject[0];
                        player = (Player)activeObject[1];
                    }

                    processCollision_Player_Powerup(player, powerup);

                }

            } 
         
            // clear list
            collisionList.Clear();

        }

        /// <summary>
        /// Processes a collision between a ball and a border
        /// </summary>
        /// <param name="ball">The ball involved in the collision</param>
        /// <param name="border">The border involved in the collision</param>
        private void processCollision_Ball_Border(Ball ball, Border border)
        {
            ParticleSystemsManager.Instance.stopMeteorBall(ball);
            // if border is bottom border -> score
            if (String.Equals(border.Name, "bottomBorder"))
            {
                matchBallSound = false;

                // if ball is in the left half
                if (ball.Position.X < (levelSize.X / 2))
                {
                    SuperController.Instance.increaseRightScore();
                    handleHotStreakPointBonus(2); //right team scored
                    handleDirectPointBonus(2);
                }
                // if ball is in the right half
                else
                {
                    SuperController.Instance.increaseLeftScore();
                    handleHotStreakPointBonus(1); //left team scored
                    handleDirectPointBonus(1);
                }

                removeBall(ball);

                // If there is no more ball and no task to spawn a ball, spawn a new one
                bool spawnBall = true;
                if (SuperController.Instance.getAllBalls().Count > 0)
                    spawnBall = false;

                if (spawnBall)
                    foreach (GraphicsTask task in TaskManager.Instance.GraphicTasks)
                        if (task.Task == GraphicTask.CreateBall)
                            spawnBall = false;

                // Create ball if necessary
                if (spawnBall)
                {
                    // Creating a new random ballResetPosition 
                    ballResetPosition = Physics.Instance.getRandomPositionAtTheTop();

                    createBall(ballResetPosition, 500);

                }

                //reset bonus counter
                lastPlayerThatHitBall = null;
                countBallCollisionsForTeam = 0;

                Logger.Instance.log(Sender.GameLogic, "Collision Ball-BottomBorder processed", PriorityLevel.Priority_1);
            }
        }

        /// <summary>
        /// Processes a collision between a player and a border
        /// </summary>
        /// <param name="player">The player involved in the collision</param>
        /// <param name="border">The border involved in the collision</param>
        private void processCollision_Player_Border(Player player, Border border)
        {

            // if border is bottom border -> reset possible jumps
            if (String.Equals(border.Name, "bottomBorder"))
            {

                // reset number of possible jumps for that player
                player.PossibleJumpsCounter = 1;

                ParticleSystemsManager.Instance.playSandFountain(new Vector2(player.Position.X + player.Size.X * 0.5f, player.Position.Y + player.Size.Y), player.PlayerIndex - 1);

                if (SuperController.Instance.GameInstance.LevelName == "Beach")
                {
                    TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.bottomTouchBeach, (int)IngameSound.BottomTouchBeach));
                }

                if (SuperController.Instance.GameInstance.LevelName == "Maya")
                {
                    TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.bottomTouchMaya, (int)IngameSound.BottomTouchMaya));
                }

                if (SuperController.Instance.GameInstance.LevelName == "Forest")
                {
                    TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.bottomTouchForest, (int)IngameSound.BottomTouchForest));
                }

                Logger.Instance.log(Sender.GameLogic, "Collision Player-BottomBorder processed", PriorityLevel.Priority_1);

            }

        }

        /// <summary>
        /// Processes a collision between a player and a ball
        /// </summary>
        /// <param name="player">The player involved in the collision</param>
        /// <param name="ball">The ball involved in the collision</param>
        private void processCollision_Player_Ball(Player player, Ball ball)
        {
            // Calculate position of collision
            Vector2 playerMiddlePosition = new Vector2(player.Position.X + (player.Size.X / 2), player.Position.Y + (player.Size.Y / 2));
            Vector2 ballMiddlePosition = new Vector2(ball.Position.X + (ball.Size.X / 2), ball.Position.Y + (ball.Size.Y / 2));
            float ratio = Math.Abs((player.Size.X / 2.0f) / (ballMiddlePosition - playerMiddlePosition).Length());

            Vector2 collisionPosition = playerMiddlePosition + ((ballMiddlePosition - playerMiddlePosition) * ratio);

            //calculate bonus            
            if(gameTime.TotalGameTime.TotalMilliseconds - timeLastHit >= 20)
                ParticleSystemsManager.Instance.stopMeteorBall(ball);

            handleBallbounceBonus(player);
            handleSmashBonus(collisionPosition, ball, player);
            lastPlayerThatHitBall = player;

            timeLastHit = gameTime.TotalGameTime.TotalMilliseconds;
            
            TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.hitBall, (int)IngameSound.HitBall));
            TaskManager.Instance.addTask(new ParticleSystemTask(0, collisionPosition, EffectIndicator.ballCollision));
        }

        /// <summary>
        /// Handles the Powerup display when someone picks up a Powerup.
        /// </summary>
        /// <param name="team">Team which got bonus.</param>
        /// <param name="p">Powerup which was obtained.</param>
        public void handlePowerupDisplay(PowerUp p)
        {
            if (!enablePowerups) return;
            //gets the right texture for the object we want to display
            Texture2D tex = StorageManager.Instance.getTextureByName(p.Name);

            //displays text by adding to objectList in SuperController
            HUD tempObject;
            HUD anchor = SuperController.Instance.getHUDElementByName("Scoreboard");

            if (p.PowerUpType == PowerUpType.Wind)
            {                
                foreach (HUD h in powerupDisplay_t1)
                {
                    if (h.Name == "Powerup_WindRight")
                        h.FadingAnimation.DurationBeforeReverse = 1;
                }
                foreach (HUD h in powerupDisplay_t2)
                {
                    if (h.Name == "Powerup_WindLeft")
                        h.FadingAnimation.DurationBeforeReverse = 1;
                }
            }

            //neutral powerUps
            if (p.PowerUpVersion == PowerUpVersion.Neutral)
            {
                Vector2 position = new Vector2(0.5f, anchor.Position.Y + anchor.Size.Y * 1.9f);
                if (powerupDisplay_neutral == null)
                {                    
                    //new object
                    powerupDisplay_neutral = new HUD("", true, tex, null, new Vector2(0.07f, 16f / 9 * 0.07f), position, 70, MeasurementUnit.PercentOfScreen);
                    powerupDisplay_neutral.Position -= powerupDisplay_neutral.Size / 2;
                    //tempObject.setFading(2, 3f, FadeValue.Quadratic);
                    powerupDisplay_neutral.FadingAnimation = new FadingAnimation(false, false, 0, true, 1000, Model.ParticleSystem.FadeMode.Quadratic);
                    powerupDisplay_neutral.FadingAnimation.DurationBeforeReverse = PowerUpManager.Instance.getDurationFromType(p.PowerUpType);
                    powerupDisplay_neutral.FadingAnimation.Paused = true;
                    powerupDisplay_neutral.FadingAnimation.Inverted = true;
                    powerupDisplay_neutral.FadingAnimation.ResetAfterFade = true;

                    SuperController.Instance.addGameObjectToGameInstance(powerupDisplay_neutral);
                }
                else
                {
                    powerupDisplay_neutral.Position = position - powerupDisplay_neutral.Size / 2;
                    powerupDisplay_neutral.Texture = tex;
                    powerupDisplay_neutral.FadingAnimation.DurationBeforeReverse = PowerUpManager.Instance.getDurationFromType(p.PowerUpType);
                    powerupDisplay_neutral.FadingAnimation.Paused = true;
                    powerupDisplay_neutral.FadingAnimation.Active = true;
                    powerupDisplay_neutral.Visible = true;
                }

                return;
            }

            //positive powerUps
            if ((p.Owner.Team == 1 && p.PowerUpType != PowerUpType.InvertedControl) || (p.Owner.Team == 2 && p.PowerUpType == PowerUpType.InvertedControl))
            {
                Vector2 position = new Vector2(0.1f, anchor.Position.Y + anchor.Size.Y * 1.5f);

                tempObject = (powerupDisplay_t1.Count > 0) ? powerupDisplay_t1.First() : null;
                if (tempObject == null || tempObject.FadingAnimation.Active)
                {
                    Curve2D curve = new Curve2D();
                    curve.addPoint(0f, position);
                    curve.addPoint(1f, position);
                    
                    PathAnimation anim = new PathAnimation(curve, 600);
                    //anim.Looping = true;

                    //new object
                    tempObject = new HUD(p.Name, true, tex, null, new Vector2(0.07f, 16f / 9 * 0.07f), position, 70, MeasurementUnit.PercentOfScreen);
                    tempObject.PathAnimation = anim;
                    tempObject.Position -= tempObject.Size / 2;
                    //tempObject.setFading(2, 3f, FadeValue.Quadratic);
                    tempObject.FadingAnimation = new FadingAnimation(false, false, 0, true, 1000, Model.ParticleSystem.FadeMode.Quadratic);
                    tempObject.FadingAnimation.DurationBeforeReverse = PowerUpManager.Instance.getDurationFromType(p.PowerUpType);
                    tempObject.FadingAnimation.Paused = true;
                    tempObject.FadingAnimation.Inverted = true;
                    tempObject.FadingAnimation.ResetAfterFade = true;
                    //tempObject.Alpha = 1f;

                    SuperController.Instance.addGameObjectToGameInstance(tempObject);

                    changePowerupPositions(powerupDisplay_t1, 1);
                    powerupDisplay_t1.Enqueue(tempObject);
                }
                else if(tempObject != null)
                {
                    movePowerupIconToFirst(powerupDisplay_t1, tempObject, tex, position, 1, p);
                }
            }
            else if ((p.Owner.Team == 2 && p.PowerUpType != PowerUpType.InvertedControl) || (p.Owner.Team == 1 && p.PowerUpType == PowerUpType.InvertedControl))
            {
                Vector2 position = new Vector2(1f - 0.1f, anchor.Position.Y + anchor.Size.Y * 1.5f);

                tempObject = (powerupDisplay_t2.Count > 0) ? powerupDisplay_t2.First() : null;
                if (tempObject == null || !tempObject.FadingAnimation.Active)
                {
                    Curve2D curve = new Curve2D();
                    curve.addPoint(0f, position);
                    curve.addPoint(1f, position);

                    PathAnimation anim = new PathAnimation(curve, 600);
                    //anim.Looping = true;

                    //new object
                    tempObject = new HUD(p.Name, true, tex, null, new Vector2(0.07f, 16f / 9 * 0.07f), position, 70, MeasurementUnit.PercentOfScreen);                    
                    tempObject.PathAnimation = anim;
                    tempObject.Position -= tempObject.Size / 2;
                    //tempObject.setFading(2, 3f, FadeValue.Quadratic);
                    tempObject.FadingAnimation = new FadingAnimation(false, false, 0, true, 1000, Model.ParticleSystem.FadeMode.Quadratic);
                    tempObject.FadingAnimation.DurationBeforeReverse = PowerUpManager.Instance.getDurationFromType(p.PowerUpType);
                    tempObject.FadingAnimation.Paused = true;
                    tempObject.FadingAnimation.Inverted = true;
                    
                    tempObject.FadingAnimation.ResetAfterFade = true;
                    //tempObject.Alpha = 1f;

                    SuperController.Instance.addGameObjectToGameInstance(tempObject);

                    changePowerupPositions(powerupDisplay_t2, 2);
                    powerupDisplay_t2.Enqueue(tempObject);
                }
                else if (tempObject != null)
                {
                    movePowerupIconToFirst(powerupDisplay_t2, tempObject, tex, position, 2, p);
                }
            }
            //neutral powerup
            else
            {

            }
        }

        /// <summary>
        /// Moves all objects in the list so that teamplay texts don't overlap.
        /// </summary>
        /// <param name="list">List of the objects you want to move.</param>
        /// <param name="team">The team that is affected.</param>
        private void changePowerupPositions(Queue<HUD> list, int team)
        {
            //move all other objects down
            foreach (HUD p in list)
            {
                if (p.PathAnimation == null || p.Visible == false) continue;

                //change curve points of the text
                Curve2D c = (Curve2D)p.PathAnimation.Path;
                p.PathAnimation.Active = true;
                if((c.evaluate(p.PathAnimation.ElapsedTime) - c.getPoints()[1]).Length() <= 0.05f) c.changeCurvePoint(0, c.getPoints()[1]);
                c.changeCurvePoint(1, c.getPoints()[1] + new Vector2( (team == 1) ? 0.09f : -0.09f, 0));

                p.PathAnimation.resetPosition();
            }
        }

        /// <summary>
        /// Removes the first object in the list and re-adds it at the end of the list.
        /// Object age increases from first to last object in the list, so we remove the oldest one.
        /// </summary>
        /// <param name="list">The list which is affected.</param>
        /// <param name="newObject">The object you want to move.</param>
        /// <param name="tex">The new texture of the object.</param>
        /// <param name="pos">The new position of the object.</param>
        /// <param name="team">The team that is affected.</param>
        private void movePowerupIconToFirst(Queue<HUD> list, HUD newObject, Texture2D tex, Vector2 pos, int team, PowerUp p)
        {
            if (newObject.PathAnimation == null) return;
            changePowerupPositions(list, team);

            //change curve points of the text
            Curve2D c = (Curve2D)newObject.PathAnimation.Path;
            c.changeCurvePoint(0, pos);
            c.changeCurvePoint(1, pos);
            newObject.PathAnimation.resetPosition();
            newObject.Position = pos - newObject.Size / 2;
            newObject.Texture = tex;
            newObject.FadingAnimation.DurationBeforeReverse = PowerUpManager.Instance.getDurationFromType(p.PowerUpType);
            newObject.FadingAnimation.Paused = true;
            newObject.FadingAnimation.Active = true;
            newObject.Visible = true;

            //renew old object
            list.Dequeue();
            list.Enqueue(newObject);
        }

        /// <summary>
        /// Calculates the amount of points the team gets for smashing the ball under certain circumstances.
        /// Player has to hit the ball above net hight.
        /// Ball has to move faster than 11f(is this m/s ?)
        /// Ball has to pass the net. (only angle is considered)
        /// </summary>
        /// <param name="collisionPosition">Position where player hit the ball.</param>
        /// <param name="ball">The ball that was hit.</param>
        /// <param name="player">Player that hit the ball.</param>
        private void handleSmashBonus(Vector2 collisionPosition, Ball ball, Player player)
        {            
            Vector2 ballBottom = new Vector2(ball.Position.X + ball.Size.X / 2, ball.Position.Y + ball.Size.Y);
            Vector2 ballToNet = new Vector2(net.Position.X + net.Size.X/2, net.Position.Y) - ballBottom ;
            float ratio = ballToNet.X / ball.Body.LinearVelocity.X;
            
            //you have to hit the ball above the net, its velocity must be high enough and the angle must be in a certain range
            if (net.Position.Y >= collisionPosition.Y && ( Math.Sqrt( Math.Pow(ball.Body.LinearVelocity.X,2) + Math.Pow(ball.Body.LinearVelocity.Y,2)/4 ) >= 10f) && 
                ball.Body.LinearVelocity.Y > -0.1f &&  (ballBottom.Y + ratio*ball.Body.LinearVelocity.Y < net.Position.Y + net.Size.Y / 5f) )
            {
                //return if bonus was granted in the last few ms to avoid double hits
                if ((gameTime.TotalGameTime.TotalMilliseconds - timeLastHit) < 200f) return;

                SuperController.Instance.setSpecialbarFilling(player.Team, 2 * energyMultiplicator);
                TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.smashBall, (int)IngameSound.SmashBall));
                ParticleSystemsManager.Instance.playMeteorBall(ball);
                Graphics.Instance.setTeamplayText(2, collisionPosition, player.Team, true);
            }
        }

        /// <summary>
        /// Calculates the amount of points the team gets for scoring many points in a row.
        /// </summary>
        /// <param name="teamNumber">Team of the player that scored. Int because couldn't acces scoring player by processCollision_Ball_Border.</param>
        private void handleHotStreakPointBonus(int teamNumber)
        {
            if (lastTeamThatScored == teamNumber)
            {
                pointsInARow++;
                if (pointsInARow % 3 == 0)
                {

                    SuperController.Instance.setSpecialbarFilling(teamNumber, 3 * energyMultiplicator);
                    Graphics.Instance.setTeamplayText(1, (teamNumber == 1) 
                        ? new Vector2(levelSize.X / 4, levelSize.Y / 2) : new Vector2(levelSize.X / 4 * 3, levelSize.Y / 2) , teamNumber, true);
                }
            }
            else
            {
                //other team scored or lastScore did not exist (init)
                pointsInARow = 1;
                lastTeamThatScored = teamNumber;
            }
        }

        /// <summary>
        /// Calculates the amount of points the team gets for scoring a point, without the other team touching the ball.
        /// </summary>
        /// <param name="team">1:left team, 2:right team</param>
        private void handleDirectPointBonus(int team)
        {
            if (!(lastPlayerThatHitBall == null) && lastPlayerThatHitBall.Team == team)
            {
                SuperController.Instance.setSpecialbarFilling(lastPlayerThatHitBall.Team, 3 * energyMultiplicator);

                // Calculate the middle position above the player: playerposition - 0.5 * (textsize (in meters) - playersize)
                Vector2 textSizeMeter = Graphics.Instance.convertUnits(((HUD)SuperController.Instance.getObjectByName("teamplay_t" + team)).Size, MeasurementUnit.Pixel,
                    MeasurementUnit.Meter);
                float xPosition = lastPlayerThatHitBall.Position.X - 0.5f * (textSizeMeter.X - lastPlayerThatHitBall.Size.X);
                Graphics.Instance.setTeamplayText(0, new Vector2(xPosition - 0.25f * lastPlayerThatHitBall.Size.X, lastPlayerThatHitBall.Position.Y), team, true);
            }
        }

        /// <summary>
        /// Counts how often the team hit the ball
        /// </summary>
        /// <param name="player">Player that hit the ball.</param>
        private void handleBallbounce(Player player)
        {
            //same team hit ball again, but not same player
            if (!(lastPlayerThatHitBall == null) && player.Team == lastPlayerThatHitBall.Team && lastPlayerThatHitBall != player)
            {
                if (Math.Abs(gameTime.TotalGameTime.TotalMilliseconds - timeLastHit) > 200)
                {
                    countBallCollisionsForTeam++;
                }
                
            }
            else
            {
                // other team got a ball hit or lastPlayer did not exist (init)        
                countBallCollisionsForTeam = 1;
            }
        }

        /// <summary>
        /// Calculates the amount of points the team gets for bouncing the ball.
        /// </summary>
        /// <param name="player">Player that hit the ball.</param>
        private void handleBallbounceBonus(Player player)
        {            
            //same team hit ball again, but not same player
            if (!(lastPlayerThatHitBall == null) && player.Team == lastPlayerThatHitBall.Team && lastPlayerThatHitBall != player && (Math.Abs(gameTime.TotalGameTime.TotalMilliseconds - timeLastHit) > 200))
            {
                countBallCollisionsForTeam++;
                //distribute points
                switch (countBallCollisionsForTeam)
                {
                    case 2:
                        SuperController.Instance.setSpecialbarFilling(player.Team, 1 * energyMultiplicator);
                        break;
                    case 3:
                        SuperController.Instance.setSpecialbarFilling(player.Team, 2 * energyMultiplicator);
                        break;
                    case 4:
                        SuperController.Instance.setSpecialbarFilling(player.Team, -1 * energyMultiplicator);
                        break;
                    case 5:
                        SuperController.Instance.setSpecialbarFilling(player.Team, -2 * energyMultiplicator);
                        break;
                    case 6:
                        SuperController.Instance.setSpecialbarFilling(player.Team, -3 * energyMultiplicator);
                        break;
                }
            }
            else
            {                
                // other team got a ball hit or lastPlayer did not exist (init)        
                countBallCollisionsForTeam = 1;
            }
        }

        /// <summary>
        /// Processes a collision between a ball and a net
        /// </summary>
        /// <param name="ball">The ball involved in the collision</param>
        /// <param name="net">The net involved in the collision</param>
        private void processCollision_Ball_Net(Ball ball, Net net)
        {
            ParticleSystemsManager.Instance.stopMeteorBall(ball);
            // Create Sound Task
            TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.netCollision, (int)IngameSound.HitNet));

        }

        /// <summary>
        /// Processes a collision between a player and a powerup
        /// </summary>
        /// <param name="player">The player involved in the collision</param>
        /// <param name="powerup">The powerup involved in the collision</param>
        private void processCollision_Player_Powerup(Player player, PowerUp powerup)
        {
            // Create Sound Task
            TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.pickupPowerup, (int)IngameSound.PickupPowerup));

        }

        /// <summary>
        /// Clears all collisions which are not processed yet
        /// </summary>
        public void clearCollisions()
        {
            // just clear whole collision list
            collisionList.Clear();
        }

        /// <summary>
        /// Triggers all necessary actions when one team wins
        /// </summary>
        /// <param name="winnerTeam">The name of the team that has won</param>
        public void levelEnds(int winnerTeam)
        {
            // Create sound
            TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.applauseGameEnd, (int)IngameSound.ApplauseGameEnd));
            SuperController.Instance.switchFromIngameToIngameMenu(winnerTeam);
        }


        private void handlePowerupCreation(GameTime gameTime)
        {

            // update all spawning times
            timeForNextSpawn_neutralPowerup -= gameTime.ElapsedGameTime.Milliseconds;

            #region Spawning neutral powerups

            if (timeForNextSpawn_neutralPowerup < 0)
            {
                // reset time
                timeForNextSpawn_neutralPowerup = random.Next((int)(averageSpawnTime_neutralPowerup - maxSpreading_spawnTimes * averageSpawnTime_neutralPowerup),
                    (int)(averageSpawnTime_neutralPowerup + maxSpreading_spawnTimes * averageSpawnTime_neutralPowerup));

                // spawn powerup
                Vector2 newPosition = new Vector2((float)random.Next(0, (int)levelSize.X), -5);
                PowerUpManager.Instance.createRandomPowerUp(newPosition, PowerUpVersion.Neutral);
            }

            #endregion

        }

        /// <summary>
        /// Changes the stone positions in Maya Level.
        /// </summary>
        /// <param name="init">True on init (only when called from gamelogic), false on every other call.</param>
        public void changeStonePositions(bool on)
        {
            if (SuperController.Instance.GameInstance.LevelName != "Maya") return;

            if (on)
            {

                while (activeMayaStones.Count == 0)
                {
                    // Find new random stones to switch on
                    for (int i = 0; i < stoneBlocks.Length; i++)
                    {
                        if (random.NextDouble() > 0.5f)
                        {
                            PassiveObject newStone = SuperController.Instance.getObjectByName("stone" + i) as PassiveObject;

                            // Go to the next stone if this one is blocked by objects
                            if (Physics.Instance.getBodiesInCircle(newStone.Position + newStone.Size / 2, Math.Max(newStone.Size.X, newStone.Size.Y) / 1.5f).Count > 0)
                            {
                                continue;
                            }

                            // Create sound
                            TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.mayaStoneChange, (int)IngameSound.MayaStongeChange));

                            // highlighting
                            newStone.ScalingAnimation.ScalingRange = new Vector2(1.0f, 1.25f);
                            newStone.ScalingAnimation.Active = true;

                            activeMayaStones.Add(i);
                            stoneBlocks[i] = Physics.Instance.createStaticRectangleObject(newStone.Size * 0.75f, newStone.Position + newStone.Size / 2 + new Vector2(0f, newStone.Size.Y * 0.1f), newStone.Rotation);

                            //light
                            Light l = newStone.AttachedObjects.First() as Light;
                            l.FadingAnimation.TimeSinceFadingStarted = 0;
                            l.FadingAnimation.DurationBeforeReverse = (int)SettingsManager.Instance.get("switchStonesEffectDuration") - 2000;
                            l.FadingAnimation.Active = true;
                            l.ScalingAnimation.ScalingRange = new Vector2(1.0f, 1.25f);
                            l.ScalingAnimation.Active = true;

                            // Start Waterfall
                            float leftSideOffset = 0.0f;
                            Vector2 waterfallPosition = newStone.Position + new Vector2(newStone.Size.X * 0.2f, newStone.Size.Y * 0.75f - 0.1f);
                            if (i == 0)
                            {
                                leftSideOffset = 0.5f;
                                waterfallPosition += new Vector2(0, 0.43f);
                            }
                            else if (i == 1)
                            {
                                leftSideOffset = 0.45f;
                                waterfallPosition += new Vector2(0, 0.37f);
                            }
                            else if (i == 2)
                            {
                                leftSideOffset = 0.3f;
                                waterfallPosition += new Vector2(0, 0.2f);
                            }
                            float xpos = (float)random.NextDouble() / 5;
                            waterfallPosition -= new Vector2(xpos / 2, 0);
                            Vector2 waterFallSize = new Vector2(newStone.Size.X * 0.65f + xpos,
                                SuperController.Instance.GameInstance.LevelSize.Y - newStone.Position.Y - newStone.Size.Y - leftSideOffset);
                            Waterfall w = new Waterfall(waterfallPosition, waterFallSize, i);
                            waterfalls.Add(w);
                            SuperController.Instance.addGameObjectToGameInstance(w);
                        }
                    }
                }
            }
            else
            {
                if (activeMayaStones.Count() > 0)
                {
                    // Create sound if any stones are removed
                    TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.mayaStoneChange, (int)IngameSound.MayaStongeChange));
                }


                // Turn off all stones
                for (int i = 0; i < stoneBlocks.Length; i++)
                {
                    if (activeMayaStones.Contains(i))
                    {
                        PassiveObject oldStone = SuperController.Instance.getObjectByName("stone" + i) as PassiveObject;
                        // tried to enlarge the size and scalling it down... does not work yet
                        oldStone.ScalingAnimation.TimeSinceFadingStarted = 0;
                        oldStone.ScalingAnimation.Active = true;
                        oldStone.ScalingAnimation.ScalingRange = new Vector2(1.25f, 1f);

                        Light l = oldStone.AttachedObjects.First() as Light;
                        l.ScalingAnimation.Active = true;
                        l.ScalingAnimation.ScalingRange = new Vector2(1.25f, 1f);

                        Physics.Instance.removeBodyFromPhysicSimulation(stoneBlocks[i]);
                        stoneBlocks[i] = null;
                    }
                }


                // Remove finished waterfalls
                List<Waterfall> removalList = new List<Waterfall>();
                foreach (Waterfall w in waterfalls)
                {
                    w.stop();
                    if (!w.Active)
                    {
                        removalList.Add(w);
                    }
                }

                foreach (Waterfall w in removalList)
                {
                    waterfalls.Remove(w);
                    SuperController.Instance.removeGameObjectFromGameInstance(w);
                }

                activeMayaStones.Clear();
            }

        }

        /// <summary>
        /// Implemented, because this class is an observer
        /// Everytime an observed object has changed, this class is called
        /// </summary>
        /// <param name=“observableObject”>the object which changed</param>
        public void notify(ObservableObject observableObject)
        {
            Logger.Instance.log(Sender.GameLogic, this.ToString() + " received a notification", PriorityLevel.Priority_2);
            //Check if we are Dealing with the right ObservableObject

            if (observableObject is SettingsManager)
            {
                maxSpreading_spawnTimes = (float)((SettingsManager)observableObject).get("maxSpreading_spawnTimes");
                averageSpawnTime_neutralPowerup = (int)((SettingsManager)observableObject).get("averageSpawnTime_neutralPowerup");
                enablePowerups = (bool)((SettingsManager)observableObject).get("enablePowerups");

                metertopixel = (float)SettingsManager.Instance.get("meterToPixel");

                Logger.Instance.log(Sender.GameLogic, this.ToString() + " received a notification", PriorityLevel.Priority_2);
            }
        }

        /// <summary>
        /// Processes all tasks that are sent to the Graphics by other Controllers.
        /// </summary>
        private void getTasks()
        {
            foreach (GameLogicTask task in TaskManager.Instance.GameLogicTasks)
            {
                switch (task.Task)
                {
                    //take actions because a powerup task was created
                    case GameLogicTask.GameLogicTaskType.RemoveStone:
                        if(SuperController.Instance.GameInstance.GameObjects.Contains(task.ObjectToRemove))
                            SuperController.Instance.removeGameObjectFromGameInstance(task.ObjectToRemove);
                        break;
                }
            }

            // Clear list after executing all tasks
            TaskManager.Instance.GameLogicTasks.Clear();
        }

        /// <summary>
        /// Sets the flag, if the next ball is a match ball.
        /// </summary>
        /// <param name="score">The latest score of the game</param>
        private void activateMatchballFlag(Vector2 score)
        {

            if (!matchBallSound)
            {
                TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.matchballSignal, (int)IngameSound.MatchballSignal));
                matchBallSound = true;
            }

            if (this.winCondition is TimeLimit_WinCondition)
            {
                this.matchBallTimeWinningCondition = true;
                startScoreboardBlinking(0);
            }
            else
            {
                if (score.X > score.Y)
                    startScoreboardBlinking(1);
                else if (score.X < score.Y)
                    startScoreboardBlinking(2);
                else
                    startScoreboardBlinking(0);
            }


        }


        /// <summary>
        /// Creates a new ball at the specified position
        /// </summary>
        /// <param name="position">The position where the ball will appear</param>
        /// <param name="afterTime">The time in MS after which the ball will appear</param>
        public void createBall(Vector2 position, int afterTime)
        {

            // Showing a Ball-Texture at the releasing point for a short Period
            GraphicsTask g = new GraphicsTask(afterTime, GraphicTask.CreateBall, 4);
            g.Position = position;
            TaskManager.Instance.addTask(g);
            TaskManager.Instance.addTask(new GraphicsTask(afterTime + 1000, GraphicTask.CreateBall, 5));

            // Creating the new ball afterwards
            TaskManager.Instance.addTask(new PhysicsTask(afterTime + 1000, PhysicsTask.PhysicTaskType.CreateNewBall, position));

        }

        /// <summary>
        /// Removes the given ball from the level
        /// </summary>
        /// <param name="ball">The ball that should be removed</param>
        private void removeBall(Ball ball)
        {

            //remove Lights from Ball in forest level
            Graphics.Instance.removeLightFromObject(ball);


            // Remove highlight
            PassiveObject ballHighlight = null;

            foreach (DrawableObject attachedObject in ball.AttachedObjects)
                if (attachedObject.Name == "BallHighlight")
                    ballHighlight = (PassiveObject)attachedObject;

            if (ballHighlight != null)
                SuperController.Instance.removeGameObjectFromGameInstance(ballHighlight);


            // remove the ball
            SuperController.Instance.removeGameObjectFromGameInstance(ball);

            // delete corresponding  ballIndicator
            SuperController.Instance.removeGameObjectFromGameInstance(ball.BallIndicator);

        }

        /// <summary>
        /// Let the score in the scoreboard blinking
        /// </summary>
        /// <param name="team"> 1 = left team, 2 = right team, 3 = both teams</param>
        private void startScoreboardBlinking(int team)
        {

            int timeBeforeReverse = 100;
            int fadingTime = 800;

            // Left score
            if ((team == 1) || (team == 0))
            {
                if (SuperController.Instance.getHUDElementByName("Digit_left_1").FadingAnimation == null)
                {
                    SuperController.Instance.getHUDElementByName("Digit_left_1").FadingAnimation = new FadingAnimation(true, true, timeBeforeReverse, true, fadingTime);
                    SuperController.Instance.getHUDElementByName("Digit_left_1").BlendColor = Color.Red;
                    SuperController.Instance.getHUDElementByName("Digit_left_2").FadingAnimation = new FadingAnimation(true, true, timeBeforeReverse, true, fadingTime);
                    SuperController.Instance.getHUDElementByName("Digit_left_2").BlendColor = Color.Red;
                }
            }

            // Right score
            if ((team == 2) || (team == 0))
            {
                if (SuperController.Instance.getHUDElementByName("Digit_right_1").FadingAnimation == null)
                {
                    SuperController.Instance.getHUDElementByName("Digit_right_1").FadingAnimation = new FadingAnimation(true, true, timeBeforeReverse, true, fadingTime);
                    SuperController.Instance.getHUDElementByName("Digit_right_1").BlendColor = Color.Red;
                    SuperController.Instance.getHUDElementByName("Digit_right_2").FadingAnimation = new FadingAnimation(true, true, timeBeforeReverse, true, fadingTime);
                    SuperController.Instance.getHUDElementByName("Digit_right_2").BlendColor = Color.Red;
                }
            }

        }

        #endregion

        #endregion



    }
}

