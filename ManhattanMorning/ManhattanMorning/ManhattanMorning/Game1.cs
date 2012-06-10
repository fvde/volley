using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

using ManhattanMorning.Controller;
using ManhattanMorning.Controller.AI;
using ManhattanMorning.Model;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Misc;
using ManhattanMorning.Misc.Logic;
using ManhattanMorning.View;
using ManhattanMorning.Model.Menu;

namespace ManhattanMorning
{



    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Properties

        /// <summary>
        /// Returns the Instance of Game1
        /// (kind of Singleton Pattern)
        /// </summary>
        public static Game1 Instance { get { return instance; } }

        /// <summary>
        /// True if the game is started for the first time
        /// </summary>
        public static bool FirstTimePlaying = true;

        /// <summary>
        /// Necessary for accessing XNA's Graphics Interface
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get { return graphicsDeviceManager; } set { graphicsDeviceManager = value; } }

        /// <summary>
        /// Stores all important time data
        /// </summary>
        public GameTime GameTime { get { return gameTime; } set { gameTime = value; } }

        /// <summary>
        /// True if a video is being played right now.
        /// </summary>
        public static bool VideoPlaying {
            get {
                if (videoPlayer.IsDisposed) return false;
                return videoPlayer.State == Microsoft.Xna.Framework.Media.MediaState.Playing; } }

        /// <summary>
        /// The video that is played.
        /// </summary>
        public Video Video
        {
            get { return video; }
            set { video = value; }
        }

        #endregion

        #region Members

        /// <summary>
        /// Necessary instance for Singleton Pattern
        /// </summary>
        private static Game1 instance;

        /// <summary>
        /// Necessary for accessig XNA's Graphics Interface
        /// </summary>
        private GraphicsDeviceManager graphicsDeviceManager;

        /// <summary>
        /// Stores all important time data
        /// </summary>
        private GameTime gameTime = new GameTime();

        /// <summary>
        /// Video player.
        /// </summary>
        public static VideoPlayer videoPlayer;

        /// <summary>
        /// The video that is played.
        /// </summary>
        private Video video;

        /// <summary>
        /// Array with all playerIndexes
        /// </summary>
        private PlayerIndex[] indexArray = { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };

        #endregion
        

        #region Initialization

        public Game1()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set window resolution and title
            graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            graphicsDeviceManager.PreferredBackBufferHeight = 720;
            Window.AllowUserResizing = true;
            Window.Title = "Volley";

            videoPlayer = new VideoPlayer();

        }



        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set instance so that every class can get it by calling
            // Game1.Instance
            instance = this;

            SuperController.Instance.initialize();
             
            Logger.Instance.log(Sender.Game1, "Initialize() done successfully",PriorityLevel.Priority_2);
            base.Initialize();

            //play IntroVideo on game start
            if((bool)SettingsManager.Instance.get("IntroVideo"))
                playVideo(video);

            determineIfFirstTimePlaying();
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {          
            Logger.Instance.log(Sender.Game1, "LoadContent() done successfully",PriorityLevel.Priority_2);
        }



        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            Logger.Instance.log(Sender.Game1, "UnloadContent() done successfully",PriorityLevel.Priority_2);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && VideoPlaying)
            {
                stopVideo();
            }

            // Skip videos with gamepads
            // go through all possible gamepads
            if (VideoPlaying)
            {
                for (int i = 0; i < indexArray.Length; i++)
                {

                    if (GamePad.GetState(indexArray[i]).IsConnected)
                    {
                        if (GamePad.GetState(indexArray[i]).IsButtonDown(Buttons.A) ||
                            GamePad.GetState(indexArray[i]).IsButtonDown(Buttons.B) ||
                            GamePad.GetState(indexArray[i]).IsButtonDown(Buttons.X) ||
                            GamePad.GetState(indexArray[i]).IsButtonDown(Buttons.Y)
                            )
                            stopVideo();
                    }

                }
            }

            if (!VideoPlaying)
            {
                SuperController.Instance.Update(gameTime);
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            SuperController.Instance.Draw(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Method that quits the whole game properly
        /// </summary>
        public void QuitGame()
        {
            Logger.Instance.log(Sender.Game1, "Game exited properly",PriorityLevel.Priority_2);
            this.Exit();
        }

        /// <summary>
        /// Determines whether a player is starting the game for the first time or not.
        /// </summary>
        private void determineIfFirstTimePlaying()
        {
            /*
            StorageDevice device = new StorageDevice();

            // Open a storage container.
            IAsyncResult result = device.BeginOpenContainer("StorageDemo", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "volleytest.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
            {
                FirstTimePlaying = false;
            }
            else
            {
                container.CreateFile(filename);
                FirstTimePlaying = true;
            }
             */
        }


        #region Videos

        /// <summary>
        /// Plays a video
        /// </summary>
        /// <param name="v"></param>
        public static void playVideo(Video v)
        {
            if (videoPlayer.IsDisposed) videoPlayer = new VideoPlayer();
            videoPlayer.Play(v);
        }

        /// <summary>
        /// Plays a video
        /// </summary>
        /// <param name="v"></param>
        public static void pauseVideo()
        {
            videoPlayer.Pause();
        }

        /// <summary>
        /// Plays a video
        /// </summary>
        /// <param name="v"></param>
        public static void resumeVideo()
        {
            videoPlayer.Resume();
        }

        /// <summary>
        /// Plays a video
        /// </summary>
        /// <param name="v"></param>
        public static void stopVideo()
        {
            videoPlayer.Stop();
            videoPlayer.Dispose();
            Graphics.Instance.fadeColor(Color.White, 800);
        }

        #endregion


        #endregion


    }
}
