using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using ManhattanMorning.View;
using ManhattanMorning.Misc;
using ManhattanMorning.Misc.Curves;
using ManhattanMorning.Misc.Levels;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model.HUD;
using ManhattanMorning.Model;
using ManhattanMorning.Model.ParticleSystem;
using ManhattanMorning.Misc.Logic;
using ManhattanMorning.Model.Menu;

using FarseerPhysics.Dynamics;




namespace ManhattanMorning.Controller
{   
    /// <summary>
    /// Responsible for loading levels and menus out
    /// of files
    /// </summary>
    class StorageManager
    {

        #region Properties

        /// <summary>
        /// Instance of StorageManager for Singleton Pattern
        /// </summary>
        public static StorageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StorageManager();
                }
                return instance;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Necessary Instance for Singleton Pattern
        /// </summary>
        private static StorageManager instance;

        /// <summary>
        /// Saves textures which can be accessed from other controllers 
        /// (check StorageManager's methods)
        /// </summary>
        private Dictionary<String, Texture2D> textures;

        #endregion


        #region Initialization

        /// <summary>
        /// Hidden Constructor as we use Singleton here.
        /// </summary>
        private StorageManager()
        {
            // Initialize Dictionary for all textures
            textures = new Dictionary<string, Texture2D>();
        }

        #endregion

        #region Methods

        #region Methods to load a level

        /// <summary>
        /// Takes the gameInstance and loads all the static data
        /// (Levelsize, ResetPositions) which is speciefied in level into it
        /// </summary>
        /// <param name="level">The level that contains the specifications</param>
        /// <param name="gameInstance">The GameInstance which properties should be set</param>
        /// <param name="resizeFactorLevelSize">The resize factor for the level if it is a 2vs2 game</param>
        public void loadStaticLevelData(Level level, GameInstance gameInstance, float resizeFactorLevelSize)
        {

            gameInstance.LevelSize = resizeFactorLevelSize * (Vector2)level.LevelProperties["LevelSize"];
            gameInstance.BallResetPosition = resizeFactorLevelSize * (Vector2)level.LevelProperties["BallResetPosition"];
            gameInstance.Team1ResetPosition = resizeFactorLevelSize * (Vector2)level.LevelProperties["LeftTeamResetPosition"];
            gameInstance.Team2ResetPosition = resizeFactorLevelSize * (Vector2)level.LevelProperties["RightTeamResetPosition"];
            gameInstance.IntroVideo = Game1.Instance.Content.Load<Video>(@"Videos\video");
            gameInstance.BeachVideo = Game1.Instance.Content.Load<Video>(@"Videos\video");
            gameInstance.ForestVideo = Game1.Instance.Content.Load<Video>(@"Videos\video");
            gameInstance.MayaVideo = Game1.Instance.Content.Load<Video>(@"Videos\maya");
        }


