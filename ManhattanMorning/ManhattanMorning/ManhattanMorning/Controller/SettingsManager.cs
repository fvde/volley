using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManhattanMorning.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ManhattanMorning.Controller
{
    class SettingsManager : ObservableObject
    {
               
        /////////////////
        //Settings
        ////////////////
        // please insert new Settings like this: new object[]{minValue, defaultValue, maxValue}
        // supported values of Ingame-SettingsManager are : int, float, Vector2, Vector3
        // if you want to set another value, just insert objec[]{null, value, null}
        // (notice: can't be changed InGame)
        void setStandardSettings()
        {
            #region General

            // Defines in which GameState the game should start
            // Used in Game1.cs
            // To Display the Menu
            // settings.Add("StartInGameState", new object[] {null, new object[] { GSLevel1.Menu, GSLevel2.MainMenu, GSLevel3.Main}, null});

            /// <summary>
            /// Ultimate Answer to the Ultimate Question of Life, The Universe, and Everything
            /// </summary>
            settings.Add("ultimateAnswer", new object[] {42, 42, 42});

            /// <summary>
            /// Factor to convert Meter to Pixel
            /// </summary>
            settings.Add("meterToPixel", new object[] {0f, 100.0f, 1000f});

            /// <summary>
            /// The resize factor if it's a 2vs2 game
            /// </summary>
            settings.Add("ResizeFactor2vs2Game", new object[] { 0.1f, 1.2f, 4.0f });

            /// <summary>
            /// Play intro video?
            /// </summary>
            settings.Add("IntroVideo", new object[] { false, false, true });

            /// <summary>
            /// Play level video?
            /// </summary>
            settings.Add("LevelVideo", new object[] { false, true, true });

            /// <summary>
            /// True if a game between KI players should be allowed
            /// </summary>            
            settings.Add("AllowKIOnlyGame", new object[] { false, false, false });

            /// <summary>
            /// True if the game runs as trial (is set in game1 initialize)
            /// </summary>                  
            settings.Add("IsTrialMode", new object[] { false, false, true });

            #endregion


            #region Objects

            // The size of the ball
            // (Is overriden by StorageManager!)
            settings.Add("ballSize", new object[] { new Vector2(0.1f, 0.1f), new Vector2(1.0f, 1.0f), new Vector2(5f, 5f) });

            // The size of a neutral powerup
            // (Is overriden by StorageManager!)
            settings.Add("neutralPowerupSize", new object[] { new Vector2(0.1f, 0.1f), new Vector2(1.0f, 1.0f), new Vector2(5f, 5f) });

            // The size of a positive powerup
            // (Is overriden by StorageManager!)
            settings.Add("positivePowerupSize", new object[] { new Vector2(0.1f, 0.1f), new Vector2(1.0f, 1.0f), new Vector2(5f, 5f) });

            // The size of the players hand
            settings.Add("playerHandRatio", new object[] { 0.01f, 0.6f, 2.0f });

            // The size of the players hand (diameter!).
            settings.Add("playerSize", new object[] { 0.1f, 1.2f, 3.0f });

            #endregion


            #region Graphics

            // ScreenResolution of whole game
            settings.Add("screenResolution", new object[] { new Vector2(640, 480), new Vector2(1280, 720), new Vector2(1920, 1080) });

            //Show Bounding Boxes
            settings.Add("showBoundingBoxes", new object[] { false, false, true });

            //Show Grid
            settings.Add("showGrid", new object[] { false, false, true });

            //camera Zoom Level
            settings.Add("cameraZoomLevel", new object[] { 0.0f, 1.0f, 5.0f });

            //camera Position
            settings.Add("cameraPosition", new object[] { new Vector2(-2000,-2000),new Vector2(640,360) ,new Vector2(2000,2000)});

            // The duration (in ms) during which the player is highlighted when someone picks up a powerup
            settings.Add("PowerupHighlightPlayerDuration", new object[] { 100, 1200, 10000 });

            // The duration (in ms) of the night in forest level. After this time the sunrise will be started.
            settings.Add("PowerupSunsetDuration", new object[] { 1000, 7000, 15000 });


            #endregion


            #region Input

            // The waiting time until the player is allowed to make the next jump (in ms)
            // Used in InputManager
            settings.Add("timeBetweenJumps", new object[] { 1, 50, 1000 });

            // The time until the next item in a scrollable list is selected (in ms)
            // Used in ImputManager
            settings.Add("scrollWaitingTime", new object[] { 1, 200, 1000 });

            #endregion


            #region Physics

            // Gravity. Vector2. Used in Physics.cs.
            settings.Add("gravity", new object[] {new Vector2(-10f, -10f), new Vector2(0.0f, 9.0f), new Vector2(10f, 15f)});

            // Maximal Y Velocity when bouncing off the ground. The point of this value is to prevent players from adding up jump forces.
            // 0.0f: No bounce
            // 15.0f: Nearly no restriction
            settings.Add("maximumYBounceVelocity", new object[] { 0f, 3.0f, 15.0f });

            // Scalar that determines the maximum moving-speed of the player in meters/second (triggered through gamepad events). Float. Used in Physics.cs.
            settings.Add("movementSpeedScalar", new object[] {0f, 8.0f, 20f});

            // Scalar that indicates how fast you can move down.
            settings.Add("downMovementScalar", new Object[] { 0f, 0.5f, 3.0f });

            // Scalar that determines the impact of the impulse on a player's hand (triggered through gamepad events). Float. Used in Physics.cs.
            settings.Add("handImpulseScalar", new object[] {0f, 1.6f, 10.0f});

            // Jump-force of Player. Vector2. Influences how high a player can jump. y-part has to be < 0 for jumping upwards.
            settings.Add("jumpForce1vs1", new object[] {new Vector2(0, -50.0f), new Vector2(0, -11.9f), new Vector2(0, 0)}); //13.2

            // Jump-force of Player. Vector2. Influences how high a player can jump. y-part has to be < 0 for jumping upwards.
            settings.Add("jumpForce2vs2", new object[] { new Vector2(0, -50.0f), new Vector2(0, -13.7f), new Vector2(0, 0) }); //14.8

            // Restitution determines how great the counteracting force on collision is. (Restitution = Rückgabe)
            // Friction determines the changes to an objects attributes when colliding. (Friction = Reibung)
            // Mass determines (for example) how great the effect of forces on a object is.
            // Player object attributes
            settings.Add("playerRestitution", new object[] { 0f, 0.3f, 1.0f }); 
            settings.Add("playerFriction", new object[] {0f, 0.5f, 1.0f}); //0.5f
            settings.Add("playerMass", new object[] {0f, 1.5f, 2.0f});           

            // Hand object attributes
            settings.Add("handRestitution", new object[] { 0f, 0.2f, 1.0f }); 
            settings.Add("handFriction", new object[] { 0f, 0.5f, 1.0f }); //0.5f

            // Ball object attributes
            settings.Add("ballRestitution", new object[] { 0f, 0.3f, 1.0f }); 
            settings.Add("ballFriction", new object[] { 0f, 0.5f, 1.0f }); //0.5f
            settings.Add("ballMass", new object[] {0f, 0.22f, 5f}); 

            // Net object attributes (Static, no mass needed)
            settings.Add("netRestitution", new object[] { 0f, 0.3f, 1.0f }); // 0.8f
            settings.Add("netFriction", new object[] { 0f, 0.5f, 1.0f }); //0.0f

            // Powerup object attributes
            settings.Add("powerupRestitution", new object[] { 0f, 1.0f, 1.0f });
            settings.Add("powerupFriction", new object[] { 0f, 0.7f, 1.0f });
            settings.Add("powerupMass", new object[] {0f, 1.0f, 10f});
            
            // Level Borders (Static, no mass needed), bottom Border not influenced
            settings.Add("borderRestitution", new object[] { 0f, 0.2f, 1f }); //0.5f
            settings.Add("borderFriction", new object[] { 0f, 0.0f, 1f });


            #endregion


            #region PowerUps

            // Allows user to switch powerups on/off
            // Used in GameLogic
            settings.Add("enablePowerups", new object[] { false, true, true });

            // Defines how much spawning times can vary from their original value (0 - 1)
            // Used in GameLogic
            settings.Add("maxSpreading_spawnTimes", new object[] { 0f, 0.5f, 1f });

            // Defines after how much time a new neutral Powerups spawns (in ms)
            // Used in GameLogic
            settings.Add("averageSpawnTime_neutralPowerup", new object[] { 1000, 35000, 60000 });

            // Defines how fast the specialbar fills if you perform a positive action
            settings.Add("PositivePowerupsMultiplier", new object[] { 1.0f, 1.0f, 10.0f });

            // Default duration of powerUps. Required to display icons for a while.
            settings.Add("defaultEffectDuration", new object[] { 0, 3000, 0 });

            #region Jumpheight

            // Increased jumpheight factor
            // Used in GameLogic
            settings.Add("increasedJumpHeight", new object[] { 1.0f, 1.17f, 3.0f });

            // Duration of this powerUp
            settings.Add("jumpheightEffectDuration", new object[] { 0, 15000, 0 });

            #endregion

            #region Vulcano

            // The size of bombs
            settings.Add("lavaSize", new object[] { new Vector2(0.1f, 0.1f), new Vector2(0.8f, 0.8f), new Vector2(5f, 5f) });

            // The range of bombs
            settings.Add("lavaRange", new object[] { 0.0f, 2.0f, 30.0f });

            // The impact of bombs
            settings.Add("lavaImpact", new object[] { 1.0f, 2.0f, 1000.0f });

            // The stun duration of bombs in ms
            settings.Add("lavaStunDuration", new object[] { 100, 1000, 5000 });

            // Number of lava stones that spawn during the powerup
            settings.Add("lavaAmount", new object[] { 1, 5, 10 });

            // Time between lava spawns
            settings.Add("lavaTime", new object[] { 500, 1000, 5000 });

            // Duration of this powerUp
            settings.Add("volcanoEffectDuration", new object[] { 0, 6000, 20000 });

            #endregion

            #region SuperBomb

            // The size of bombs
            settings.Add("superBombSize", new object[] { new Vector2(0.1f, 0.1f), new Vector2(1.6f, 1.6f), new Vector2(5f, 5f) });

            // The range of bombs
            settings.Add("superBombRange", new object[] { 0.0f, 4.0f, 30.0f });

            // The impact of bombs
            settings.Add("superBombImpact", new object[] { 1.0f, 5.0f, 1000.0f });

            // The time until the bomb explodes
            settings.Add("superBombDuration", new object[] { 100, 9000, 50000 });

            // The bombs mass
            settings.Add("superBombMass", new object[] { 0.1f, 0.2f, 1000.0f });

            // The stun duration of bombs in ms
            settings.Add("superBombStunDuration", new object[] { 100, 4000, 5000 });

            #endregion

            #region Wind

            // Strength of the wind
            settings.Add("windStrength", new object[] { new Vector2(0.5f, 0.0f), new Vector2(2.8f, 0.8f), new Vector2(5.0f, 5.0f) });

            // Duration of this powerUp
            settings.Add("windEffectDuration", new object[] { 0, 15000, 0 });

            #endregion

            #region InvertedControl

            // Duration of this powerUp
            settings.Add("invertedControlEffectDuration", new object[] { 0, 8000, 0 });

            #endregion

            #region SwitchStones

            // Duration of this powerUp
            settings.Add("switchStonesEffectDuration", new object[] { 0, 12000, 0 });

            #endregion

            #region SunsetSunrise

            // Duration of this powerUp
            settings.Add("sunsetSunriseEffectDuration", new object[] { 0, 30000, 0 });

            #endregion

            #region BallRain

            // Number of balls that spawn during the ballrain
            settings.Add("ballRainAmount", new object[] { 1, 3, 5 });

            // Time between ballrain spawns
            settings.Add("ballRainTime", new object[] { 1000, 5000, 20000 });

            // Duration of this powerUp
            settings.Add("ballRainEffectDuration", new object[] { 0, 12000, 0 });

            #endregion

            #endregion


            #region Specialbar

            // Initial x-size (in percent of screen) for left and right specialbar
            settings.Add("initialSpecialbarSize", new object[] { new Vector2(0, 0), new Vector2(0.20f, 0.20f), new Vector2(1, 1) });

            // Value that defines the intensity of the specialbar length change when the score changes
            settings.Add("resizeIntensitySpecialbar", new object[] { 1.0f, 5.0f, 10.0f });

            // The value that is necessary to fill the specialbar in its initial size
            // This parameters allows to adjust the amount of filling to get a positive powerup
            settings.Add("ValueToFillInitialSpecialbar", new object[] { 1.0f, 10.0f, 100.0f });

            // The number of specialbar filling tiles that is necessary to fill the specialbar in its initial size
            // This parameters allows to adjust the size of the tiles (just for graphical representation)
            settings.Add("NumberOfSpecialBarFillingTilesToFillInitialSpecialbar", new object[] { 1, 10, 100 });

            // The gap size between the filling tiles (in percent of screen)
            settings.Add("GapSizeBetweenFillingTiles", new object[] { -1.0f, -0.012f, 1.0f });

            // The speed whith which the specialbar changes its size (in percent per 10ms)
            settings.Add("specialbarSizeChangeAnimationSpeed", new object[] { 0.001f, 0.05f, 0.1f });

            // The speed whith which the specialbar filling changes its size (in value per 10ms)
            settings.Add("specialbarFillingChangeAnimationSpeed", new object[] { 0.01f, 0.1f, 1.0f });


            #endregion


            #region Sound

            //Sound volume of sound effects
            settings.Add("soundVolume", new object[] { 0.0f, 0.75f, 1.0f });

            //Sound volume of extra jump height effects
            settings.Add("jumpPowerUpSoundVolume", new object[] { 0.0f, 0.25f, 1.0f });

            //Sound volume of extra jump height effects
            settings.Add("hitBallSoundVolume", new object[] { 0.0f, 0.25f, 1.0f });
            
            //Sound volume of hitNet sound effects
            settings.Add("hitNetSoundVolume", new object[] { 0.0f, 0.25f, 1.0f });

            //Sound volume of picking up powerUp
            settings.Add("pickupPowerUpSoundVolume", new object[] { 0.0f, 0.2f, 1.0f });

            //Sound volume of BigExplosion sound effects
            settings.Add("bigExplosionSoundVolume", new object[] { 0.0f, 0.4f, 1.0f });

            //Sound volume of small Explosion sound effects
            settings.Add("smallExplosionSoundVolume", new object[] { 0.0f, 0.25f, 1.0f });

            //Sound volume of inverted control sound effects
            settings.Add("invertedControlSoundVolume", new object[] { 0.0f, 0.25f, 1.0f });

            //Sound volume of smash ball sound effects
            settings.Add("smashBallSoundVolume", new object[] { 0.0f, 0.5f, 1.0f });

            //Sound volume of Sunset sound effects
            settings.Add("sunsetPowerUpSoundVolume", new object[] { 0.0f, 0.2f, 1.0f });

            //Sound volume of wind sound effects
            settings.Add("windPowerUpSoundVolume", new object[] { 0.0f, 0.2f, 1.0f });

            //Sound volume of stone change sound effects
            settings.Add("mayaStoneChangeSoundVolume", new object[] { 0.0f, 0.3f, 1.0f });

            //Sound volume of start whistle
            settings.Add("startWhistleSoundVolume", new object[] { 0.0f, 0.25f, 1.0f });

            //Sound volume of specialbar -> full sound effects
            settings.Add("specialbarFullSoundVolume", new object[] { 0.0f, 0.35f, 1.0f });

            //Sound volume of applause -> GameEnd
            settings.Add("applauseGameEndSoundVolume", new object[] { 0.0f, 0.25f, 1.0f });

            //Sound volume of spawing powerUp effect
            settings.Add("powerUpSpawnSoundVolume", new object[] { 0.0f, 0.15f, 1.0f });

            //Sound volume of heartbeat -> Matchball (Not very good, so disabled for now)
            settings.Add("matchballHeartbeatSoundVolume", new object[] { 0.0f, 0.0f, 1.0f });

            //Sound volume of player touching the bottom
            settings.Add("bottomTouchSoundVolume", new object[] { 0.0f, 0.15f, 1.0f });

            //Sound volume of player touching the bottom
            settings.Add("bombTickSoundVolume", new object[] { 0.0f, 0.15f, 1.0f });

            //Sound volume of player touching the bottom
            settings.Add("volcanoEruptionSoundVolume", new object[] { 0.0f, 1.0f, 1.0f });

            //Sound volume of player touching the bottom
            settings.Add("countdownSoundVolume", new object[] { 0.0f, 0.4f, 1.0f });

            //Sound volume of menubuttons
            settings.Add("menuSoundsSoundVolume", new object[] { 0.0f, 0.75f, 1.0f });

            //Sound volume of music
            settings.Add("musicVolume", new object[] { 0.0f, 0.2f, 1.0f });
            
            #endregion


        }


        /////////////////
        //Class Stuff
        /////////////////

        #region Properties

        private static SettingsManager instance = null;
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingsManager();
                }
                return instance;
            }
        }

        // Dictionary which stores all the settings content
        private Dictionary<String,Object> settings;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor hidden because of Singleton
        /// </summary>
        private SettingsManager()
        {
            settings = new Dictionary<string,object>();
            this.setStandardSettings();

        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to get a Setting
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>null and ErrorMessage if no such Setting was found</returns>
        public Object get(String identifier)
        {
            //Check if the Setting we want to access even exists
           if(settings.ContainsKey(identifier))
           {
                return ((object[])settings[identifier])[1];
           }
            //If not, log Error Message
           else
            {
                Logger.Instance.log(Sender.SettingsManager, "ERROR: Failed to open Setting: " + identifier,PriorityLevel.Priority_5);
                return 0;
            }
        }

        /// <summary>
        /// Method to set a value of a setting
        /// </summary>
        /// <param name="identifier">Name of Setting</param>
        /// <param name="value">Value of Setting (Can be of Any Type)</param>
        /// <returns>False if Setting does not exist</returns>
        public bool set(String identifier, Object value)
        {
            if(settings.ContainsKey(identifier))
            {
                // get old array
                object[] temp_array = (object[])settings[identifier];
                // change currentvalue and store it in dictionary
                settings[identifier] = new object[] { temp_array[0], value, temp_array[2] };
                
                notifyObservers();
                return true;
            }
            else
            {
                Logger.Instance.log(Sender.SettingsManager, "ERROR: Failed to set Setting: " + identifier,PriorityLevel.Priority_5);
                return false;
            }
        }

        #endregion

    }
}