        /// <summary>
        /// Takes the gameInstance and loads all the level objects
        /// which are specified in the level into it
        /// </summary>
        /// <param name="level">The level that contains the specifications</param>
        /// <param name="gameInstance">The GameInstance that gets all the objects</param>
        /// <param name="playerLeftTeam">The representations for the player of the left team</param>
        /// <param name="playerLeftTeam">The representations for the player of the right team</param>
        /// <param name="resizeFactorLevelSize">The resize factor for the level if it is a 2vs2 game</param>
        public void loadLevelObjects(Level level, GameInstance gameInstance, List<PlayerRepresentationMainMenu> playerLeftTeam,
            List<PlayerRepresentationMainMenu> playerRightTeam, float resizeFactorLevelSize)
        {
            level.load();

            // Get temp values/references
            Game1 game1 = Game1.Instance;
            String levelName = (String)level.LevelProperties["LevelName"];

            // Set allowed powerups
            PowerUpManager.Instance.setAllowedPowerUps(level.AllowedPowerUps);

            #region Players

            Texture2D player1Texture = game1.Content.Load<Texture2D>(@"Textures\Levels\Default\Player_blue");
            Texture2D player2Texture = game1.Content.Load<Texture2D>(@"Textures\Levels\Default\Player_red");
            Texture2D player3Texture = game1.Content.Load<Texture2D>(@"Textures\Levels\Default\Player_green");
            Texture2D player4Texture = game1.Content.Load<Texture2D>(@"Textures\Levels\Default\Player_yellow"); 
            Vector2 playerSize = new Vector2((float)SettingsManager.Instance.get("playerSize"), (float)SettingsManager.Instance.get("playerSize"));

            // Check if it's a 2vs2 game (necessary for spawn positions)
            float offset = 0;
            if (playerLeftTeam.Count > 1)
                offset = playerSize.X;

            // Create all left team player 
            for (int i = 0; i < 2; i++)
            {
                int team = i + 1;

                foreach (PlayerRepresentationMainMenu playerRepresentation in (team == 1) ? playerLeftTeam : playerRightTeam)
                {                    
                    Vector2 resetPosition = (team == 1) ? gameInstance.Team1ResetPosition : gameInstance.Team2ResetPosition;
                    PlayerStatus status;

                    if (playerRepresentation.KI)
                        status = PlayerStatus.KIPlayer;
                    else
                        status = PlayerStatus.HumanPlayer;

                    // Select the according player texture
                    Texture2D playerTexture = null;
                    switch (playerRepresentation.PlayerIndex)
                    {
                        case 1:
                            playerTexture = player1Texture;
                            break;
                        case 2:
                            playerTexture = player2Texture;
                            break;
                        case 3:
                            playerTexture = player3Texture;
                            break;
                        case 4:
                            playerTexture = player4Texture;
                            break;
                    }

                    Player player = new Player("player" + playerRepresentation.PlayerIndex, true, playerTexture, null, null, playerSize,
                        resetPosition, 50,
                        1, MeasurementUnit.Meter, playerRepresentation.PlayerIndex, team, status, playerRepresentation.InputDevice);

                    // Add player to given GameInstance
                    SuperController.Instance.addGameObjectToGameInstance(player);
                }
            }

            // If it's a 2vs 2 game, disable collision between team members
            if (playerLeftTeam.Count > 1)
            {
                // Btween Player and Player
                Physics.Instance.disableCollisionBetweenActiveObjects(gameInstance.GameObjects.GetPlayer(playerLeftTeam[0].PlayerIndex),
                    gameInstance.GameObjects.GetPlayer(playerLeftTeam[1].PlayerIndex));
                // Between Hand and Hand
                Physics.Instance.disableCollisionBetweenBodies(gameInstance.GameObjects.GetPlayer(playerLeftTeam[0].PlayerIndex).HandBody,
                    gameInstance.GameObjects.GetPlayer(playerLeftTeam[1].PlayerIndex).HandBody);
                // Between Hand and Player
                Physics.Instance.disableCollisionBetweenBodies(gameInstance.GameObjects.GetPlayer(playerLeftTeam[0].PlayerIndex).HandBody,
                    gameInstance.GameObjects.GetPlayer(playerLeftTeam[1].PlayerIndex).Body);
                Physics.Instance.disableCollisionBetweenBodies(gameInstance.GameObjects.GetPlayer(playerLeftTeam[0].PlayerIndex).Body,
                    gameInstance.GameObjects.GetPlayer(playerLeftTeam[1].PlayerIndex).HandBody);

                // Btween Player and Player
                Physics.Instance.disableCollisionBetweenActiveObjects(gameInstance.GameObjects.GetPlayer(playerRightTeam[0].PlayerIndex),
                    gameInstance.GameObjects.GetPlayer(playerRightTeam[1].PlayerIndex));
                // Between Hand and Player
                Physics.Instance.disableCollisionBetweenBodies(gameInstance.GameObjects.GetPlayer(playerRightTeam[0].PlayerIndex).HandBody,
                    gameInstance.GameObjects.GetPlayer(playerRightTeam[1].PlayerIndex).HandBody);
                // Between Hand and Player
                Physics.Instance.disableCollisionBetweenBodies(gameInstance.GameObjects.GetPlayer(playerRightTeam[0].PlayerIndex).HandBody,
                    gameInstance.GameObjects.GetPlayer(playerRightTeam[1].PlayerIndex).Body);
                Physics.Instance.disableCollisionBetweenBodies(gameInstance.GameObjects.GetPlayer(playerRightTeam[0].PlayerIndex).Body,
                    gameInstance.GameObjects.GetPlayer(playerRightTeam[1].PlayerIndex).HandBody);

            }
            
            #endregion

            #region Powerups

            // Load the textures
            Texture2D powerupBox_volcano = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_volcano");
            Texture2D powerupBox_bomb = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_bomb");
            Texture2D powerupBox_doubleball = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_double");
            Texture2D powerupBox_invert = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_invert");
            Texture2D powerupBox_jump = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_jump");
            Texture2D powerupBox_rain = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_rain");
            Texture2D powerupBox_sun = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_sun");
            Texture2D powerupBox_wall = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_wall");
            Texture2D powerupBox_wind_left = game1.Content.Load<Texture2D>(@"Textures\Powerups\powerupBox_wind_left");

            // Save textures for later powerup creation
            saveTextureByName(powerupBox_volcano, "PowerupBox_" + PowerUpType.Volcano.ToString() );
            saveTextureByName(powerupBox_bomb, "PowerupBox_" + PowerUpType.SuperBomb.ToString());
            saveTextureByName(powerupBox_doubleball, "PowerupBox_" + PowerUpType.DoubleBall.ToString());
            saveTextureByName(powerupBox_invert, "PowerupBox_" + PowerUpType.InvertedControl.ToString());
            saveTextureByName(powerupBox_jump, "PowerupBox_" + PowerUpType.Jumpheight.ToString());
            saveTextureByName(powerupBox_rain, "PowerupBox_" + PowerUpType.BallRain.ToString());
            saveTextureByName(powerupBox_sun, "PowerupBox_" + PowerUpType.SunsetSunrise.ToString());
            saveTextureByName(powerupBox_wall, "PowerupBox_" + PowerUpType.SwitchStones.ToString());
            saveTextureByName(powerupBox_wind_left, "PowerupBox_" + PowerUpType.Wind.ToString());
          

            #endregion

            // Temporary net size needed for middle border
            Vector2 tempNetSize = Vector2.Zero;
            Vector2 tempNetPosition = Vector2.Zero;
            Net tempNet = null;

            // Take each level object and check which type it is, then create the right game object
            foreach (DrawableObject drawObj in level.LevelObjectsList)
            {
                if (drawObj is PassiveObject && !(drawObj is Light))
                {
                    #region PassiveObjects

                    PassiveObject tempObject = drawObj as PassiveObject;

                    // Check if it has an animation
                    if (tempObject.TextureName != null)
                    {
                        // Load the texture
                        tempObject.Texture = game1.Content.Load<Texture2D>(@"Textures\Levels\" + levelName + "\\" + tempObject.TextureName);
                        drawObj.Visible = true;
                    }
                    if(tempObject.Animation != null)
                    {
                        // Create the animation
                        tempObject.Animation.SpriteStrip = game1.Content.Load<Texture2D>(@"Textures\Levels\" + levelName + "\\" + tempObject.Animation.TextureName);
                        drawObj.Visible = true;
                    }
                    tempObject.Position *= resizeFactorLevelSize;
                    tempObject.Size *= resizeFactorLevelSize;

                    // Check if it has a path
                    if (tempObject.PathAnimation != null && resizeFactorLevelSize != 1f)
                    {
                        Curve2D curve = tempObject.PathAnimation.Path as Curve2D;
                        List<Vector2> points = curve.getPoints();
                        for(int i=0; i<points.Count; i++)
                        {
                            curve.changeCurvePoint(i, points.ElementAt(i) * resizeFactorLevelSize);
                        }
                        curve.SetTangents();
                    }                    

                    // Add it to the gameInstance
                    SuperController.Instance.addGameObjectToGameInstance(tempObject);

                    #endregion
                }
                else if (drawObj is Net)
                {
                    #region Net

                    Net tempObject = drawObj as Net;

                    // Load the texture
                    tempObject.Texture = game1.Content.Load<Texture2D>(@"Textures\Levels\" + levelName + "\\" + tempObject.TextureName);
                    drawObj.Visible = true;

                    // Create the object
                    tempObject.Size *= resizeFactorLevelSize;
                    tempNetPosition = new Vector2((gameInstance.LevelSize.X / 2) - (tempObject.Size.X / 2), gameInstance.LevelSize.Y - tempObject.Size.Y);
                    tempObject.Position = tempNetPosition;

                    SuperController.Instance.addGameObjectToGameInstance(tempObject);
                    // Make the middle border a little bit broader then the net
                    tempNetSize = new Vector2(tempObject.Size.X * 1.05f, tempObject.Size.Y);
                    tempNet = tempObject;

                    //Physics.Instance.disableCollisionBetweenActiveObjectAndCollisionCategory(net, Category.Cat4);

                    #endregion
                }
                else if (drawObj is Ball)
                {
                    #region Ball

                    Ball tempObject = drawObj as Ball;

                    // Load the texture
                    Texture2D objectTexture = game1.Content.Load<Texture2D>(@"Textures\Levels\" + levelName + "\\" + tempObject.TextureName);

                    // Save texture for later ball creation
                    if (textures.ContainsKey("Ball"))
                        textures.Remove("Ball");
                    saveTextureByName(objectTexture, "Ball");
                    #endregion
                }
                else if (drawObj is Border)
                {
                    #region Border

                    Border tempObject = drawObj as Border;

                    // Load the texture
                    if (tempObject.TextureName != null)
                    {
                        Texture2D objectTexture = game1.Content.Load<Texture2D>(@"Textures\Levels\" + levelName + "\\" + tempObject.TextureName);
                        // Save texture for later powerup creation
                        saveTextureByName(objectTexture, "Border");
                        tempObject.Texture = objectTexture;
                        drawObj.Visible = true;
                    }
                    tempObject.Position *= resizeFactorLevelSize;
                    tempObject.Size *= resizeFactorLevelSize;

                    SuperController.Instance.addGameObjectToGameInstance(tempObject);

                    #endregion
                }
                else if (drawObj is Light)
                {
                    Light tempObject = drawObj as Light;
                    tempObject.Texture = game1.Content.Load<Texture2D>(@"Textures\Light\" + tempObject.TextureName);
                    drawObj.Visible = true;
                    tempObject.Position *= resizeFactorLevelSize;
                    tempObject.Size *= resizeFactorLevelSize;

                    SuperController.Instance.addGameObjectToGameInstance(tempObject);
                }
            }

            #region Borders

            // Positions of the new physical objects, pointing to the Upper left position
            Vector2 leftBorderPosition = new Vector2(-1f, -3f * gameInstance.LevelSize.Y);
            Vector2 rightBorderPosition = new Vector2(gameInstance.LevelSize.X, -3 * gameInstance.LevelSize.Y);
            // Set the top border very high, but set it
            // So that the ball can't leave level
            Vector2 topBorderPosition = new Vector2(-1f, -3 * gameInstance.LevelSize.Y - 1f);
            Vector2 bottomBorderPosition = new Vector2(-1f, gameInstance.LevelSize.Y * 0.95f);

            Border leftBorder = new Border("leftBorder", false, null, null, null, new Vector2(1.0f, (gameInstance.LevelSize.Y * 4) + 1f),
                leftBorderPosition, 50, Model.MeasurementUnit.Meter);
            Border rightBorder = new Border("rightBorder", false, null, null, null, new Vector2(1.0f, (gameInstance.LevelSize.Y * 4 + 1f)),
                rightBorderPosition, 50, Model.MeasurementUnit.Meter);
            Border topBorder = new Border("topBorder", false, null, null, null, new Vector2(gameInstance.LevelSize.X + 2f, 1.0f),
                topBorderPosition, 50, Model.MeasurementUnit.Meter);
            Border bottomBorder = new Border("bottomBorder", false, null, null, null, new Vector2(gameInstance.LevelSize.X + 2f, 1.0f),
                bottomBorderPosition, 50, Model.MeasurementUnit.Meter);

            Border middleBorder = new Border("middleBorder", false, null, null, null, new Vector2(tempNetSize.X, (4 * gameInstance.LevelSize.Y)),
                new Vector2(gameInstance.LevelSize.X / 2.0f - (0.5f * tempNetSize.X), (-3 * gameInstance.LevelSize.Y)), 50, Model.MeasurementUnit.Meter);            
            
            Texture2D test = game1.Content.Load<Texture2D>(@"Textures\HUD\BallIndicator");
            
            // Borders to prevent lifting at the net
            Border rightSideNetHandBorder = new Border("rightSideNetHandBorder", false, test, null, null, new Vector2(1.0f, 0.2f), tempNet.Position + new Vector2(tempNetSize.X, 0), 50, Model.MeasurementUnit.Meter);
            Border leftSideNetHandBorder = new Border("leftSideNetHandBorder", false, test, null, null, new Vector2(1.0f, 0.2f), tempNet.Position - new Vector2(1.0f, 0), 50, Model.MeasurementUnit.Meter);

            // Borders to prevent right players from grabbing too far over the net.
            float allowedHandDistance = 0.3f;
            float borderHeight = 4.0f;
            float magicNumber = 0.2f;

            Border rightSideHandDistanceBorder = new Border("rightSideHandDistanceBorder", true, test, null, null, new Vector2(0.2f, 5.0f), tempNet.Position + new Vector2(tempNetSize.X, -borderHeight * 2) / 2 + new Vector2(allowedHandDistance, 0), 50, Model.MeasurementUnit.Meter);
            Border leftSideHandDistanceBorder = new Border("leftSideHandDistanceBorder", true, test, null, null, new Vector2(0.2f, 5.0f), tempNet.Position + new Vector2(tempNetSize.X, -borderHeight * 2) / 2 - new Vector2(allowedHandDistance + magicNumber, 0), 50, Model.MeasurementUnit.Meter);

            SuperController.Instance.addGameObjectToGameInstance(leftBorder);
            SuperController.Instance.addGameObjectToGameInstance(rightBorder);
            SuperController.Instance.addGameObjectToGameInstance(topBorder);
            SuperController.Instance.addGameObjectToGameInstance(bottomBorder);
            SuperController.Instance.addGameObjectToGameInstance(middleBorder);
            SuperController.Instance.addGameObjectToGameInstance(rightSideNetHandBorder);
            SuperController.Instance.addGameObjectToGameInstance(leftSideNetHandBorder);
            SuperController.Instance.addGameObjectToGameInstance(rightSideHandDistanceBorder);
            SuperController.Instance.addGameObjectToGameInstance(leftSideHandDistanceBorder);

            Physics.Instance.disableCollisionBetweenActiveObjectAndCollisionCategory(middleBorder, Category.Cat4);
            Physics.Instance.disableCollisionBetweenActiveObjectAndCollisionCategory(bottomBorder, Category.Cat4);
            Physics.Instance.disableCollisionBetweenActiveObjectAndCollisionCategory(leftBorder, Category.Cat4);
            Physics.Instance.disableCollisionBetweenActiveObjectAndCollisionCategory(rightBorder, Category.Cat4);

            // For player, create other left and right border
            // Reason: They can half leave playingfield

            // player has to ignore original left and right borders

            foreach (ActiveObject tempPlayer in gameInstance.GameObjects.GetPlayer())
            {
                Physics.Instance.disableCollisionBetweenActiveObjects(leftBorder, tempPlayer);
                Physics.Instance.disableCollisionBetweenActiveObjects(rightBorder, tempPlayer);
                Physics.Instance.setPlayerHandCollisionCategory((Player)tempPlayer, Category.Cat4);

                if (((Player)tempPlayer).Team == 1)
                {
                    Physics.Instance.disableCollisionBetweenActiveObjects(leftSideNetHandBorder, tempPlayer);
                    Physics.Instance.disableCollisionBetweenActiveObjects(leftSideHandDistanceBorder, tempPlayer);
                    Physics.Instance.disableCollisionBetweenBodies(leftSideNetHandBorder.Body, ((Player)tempPlayer).HandBody);
                    Physics.Instance.disableCollisionBetweenBodies(leftSideHandDistanceBorder.Body, ((Player)tempPlayer).HandBody);
                }
                else
                {
                    Physics.Instance.disableCollisionBetweenActiveObjects(rightSideNetHandBorder, tempPlayer);
                    Physics.Instance.disableCollisionBetweenActiveObjects(rightSideHandDistanceBorder, tempPlayer);
                    Physics.Instance.disableCollisionBetweenBodies(rightSideNetHandBorder.Body, ((Player)tempPlayer).HandBody);
                    Physics.Instance.disableCollisionBetweenBodies(rightSideHandDistanceBorder.Body, ((Player)tempPlayer).HandBody);
                }
            }

            // calculate positions of player borders
            leftBorderPosition -= new Vector2((0.5f * gameInstance.GameObjects.GetPlayer()[0].Size.X), 0);
            rightBorderPosition += new Vector2((0.5f * gameInstance.GameObjects.GetPlayer()[0].Size.X), 0);

            leftBorder = new Border("leftPlayerBorder", false, null, null, null, new Vector2(1.0f, (gameInstance.LevelSize.Y * 4)),
                leftBorderPosition, 50, Model.MeasurementUnit.Meter);
            rightBorder = new Border("rightPlayerBorder", false, null, null, null, new Vector2(1.0f, (gameInstance.LevelSize.Y * 4)),
                rightBorderPosition, 50, Model.MeasurementUnit.Meter);

            Physics.Instance.setCollisionCategory(leftBorder.Body, Category.Cat3);
            Physics.Instance.setCollisionCategory(rightBorder.Body, Category.Cat3);
            SuperController.Instance.addGameObjectToGameInstance(leftBorder);
            SuperController.Instance.addGameObjectToGameInstance(rightBorder);

            #endregion

            // A Passive Object to display where a ball after a score will release
            #region BallIndicatorAfterScore
            PassiveObject ballDisplayAfterScore = new PassiveObject("BallAfterScore", false, getTextureByName("Ball"), null, null, (Vector2)SettingsManager.Instance.get("ballSize"), GameLogic.Instance.ballResetPosition, 52, MeasurementUnit.Meter, null);
            FadingAnimation fading = new FadingAnimation(false, false, 0, false, 1000);
            Light light = new Light("", getTextureByName("light_smooth"), ballDisplayAfterScore.Size*2, ballDisplayAfterScore.Position, Color.White, false, null);

            ballDisplayAfterScore.attachObject(light);
            ballDisplayAfterScore.FadingAnimation = fading;
            light.FadingAnimation = fading;

            SuperController.Instance.addGameObjectToGameInstance(ballDisplayAfterScore);
            SuperController.Instance.addGameObjectToGameInstance(light);
            #endregion

        }

        #endregion

        /// <summary>
        /// Loads textures which are needed while runtime to create objects / replace textures
        /// </summary>
        public void loadTexturesForDictionary()
        {

            // Get Game1 reference
            Game1 game1 = Game1.Instance;

            // clear Dictionary
            textures.Clear();

            #region Ingame HUD

            Texture2D texture_countdown_1 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_01");
            Texture2D texture_countdown_2 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_02");
            Texture2D texture_countdown_3 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_03");
            Texture2D texture_countdown_4 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_04");
            Texture2D texture_countdown_5 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_05");
            Texture2D texture_countdown_6 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_06");
            Texture2D texture_countdown_7 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_07");
            Texture2D texture_countdown_8 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_08");
            Texture2D texture_countdown_9 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_09");
            Texture2D texture_countdown_10 = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\Countdown_10");

            saveTextureByName(texture_countdown_1, "texture_countdown_1");
            saveTextureByName(texture_countdown_2, "texture_countdown_2");
            saveTextureByName(texture_countdown_3, "texture_countdown_3");
            saveTextureByName(texture_countdown_4, "texture_countdown_4");
            saveTextureByName(texture_countdown_5, "texture_countdown_5");
            saveTextureByName(texture_countdown_6, "texture_countdown_6");
            saveTextureByName(texture_countdown_7, "texture_countdown_7");
            saveTextureByName(texture_countdown_8, "texture_countdown_8");
            saveTextureByName(texture_countdown_9, "texture_countdown_9");
            saveTextureByName(texture_countdown_10, "texture_countdown_10");

            Texture2D texture_digit_0 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_0");
            Texture2D texture_digit_1 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_1");
            Texture2D texture_digit_2 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_2");
            Texture2D texture_digit_3 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_3");
            Texture2D texture_digit_4 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_4");
            Texture2D texture_digit_5 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_5");
            Texture2D texture_digit_6 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_6");
            Texture2D texture_digit_7 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_7");
            Texture2D texture_digit_8 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_8");
            Texture2D texture_digit_9 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_9");

            Texture2D specialbar_fillingRectTexture_left = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\specialbar_filling_left");
            Texture2D specialbar_fillingRectTexture_right = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\specialbar_filling_right");

            saveTextureByName(texture_digit_0, "texture_digit_0");
            saveTextureByName(texture_digit_1, "texture_digit_1");
            saveTextureByName(texture_digit_2, "texture_digit_2");
            saveTextureByName(texture_digit_3, "texture_digit_3");
            saveTextureByName(texture_digit_4, "texture_digit_4");
            saveTextureByName(texture_digit_5, "texture_digit_5");
            saveTextureByName(texture_digit_6, "texture_digit_6");
            saveTextureByName(texture_digit_7, "texture_digit_7");
            saveTextureByName(texture_digit_8, "texture_digit_8");
            saveTextureByName(texture_digit_9, "texture_digit_9");

            Texture2D bonus_smash = game1.Content.Load<Texture2D>(@"Textures\HUD\PowerupText\bonus_smash");
            Texture2D bonus_teamplay = game1.Content.Load<Texture2D>(@"Textures\HUD\PowerupText\bonus_teamplay");
            Texture2D bonus_directpoint = game1.Content.Load<Texture2D>(@"Textures\HUD\PowerupText\bonus_directpoint");
            Texture2D bonus_pointstreak = game1.Content.Load<Texture2D>(@"Textures\HUD\PowerupText\bonus_pointstreak");
            Texture2D bonus_control = game1.Content.Load<Texture2D>(@"Textures\HUD\PowerupText\bonus_control");

            saveTextureByName(bonus_smash, "bonus_smash");
            saveTextureByName(bonus_teamplay, "bonus_teamplay");
            saveTextureByName(bonus_directpoint, "bonus_directpoint");
            saveTextureByName(bonus_pointstreak, "bonus_pointstreak");
            saveTextureByName(bonus_control, "bonus_control");

            saveTextureByName(specialbar_fillingRectTexture_left, "specialbar_fillingRectTexture_left");
            saveTextureByName(specialbar_fillingRectTexture_right, "specialbar_fillingRectTexture_right");


            Texture2D ballIndicatorTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\BallIndicator");
            saveTextureByName(ballIndicatorTexture, "BallIndicator");

            #endregion

            #region PowerUps

            Texture2D texturePowerUpBomb = game1.Content.Load<Texture2D>(@"Textures\PowerUps\powerup_bomb");
            Texture2D texturePowerUpLava = game1.Content.Load<Texture2D>(@"Textures\PowerUps\powerup_lava");
            Texture2D texturePowerUpJump = game1.Content.Load<Texture2D>(@"Textures\Animations\powerjump_cut");

            Texture2D iconBomb = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_bomb");
            Texture2D iconDouble = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_double");
            Texture2D iconInvert = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_invert");
            Texture2D iconJump = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_jump");
            Texture2D iconRain = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_rain");
            Texture2D iconSun = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_sun");
            Texture2D iconVulcano = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_vulcano");
            Texture2D iconWall = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_wall");
            Texture2D iconWindLeft = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_wind_left");
            Texture2D iconWindRight = game1.Content.Load<Texture2D>(@"Textures\PowerUps\icon_wind_right");

            saveTextureByName(texturePowerUpBomb, "PowerUp_Bomb");
            saveTextureByName(texturePowerUpLava, "PowerUp_Lava");
            saveTextureByName(texturePowerUpJump, "PowerUp_Jump");

            saveTextureByName(iconRain, "Powerup_BallRain");
            saveTextureByName(iconDouble, "Powerup_DoubleBall");
            saveTextureByName(iconInvert, "Powerup_InvertedControl");
            saveTextureByName(iconJump, "Powerup_Jumpheight");
            saveTextureByName(iconSun, "Powerup_SunsetSunrise");
            saveTextureByName(iconBomb, "Powerup_SuperBomb");
            saveTextureByName(iconWall, "Powerup_SwitchStones");
            saveTextureByName(iconVulcano, "Powerup_Volcano");
            saveTextureByName(iconWindLeft, "Powerup_WindLeft");
            saveTextureByName(iconWindRight, "Powerup_WindRight");

            #endregion

            #region Lights

            saveTextureByName(game1.Content.Load<Texture2D>(@"Textures/Light/light_large_highlight"), "light_large_highlight");
            saveTextureByName(game1.Content.Load<Texture2D>(@"Textures/Light/light_small_highlight"), "light_small_highlight");
            saveTextureByName(game1.Content.Load<Texture2D>(@"Textures/Light/light_smooth"), "light_smooth");
            saveTextureByName(game1.Content.Load<Texture2D>(@"Textures/Light/light"), "light");

            #endregion

        }



        #region Methods to load the HUD

        /// <summary>
        /// Creates all Scoreboard objects and adds them to the gameobjects
        /// </summary>
        public void LoadScoreboard(WinCondition winCondition)
        {

            // Get Game1 reference
            Game1 game1 = Game1.Instance;


            Texture2D scoreboardTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\scoreboard_background");

            Texture2D texture_digit_0 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_0");
            Texture2D texture_digit_1 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_1");
            Texture2D texture_digit_2 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_2");
            Texture2D texture_digit_3 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_3");
            Texture2D texture_digit_4 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_4");
            Texture2D texture_digit_5 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_5");
            Texture2D texture_digit_6 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_6");
            Texture2D texture_digit_7 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_7");
            Texture2D texture_digit_8 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_8");
            Texture2D texture_digit_9 = game1.Content.Load<Texture2D>(@"Textures\HUD\Digits\digit_9");

            Texture2D texture_matchball = game1.Content.Load<Texture2D>(@"Textures\HUD\matchball");

            // Explanation for y-Size: X-Size * proportion texture * proportion Screen
            HUD scoreboard = new HUD("Scoreboard", true, scoreboardTexture, null, new Vector2(0.27f, (0.27f * 0.2185f * 1.7778f)), new Vector2(0.365f, -0.02f), 73, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(scoreboard);

            HUD digit_left_1 = new HUD("Digit_left_1", true, texture_digit_0, null, new Vector2(0.02f, 0.06f), new Vector2(0.42f, 0.006f), 74, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(digit_left_1);
            HUD digit_left_2 = new HUD("Digit_left_2", true, texture_digit_0, null, new Vector2(0.02f, 0.06f), new Vector2(0.44f, 0.006f), 74, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(digit_left_2);
            HUD digit_right_1 = new HUD("Digit_right_1", true, texture_digit_0, null, new Vector2(0.02f, 0.06f), new Vector2(0.54f, 0.006f), 74, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(digit_right_1);
            HUD digit_right_2 = new HUD("Digit_right_2", true, texture_digit_0, null, new Vector2(0.02f, 0.06f), new Vector2(0.561f, 0.006f), 74, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(digit_right_2);

            HUD matchball = new HUD("matchball", false, texture_matchball, null, new Vector2(0.20f, 0.04f), new Vector2(0.4f, 0.2f), 74, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(matchball);

            if (winCondition is TimeLimit_WinCondition)
            {
                // Load clock
                Texture2D clock_backgroundTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\clock_background");
                Texture2D clock_highlightedHalfTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\clock_highlighted_Half");
                Texture2D clock_unhighlightedHalfTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\clock_unhighlighted_Half");
                Texture2D clock_borderTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\clock_border");

                HUD clock_background = new HUD("Clock_background", true, clock_backgroundTexture, null, new Vector2(0.06f, 0.06f * 1.7778f), new Vector2(0.47f, 0.01f), 75, MeasurementUnit.PercentOfScreen);
                SuperController.Instance.addGameObjectToGameInstance(clock_background);

                HUD clock_highlightedHalf = new HUD("Clock_highlightedHalf", true, clock_highlightedHalfTexture, null, new Vector2(0.06f, 0.06f * 1.7778f), new Vector2(0.47f, 0.01f), 76, MeasurementUnit.PercentOfScreen);
                SuperController.Instance.addGameObjectToGameInstance(clock_highlightedHalf);

                HUD clock_highlightedRightHalf = new HUD("Clock_highlightedRightHalf", true, clock_highlightedHalfTexture, null, new Vector2(0.06f, 0.06f * 1.7778f), new Vector2(0.47f, 0.01f), 77, MeasurementUnit.PercentOfScreen);
                clock_highlightedRightHalf.Rotation = (float)(Math.PI);
                SuperController.Instance.addGameObjectToGameInstance(clock_highlightedRightHalf);

                HUD clock_unhighlightedHalf = new HUD("Clock_unhighlightedHalf", true, clock_unhighlightedHalfTexture, null, new Vector2(0.06f, 0.06f * 1.7778f), new Vector2(0.47f, 0.01f), 77, MeasurementUnit.PercentOfScreen);
                SuperController.Instance.addGameObjectToGameInstance(clock_unhighlightedHalf);

                HUD clock_border = new HUD("Clock_border", true, clock_borderTexture, null, new Vector2(0.06f, 0.06f * 1.7778f), new Vector2(0.47f, 0.01f), 78, MeasurementUnit.PercentOfScreen);
                SuperController.Instance.addGameObjectToGameInstance(clock_border);

            }
            else
            {
                // Load Score Rect
                Texture2D scoreRect_unhighlightedTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\scoreRect_unhighlighted");
                Texture2D scoreRect_highlightedTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\scoreRect_highlighted");
                Texture2D scoreRect_borderTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\scoreRect_border");

                HUD scoreRect_unhighlighted = new HUD("ScoreRect_unhighlighted", true, scoreRect_unhighlightedTexture, null, new Vector2(0.055f, 0.05f * 1.7778f), new Vector2(0.471f, 0.020f), 75, 
                    MeasurementUnit.PercentOfScreen);
                SuperController.Instance.addGameObjectToGameInstance(scoreRect_unhighlighted);

                HUD scoreRect_highlighted = new HUD("ScoreRect_highlighted", true, scoreRect_highlightedTexture, null, new Vector2(0.055f, 0.05f * 1.7778f), new Vector2(0.471f, 0.020f), 76,
                    MeasurementUnit.PercentOfScreen);
                SuperController.Instance.addGameObjectToGameInstance(scoreRect_highlighted);

                HUD scoreRect_border = new HUD("ScoreRect_border", true, scoreRect_borderTexture, null, new Vector2(0.06f, 0.06f * 1.7778f), new Vector2(0.47f, 0.01f), 77,
                    MeasurementUnit.PercentOfScreen);
                SuperController.Instance.addGameObjectToGameInstance(scoreRect_border);

            }

        }

        /// <summary>
        /// Creates all Specialbar objects, adds them to the gameobjects
        /// and returns them to have a reference for later use
        /// </summary>
        /// <param name="levelSize">The size of the level to adjust the powerup texts</param>
        /// <returns>The references to the necessary HUD elements</returns>
        public LayerList<HUD> LoadSpecialbar(Vector2 levelSize)
        {

            LayerList<HUD> returnList = new LayerList<HUD>();

            // Get Game1 reference
            Game1 game1 = Game1.Instance;

            Texture2D specialbar_leftTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\specialbar_left");
            Texture2D specialbar_rightTexture = game1.Content.Load<Texture2D>(@"Textures\HUD\IngameHUD\specialbar_right");

            HUD specialbar_left = new HUD("specialbarBar_Left", true, specialbar_leftTexture, null, new Vector2(0.2f, 0.06f), new Vector2(0.23f, 0.011f), 72, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.addGameObjectToGameInstance(specialbar_left);
            returnList.Add(specialbar_left);

            HUD specialbar_right = new HUD("specialbarBar_Right", true, specialbar_leftTexture, null, new Vector2(0.2f, 0.06f), new Vector2(0.60f, 0.011f), 72, MeasurementUnit.PercentOfScreen);
            specialbar_right.FlipHorizontally = true;
            SuperController.Instance.addGameObjectToGameInstance(specialbar_right);
            returnList.Add(specialbar_right);

            // Make specialbars invisible when powerups are disabled
            if (!(bool)SettingsManager.Instance.get("enablePowerups"))
            {
                specialbar_left.Visible = false;
                specialbar_right.Visible = false;
            }


            // Add Powerup Texts to Specialbar
            Texture2D teamplay = game1.Content.Load<Texture2D>(@"Textures\HUD\teamplay");

            // PowerupText. We need one text for every team.
            float meterpixel = (float)SettingsManager.Instance.get("meterToPixel");

            //additional texts
            HUD text_teamplay_t1 = new HUD("teamplay_t1", false, teamplay, null, new Vector2(teamplay.Width, teamplay.Height) / meterpixel, new Vector2(levelSize.X / 4f, levelSize.Y / 4f), 70, Model.MeasurementUnit.Meter);
            HUD text_teamplay_t2 = new HUD("teamplay_t2", false, teamplay, null, new Vector2(teamplay.Width, teamplay.Height) / meterpixel, new Vector2(levelSize.X / 4f, levelSize.Y / 4f), 70, Model.MeasurementUnit.Meter);

            SuperController.Instance.addGameObjectToGameInstance(text_teamplay_t1);
            SuperController.Instance.addGameObjectToGameInstance(text_teamplay_t2);

            return returnList;

        }

        #endregion


        #region Methods to load the menu

        /// <summary>
        /// Loads all objects for MainMenu
        /// </summary>
        /// <returns>LayerList with all MainMenu objects</returns>
        public LayerList<LayerInterface> LoadMainMenuObjects()
        {

            // Load textures
            Texture2D Texture_MainMenu_Background = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainMenu_Background");
            Texture2D Texture_MainMenu_Background_Ball = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainMenu_Background_Ball");

            Texture2D Texture_MainScreen_1vs1 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_1vs1");
            Texture2D Texture_MainScreen_1vs1_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_1vs1_s");
            Texture2D Texture_MainScreen_2vs2 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_2vs2");
            Texture2D Texture_MainScreen_2vs2_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_2vs2_s");
            Texture2D Texture_MainScreen_Help = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_Help");
            Texture2D Texture_MainScreen_Help_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_Help_s");
            Texture2D Texture_MainScreen_Exit = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_Exit");
            Texture2D Texture_MainScreen_Exit_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\MainScreen_Exit_s");

            Texture2D Texture_SelectLevel_left = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Left");
            Texture2D Texture_SelectLevel_left_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Left_s");
            Texture2D Texture_SelectLevel_right = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Right");
            Texture2D Texture_SelectLevel_right_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Right_s");
            Texture2D Texture_SelectLevel_up_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Up_s");
            Texture2D Texture_SelectLevel_down_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Down_s");
            Texture2D Texture_SelectLevel_powerups = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Powerups");
            Texture2D Texture_SelectLevel_classic = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_Classic");
            Texture2D Texture_SelectLevel_15pts = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_15pts");
            Texture2D Texture_SelectLevel_30pts = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_30pts");
            Texture2D Texture_SelectLevel_3min = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_3min");
            Texture2D Texture_SelectLevel_5min = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Level_5min");

            Texture2D Texture_TeamMenu_Background2_1vs1 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_1vs1");
            Texture2D Texture_TeamMenu_Background2_2vs2 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_2vs2");
            Texture2D Texture_TeamMenu_Gamepad1 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_Gamepad_1");
            Texture2D Texture_TeamMenu_Gamepad2 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_Gamepad_2");
            Texture2D Texture_TeamMenu_Gamepad3 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_Gamepad_3");
            Texture2D Texture_TeamMenu_Gamepad4 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_Gamepad_4");
            Texture2D Texture_TeamMenu_Keyboard1 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_Keyboard1");
            Texture2D Texture_TeamMenu_Keyboard2 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_Keyboard2");
            Texture2D Texture_TeamMenu_KI = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_KI");
            Texture2D Texture_TeamMenu_Gamepad_Deactivated = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\TeamMenu_Gamepad_Deactivated");

            Texture2D Texture_Help_Box1 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\help_Box1");
            Texture2D Texture_Help_Box2 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\help_Box2");
            Texture2D Texture_Help_Box3 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\help_Box3");
            Texture2D Texture_Help_CreditsBox = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\help_creditsBox");
            Texture2D Texture_Help_RoundButton = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\help_roundButton");
            Texture2D Texture_Help_RoundButton_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\help_roundButton_s");

            Texture2D Texture_ReallyQuit_Frame = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\ReallyQuit_Frame");
            Texture2D Texture_ReallyQuit_Frame2 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\ReallyQuit_Frame2");
            Texture2D Texture_ReallyQuit_Head = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\ReallyQuit_Head");
            Texture2D Texture_ReallyQuit_Yes = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\ReallyQuit_Yes");
            Texture2D Texture_ReallyQuit_Yes_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\ReallyQuit_Yes_s");
            Texture2D Texture_ReallyQuit_No = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\ReallyQuit_No");
            Texture2D Texture_ReallyQuit_No_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\ReallyQuit_No_s");

            Texture2D Texture_MainScreen_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Overlay_choose_select");
            Texture2D Texture_ReallyQuit_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Overlay_choose_select_back");
            Texture2D Texture_Help_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Overlay_browse_back");
            Texture2D Texture_TeamMenu_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Overlay_choose_continue_back");
            Texture2D Texture_TeamMenuWarning1_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Overlay_noTeam");
            Texture2D Texture_TeamMenuWarning2_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Overlay_tooManyTeam");
            Texture2D Texture_SelectLevel_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\MainMenu\Overlay_choose_startGame_back");

            // Create objects
            LayerList<LayerInterface> objectList = new LayerList<LayerInterface>();

            // Main Screen
            MenuObject Passive_MainMenu_Background = new MenuObject("MainMenu_Background", true,
                Texture_MainMenu_Background, null, new Vector2(1, 1), Vector2.Zero, 91, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_Background);

            MenuObject Passive_MainMenu_Background_Ball = new MenuObject("MainMenu_Background_Ball", true,
                Texture_MainMenu_Background_Ball, null, new Vector2(1, 1), Vector2.Zero, 91, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_Background_Ball);

            MenuButtonObject Button_MainMenu_MainScreen_1vs1 = new MenuButtonObject("MainScreen_1vs1", true, Texture_MainScreen_1vs1,
                Texture_MainScreen_1vs1_s, null, new Vector2(504f/1280f, 702f/720f), new Vector2(2f/1280f, -10f/720f), 92, MeasurementUnit.PercentOfScreen);//2 - -10
            objectList.Add(Button_MainMenu_MainScreen_1vs1);

            MenuButtonObject Button_MainMenu_MainScreen_2vs2 = new MenuButtonObject("MainScreen_2vs2", true, Texture_MainScreen_2vs2,
                Texture_MainScreen_2vs2_s, null, new Vector2(504f/1280f, 702f/720f), new Vector2(783f/1280f, 30f/720f), 92, MeasurementUnit.PercentOfScreen);// 783 - 17
            objectList.Add(Button_MainMenu_MainScreen_2vs2);

            MenuButtonObject Button_MainMenu_MainScreen_Help = new MenuButtonObject("MainScreen_Help", true, Texture_MainScreen_Help,
                Texture_MainScreen_Help_s, null, new Vector2(405f/1280f, 161f/720f), new Vector2(435/1280f, 47f/720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_MainScreen_Help);

            MenuButtonObject Button_MainMenu_MainScreen_Exit = new MenuButtonObject("MainScreen_Exit", true, Texture_MainScreen_Exit,
                Texture_MainScreen_Exit_s, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435/1280f, 507f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_MainScreen_Exit);


            // Select Level

            MenuObject Passive_SelectLevel_Left = new MenuObject("SelectLevel_left", true,
                Texture_SelectLevel_left, null, new Vector2(161f / 1280f, 405f / 720f), new Vector2(330f / 1280f, 156f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Left);
            MenuObject Passive_SelectLevel_Left_s = new MenuObject("SelectLevel_left_s", false,
                Texture_SelectLevel_left_s, null, new Vector2(161f / 1280f, 405f / 720f), new Vector2(330f / 1280f, 156f / 720f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Left_s);

            MenuObject Passive_SelectLevel_Right = new MenuObject("SelectLevel_right", true,
                Texture_SelectLevel_right, null, new Vector2(161f / 1280f, 405f / 720f), new Vector2(786f / 1280f, 156f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Right);
            MenuObject Passive_SelectLevel_Right_s = new MenuObject("SelectLevel_right_s", false,
                Texture_SelectLevel_right_s, null, new Vector2(161f / 1280f, 405f / 720f), new Vector2(786f / 1280f, 156f / 720f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Right_s);

            MenuObject Passive_SelectLevel_Up_s = new MenuObject("SelectLevel_up_s", false,
                Texture_SelectLevel_up_s, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 47f / 720f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Up_s);
            MenuObject Passive_SelectLevel_Down_s = new MenuObject("SelectLevel_down_s", false,
                Texture_SelectLevel_down_s, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 507f / 720f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Down_s);

            MenuObject Passive_SelectLevel_Powerups = new MenuObject("SelectLevel_powerups", true,
                Texture_SelectLevel_powerups, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 47f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Powerups);
            MenuObject Passive_SelectLevel_Classic = new MenuObject("SelectLevel_classic", true,
                Texture_SelectLevel_classic, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 47f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_Classic);

            MenuObject Passive_SelectLevel_15pts = new MenuObject("SelectLevel_15pts", true,
                Texture_SelectLevel_15pts, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 507f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_15pts);
            MenuObject Passive_SelectLevel_30pts = new MenuObject("SelectLevel_30pts", true,
                Texture_SelectLevel_30pts, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 507f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_30pts);
            MenuObject Passive_SelectLevel_3min = new MenuObject("SelectLevel_3min", true,
                Texture_SelectLevel_3min, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 507f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_3min);
            MenuObject Passive_SelectLevel_5min = new MenuObject("SelectLevel_5min", true,
                Texture_SelectLevel_5min, null, new Vector2(405f / 1280f, 161f / 720f), new Vector2(435f / 1280f, 507f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_SelectLevel_5min);

            // Team Menu

            MenuObject Passive_TeamMenu_Background2_1vs1 = new MenuObject("TeamMenu_Background2_1vs1", false,
                Texture_TeamMenu_Background2_1vs1, null, new Vector2(1f, 1f), Vector2.Zero, 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_TeamMenu_Background2_1vs1);
            MenuObject Passive_TeamMenu_Background2_2vs2 = new MenuObject("TeamMenu_Background2_2vs2", false,
                Texture_TeamMenu_Background2_2vs2, null, new Vector2(1f, 1f), Vector2.Zero, 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_TeamMenu_Background2_2vs2);

            MenuObject Passive_MainMenu_TeamMenu_Gamepad1 = new MenuObject("TeamMenu_Gamepad1", false,
                Texture_TeamMenu_Gamepad1, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad1);
            MenuObject Passive_MainMenu_TeamMenu_Gamepad2 = new MenuObject("TeamMenu_Gamepad2", false,
                Texture_TeamMenu_Gamepad2, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.37f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad2);
            MenuObject Passive_MainMenu_TeamMenu_Gamepad3 = new MenuObject("TeamMenu_Gamepad3", false,
                Texture_TeamMenu_Gamepad3, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.55f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad3);
            MenuObject Passive_MainMenu_TeamMenu_Gamepad4 = new MenuObject("TeamMenu_Gamepad4", false,
                Texture_TeamMenu_Gamepad4, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.73f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad4);

            MenuObject Passive_MainMenu_TeamMenu_Keyboard1 = new MenuObject("TeamMenu_Keyboard1", false,
                Texture_TeamMenu_Keyboard1, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.73f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Keyboard1);
            MenuObject Passive_MainMenu_TeamMenu_Keyboard2 = new MenuObject("TeamMenu_Keyboard2", false,
                Texture_TeamMenu_Keyboard2, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.73f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Keyboard2);

            MenuObject Passive_MainMenu_TeamMenu_Gamepad_Deactivated1 = new MenuObject("TeamMenu_Gamepad_Deactivated1", false,
                Texture_TeamMenu_Gamepad_Deactivated, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad_Deactivated1);
            MenuObject Passive_MainMenu_TeamMenu_Gamepad_Deactivated2 = new MenuObject("TeamMenu_Gamepad_Deactivated2", false,
                Texture_TeamMenu_Gamepad_Deactivated, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad_Deactivated2);
            MenuObject Passive_MainMenu_TeamMenu_Gamepad_Deactivated3 = new MenuObject("TeamMenu_Gamepad_Deactivated3", false,
                Texture_TeamMenu_Gamepad_Deactivated, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad_Deactivated3);
            MenuObject Passive_MainMenu_TeamMenu_Gamepad_Deactivated4 = new MenuObject("TeamMenu_Gamepad_Deactivated4", false,
                Texture_TeamMenu_Gamepad_Deactivated, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_TeamMenu_Gamepad_Deactivated4);

            MenuObject Passive_MainMenu_TeamMenu_KI1 = new MenuObject("TeamMenu_KI1", false,
                Texture_TeamMenu_KI, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            Passive_MainMenu_TeamMenu_KI1.Alpha = 0.5f;
            objectList.Add(Passive_MainMenu_TeamMenu_KI1);
            MenuObject Passive_MainMenu_TeamMenu_KI2 = new MenuObject("TeamMenu_KI2", false,
                Texture_TeamMenu_KI, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            Passive_MainMenu_TeamMenu_KI2.Alpha = 0.5f;
            objectList.Add(Passive_MainMenu_TeamMenu_KI2);
            MenuObject Passive_MainMenu_TeamMenu_KI3 = new MenuObject("TeamMenu_KI3", false,
                Texture_TeamMenu_KI, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            Passive_MainMenu_TeamMenu_KI3.Alpha = 0.5f;
            objectList.Add(Passive_MainMenu_TeamMenu_KI3);
            MenuObject Passive_MainMenu_TeamMenu_KI4 = new MenuObject("TeamMenu_KI4", false,
                Texture_TeamMenu_KI, null, new Vector2(0.10f, 0.15f), new Vector2(0.45f, 0.24f), 93, MeasurementUnit.PercentOfScreen);
            Passive_MainMenu_TeamMenu_KI4.Alpha = 0.5f;
            objectList.Add(Passive_MainMenu_TeamMenu_KI4);
            // Help

            MenuObject Passive_MainMenu_Help_Box1 = new MenuObject("Help_Box1", false,
                Texture_Help_Box1, null, new Vector2(1000f / 1280f, 563f / 720f), new Vector2(140f / 1280f, 50f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_Help_Box1);

            MenuObject Passive_MainMenu_Help_Box2 = new MenuObject("Help_Box2", false,
                Texture_Help_Box2, null, new Vector2(1000f / 1280f, 563f / 720f), new Vector2(140f / 1280f, 50f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_Help_Box2);

            MenuObject Passive_MainMenu_Help_Box3 = new MenuObject("Help_Box3", false,
                Texture_Help_Box3, null, new Vector2(1000f / 1280f, 563f / 720f), new Vector2(140f / 1280f, 50f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_Help_Box3);

            MenuObject Passive_MainMenu_Help_CreditsBox = new MenuObject("Help_CreditsBox", false,
                Texture_Help_CreditsBox, null, new Vector2(1000f / 1280f, 563f / 720f), new Vector2(140f / 1280f, 50f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_Help_CreditsBox);

            MenuButtonObject Button_MainMenu_Help_Button1 = new MenuButtonObject("Help_Button1", false, Texture_Help_RoundButton,
                Texture_Help_RoundButton_s, null, new Vector2(0.03f, 0.05f), new Vector2(0.35f - 0.015f, 0.92f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_Help_Button1);
            MenuButtonObject Button_MainMenu_Help_Button2 = new MenuButtonObject("Help_Button2", false, Texture_Help_RoundButton,
                Texture_Help_RoundButton_s, null, new Vector2(0.03f, 0.05f), new Vector2(0.45f - 0.015f, 0.92f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_Help_Button2);
            MenuButtonObject Button_MainMenu_Help_Button3 = new MenuButtonObject("Help_Button3", false, Texture_Help_RoundButton,
                Texture_Help_RoundButton_s, null, new Vector2(0.03f, 0.05f), new Vector2(0.55f - 0.015f, 0.92f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_Help_Button3);
            MenuButtonObject Button_MainMenu_Help_Button4 = new MenuButtonObject("Help_Button4", false, Texture_Help_RoundButton,
                Texture_Help_RoundButton_s, null, new Vector2(0.03f, 0.05f), new Vector2(0.65f - 0.015f, 0.92f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_Help_Button4);


            // Really Quit
            MenuObject Passive_MainMenu_ReallyQuit_Frame = new MenuObject("ReallyQuit_Frame", false,
                Texture_ReallyQuit_Frame, null, new Vector2(1f, 1f), Vector2.Zero, 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_ReallyQuit_Frame);

            MenuObject Passive_MainMenu_ReallyQuit_Head = new MenuObject("ReallyQuit_Head", false,
                Texture_ReallyQuit_Head, null, new Vector2(508f / 1280f, 126f / 720f), new Vector2(388f / 1280f, 169f / 720f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_ReallyQuit_Head);

            MenuButtonObject Button_MainMenu_ReallyQuit_Yes = new MenuButtonObject("ReallyQuit_Yes", false, Texture_ReallyQuit_Yes,
                Texture_ReallyQuit_Yes_s, null, new Vector2(506f / 1280f, 125f / 720f), new Vector2(389f / 1280f, 295f / 720f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_ReallyQuit_Yes);
            MenuButtonObject Button_MainMenu_ReallyQuit_No = new MenuButtonObject("ReallyQuit_No", false, Texture_ReallyQuit_No,
            Texture_ReallyQuit_No_s, null, new Vector2(508f / 1280f, 126f / 720f), new Vector2(388f / 1280f, 420f / 720f), 93, MeasurementUnit.PercentOfScreen);
            objectList.Add(Button_MainMenu_ReallyQuit_No);

            MenuObject Passive_MainMenu_ReallyQuit_Frame2 = new MenuObject("ReallyQuit_Frame2", false,
                Texture_ReallyQuit_Frame2, null, new Vector2(1f, 1f), Vector2.Zero, 94, MeasurementUnit.PercentOfScreen);
            objectList.Add(Passive_MainMenu_ReallyQuit_Frame2);


            // Overlays

            MenuObject MainScreen_Overlay = new MenuObject("MainScreen_Overlay", false,
                Texture_MainScreen_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(MainScreen_Overlay);

            MenuObject ReallyQuit_Overlay = new MenuObject("ReallyQuit_Overlay", false,
                Texture_ReallyQuit_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(ReallyQuit_Overlay);

            MenuObject Help_Overlay = new MenuObject("Help_Overlay", false,
                Texture_Help_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(Help_Overlay);

            MenuObject TeamMenu_Overlay = new MenuObject("TeamMenu_Overlay", false,
                Texture_TeamMenu_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(TeamMenu_Overlay);

            MenuObject TeamMenuWarning1_Overlay = new MenuObject("TeamMenuWarning1_Overlay", false,
                Texture_TeamMenuWarning1_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(TeamMenuWarning1_Overlay);
            MenuObject TeamMenuWarning2_Overlay = new MenuObject("TeamMenuWarning2_Overlay", false,
                Texture_TeamMenuWarning2_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(TeamMenuWarning2_Overlay);

            MenuObject SelectLevel_Overlay = new MenuObject("SelectLevel_Overlay", false,
                Texture_SelectLevel_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(SelectLevel_Overlay);

            // return list with all objects
            return objectList;
        }



        /// <summary>
        /// Loads the level preview pictures
        /// </summary>
        /// <param name="levelList">List with all the loadable levels</param>
        /// <returns>a list with all the preview pictures for the loadable levels</returns>
        public LayerList<LayerInterface> LoadLevelPreviews(List<string> levelList)
        {

            LayerList<LayerInterface> levelPreviewPictures = new LayerList<LayerInterface>();

            foreach (string levelName in levelList)
            {

                Texture2D texture = Game1.Instance.Content.Load<Texture2D>(@"Textures\Levels\" + 
                    levelName + "\\levelPreview");

                MenuButtonObject levelPreview = new MenuButtonObject(levelName, false, texture, texture, null, 
                    new Vector2(0.08f, 0.15f), new Vector2(0.46f, 0.25f), 94, MeasurementUnit.PercentOfScreen);

                levelPreviewPictures.Add(levelPreview);
            }

            return levelPreviewPictures;

        }

        /// <summary>
        /// Loads all objects for IngameMenu
        /// </summary>
        /// <returns>LayerList with all Ingame objects</returns>
        public LayerList<LayerInterface> LoadIngameMenuObjects()
        {
            // Load textures
            Texture2D Texture_IngameMenu_Back = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Back");
            Texture2D Texture_IngameMenu_Back_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Back_s");
            Texture2D Texture_IngameMenu_Quit = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Quit");
            Texture2D Texture_IngameMenu_Quit_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Quit_s");
            Texture2D Texture_IngameMenu_Restart = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Restart");
            Texture2D Texture_IngameMenu_Restart_s = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Restart_s");
            Texture2D Texture_Background = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Background");
            Texture2D Texture_Frame = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_frame");

            Texture2D Texture_IngameMenu_Winner_Team1 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Winner_team1");
            Texture2D Texture_IngameMenu_Winner_Team2 = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\IngameMenu_Winner_team2");

            Texture2D Texture_IngameMenu_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\Overlay_choose_select_back");
            Texture2D Texture_WinnerScreen_Overlay = Game1.Instance.Content.Load<Texture2D>(@"Textures\Menu\IngameMenu\Overlay_mainmenu_revenge");

            // Create objects
            LayerList<LayerInterface> objectList = new LayerList<LayerInterface>();

            // Create ingame menu
            MenuButtonObject IngameMenu_Button_Restart = new MenuButtonObject("Button_Restart", true, Texture_IngameMenu_Restart,
                Texture_IngameMenu_Restart_s, null, new Vector2(508f / 1280f, 126f / 720f), new Vector2(388f / 1280f, 169f / 720f), 91, MeasurementUnit.PercentOfScreen);
            objectList.Add(IngameMenu_Button_Restart);
            MenuButtonObject IngameMenu_Button_Quit = new MenuButtonObject("Button_Quit", true, Texture_IngameMenu_Quit,
                Texture_IngameMenu_Quit_s, null, new Vector2(506f / 1280f, 125f / 720f), new Vector2(389f / 1280f, 295f / 720f), 91, MeasurementUnit.PercentOfScreen);
            objectList.Add(IngameMenu_Button_Quit);
            MenuButtonObject IngameMenu_Button_Back = new MenuButtonObject("Button_Back", true, Texture_IngameMenu_Back,
                Texture_IngameMenu_Back_s, null, new Vector2(508f / 1280f, 126f / 720f), new Vector2(388f / 1280f, 420f / 720f), 91, MeasurementUnit.PercentOfScreen);
            objectList.Add(IngameMenu_Button_Back);

            MenuObject IngameMenu_Passive_Background = new MenuObject("Background", true,
                Texture_Background, null, new Vector2(1f, 1f), Vector2.Zero, 90, MeasurementUnit.PercentOfScreen);
            objectList.Add(IngameMenu_Passive_Background);

            MenuObject IngameMenu_Passive_Frame = new MenuObject("Frame", true,
                Texture_Frame, null, new Vector2(1f, 1f), Vector2.Zero, 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(IngameMenu_Passive_Frame);

            // Create winner screen
            MenuObject WinnerScreen_Winner_Team1 = new MenuObject("WinnerScreen_Winner_Team1", false,
                Texture_IngameMenu_Winner_Team1, null, new Vector2(446f/1280f, 392f/720f), new Vector2(443f / 1280f, 180f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(WinnerScreen_Winner_Team1);
            MenuObject WinnerScreen_Winner_Team2 = new MenuObject("WinnerScreen_Winner_Team2", false,
                Texture_IngameMenu_Winner_Team2, null, new Vector2(446f / 1280f, 392f / 720f), new Vector2(443f / 1280f, 180f / 720f), 92, MeasurementUnit.PercentOfScreen);
            objectList.Add(WinnerScreen_Winner_Team2);


            // Create Overlays

            MenuObject IngameMenu_Overlay = new MenuObject("IngameMenu_Overlay", false,
                Texture_IngameMenu_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(IngameMenu_Overlay);

            MenuObject WinnerScreen_Overlay = new MenuObject("WinnerScreen_Overlay", false,
                Texture_WinnerScreen_Overlay, null, new Vector2(1280f / 1280f, 71f / 720f), new Vector2(0f / 1280f, 649f / 720f), 95, MeasurementUnit.PercentOfScreen);
            objectList.Add(WinnerScreen_Overlay);

            // Return list with all objects
            return objectList;

        }

        #endregion

        /// <summary>
        /// Returns the texture for a given name
        /// </summary>
        /// <param name="name">name of the texture</param>
        /// <returns>the texture</returns>
        public Texture2D getTextureByName(String name)
        {
            return textures[name];
        }

        /// <summary>
        /// Checks if texture is already in dictionary
        /// if not, adds it
        /// </summary>
        /// <param name="texture">texture that should be saved</param>
        /// <param name="name">name of the texture</param>
        public void saveTextureByName(Texture2D texture, String name)
        {
            if (name.Length == 0)
                return;

            if (!textures.ContainsKey(name))
                textures.Add(name, texture);

            return;
        }





        #region HelperMethods


        #endregion

    #endregion

    }
}
