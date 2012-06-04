﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using ManhattanMorning.Misc.Tasks;
using ManhattanMorning.Misc;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Controller
{
    /// <summary>
    /// This is used to simplify playing ingame sound effects. You don't have to know every number of the files,
    /// just use this enum to identify the right effect.
    /// </summary>
    public enum IngameSound
    {
        Jump,
        HitBall,
        HitNet,
        PickupPowerup,
        TouchFloor,
        ExplosionBig,
        ExplosionSmall,
        InvertedControl,
        SmashBall,
        SunsetPowerUp,
        WindPowerUp,
        MayaStongeChange,
        StartWhistle,
        SpecialbarFull
    };

    /// <summary>
    /// This is used to simplify playing menu sound effects. You don't have to know every number of the files,
    /// just use this enum to identify the right effect.
    /// </summary>
    public enum MenuSound
    {
        Select,
        Switch,
        Warning
    };


    /// <summary>
    /// Sound Engine responsible of
    /// playing sounds
    /// </summary>
    class SoundManager : IObserver, IController
    {

        #region Properties

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Stores if the music shall play or not. True means no music.
        /// </summary>
        public bool DisableMusic { get { return disableMusic; } set { disableMusic = value; } }

        /// <summary>
        /// Stores if the sound shall play or not. True means no sound.
        /// </summary>
        public bool DisableSound { get { return disableSound; } set { disableSound = value; } }


        #endregion

        #region Members

        /// <summary>
        /// Stores the state of music. If true, there will be no music.
        /// </summary>
        public bool disableMusic;

        /// <summary>
        /// Stores the state of sound. If true, there will be no sound.
        /// </summary>
        public bool disableSound;

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        static SoundManager instance;

        /// <summary>
        /// Instance of the logger (singleton)
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Instance of the SettingsManager.
        /// </summary>
        private SettingsManager settingsManager;

        /// <summary>
        /// Stores the Instance of our game.
        /// </summary>
        Game1 game;

        /// <summary>
        /// Holds the current song that is played.
        /// </summary>
        SoundEffectInstance musicInstance;

        /// <summary>
        /// Holds the current sound effects that can be played ingame
        /// </summary>
        SoundEffectInstance[] ingameSoundInstance = new SoundEffectInstance[13];

        /// <summary>
        /// Points to the next free ingameSoundInstance that can play a sound.
        /// </summary>
        int ingameSoundInstanceUsage;

        /// <summary>
        /// Holds the current sound effects that can be played in menu
        /// </summary>
        SoundEffectInstance[] menuSoundInstance = new SoundEffectInstance[2];

        /// <summary>
        /// Points to the next free menuSoundInstance that can play a sound.
        /// </summary>
        int menuSoundInstanceUsage;

        /// <summary>
        /// Indicates the number of the song that is currently played.
        /// </summary>
        int currentSong;

        /// <summary>
        /// Stores all of our Background music.
        /// </summary>
        SoundEffect[] backgroundMusic = new SoundEffect[11];

        /// <summary>
        /// Stores all the ingameSound effects.
        /// </summary>
        SoundEffect[] ingameSoundEffects = new SoundEffect[30];

        /// <summary>
        /// Time till next ingame sound of the given effect im ms
        /// </summary>
        int[] timeTillNextIngameSound = new int[30];

        /// <summary>
        /// Stores all the menuSound effects.
        /// </summary>
        SoundEffect[] menuSoundEffects = new SoundEffect[10];

        /// <summary>
        /// Time till next menu sound of the given effect im ms
        /// </summary>
        int[] timeTillNextMenuSound = new int[10];

        /// <summary>
        /// Stores the order of the songs so that every song is played before the lists repeats
        /// in different order.
        /// </summary>
        int[] playlist = new int[11];

        /// <summary>
        /// Stores the music volume.
        /// </summary>
        float musicVolume;

        /// <summary>
        /// Stores the sound effect volume.
        /// </summary>
        float soundEffectVolume;

        /// <summary>
        /// Stores the bigExplosionSound effect volume.
        /// </summary>
        float bigExplosionSoundEffectVolume;

        /// <summary>
        /// Stores the bigExplosionSound effect volume.
        /// </summary>
        float smallExplosionSoundEffectVolume;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes sound manager
        /// hidden because of Singleton Pattern
        /// </summary>
        private SoundManager()
        {

            logger = Logger.Instance;
            logger.log(Sender.SoundEngine, "initialized", PriorityLevel.Priority_2);

            game = Game1.Instance;
            settingsManager = SettingsManager.Instance;

            // Register as an observer for SettingsManager
            settingsManager.registerObserver(this);
            notify(settingsManager);

            //load soundeffects
            backgroundMusic[0] = game.Content.Load<SoundEffect>(@"Audio\Music\Murloc");
            backgroundMusic[1] = game.Content.Load<SoundEffect>(@"Audio\Music\1");
            backgroundMusic[2] = game.Content.Load<SoundEffect>(@"Audio\Music\2");
            backgroundMusic[3] = game.Content.Load<SoundEffect>(@"Audio\Music\3");
            backgroundMusic[4] = game.Content.Load<SoundEffect>(@"Audio\Music\4");
            backgroundMusic[5] = game.Content.Load<SoundEffect>(@"Audio\Music\5");
            backgroundMusic[6] = game.Content.Load<SoundEffect>(@"Audio\Music\Suspense1");
            backgroundMusic[7] = game.Content.Load<SoundEffect>(@"Audio\Music\Suspense2");
            backgroundMusic[8] = game.Content.Load<SoundEffect>(@"Audio\Music\Suspense3");
            backgroundMusic[9] = game.Content.Load<SoundEffect>(@"Audio\Music\Suspense4");
            backgroundMusic[10] = game.Content.Load<SoundEffect>(@"Audio\Music\Dreh_den_Swag_auf_Indie_Version");

            ingameSoundEffects[(int)IngameSound.Jump] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\jump");
            ingameSoundEffects[(int)IngameSound.HitBall] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\hitBall");
            ingameSoundEffects[(int)IngameSound.HitNet] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\hitNet");
            ingameSoundEffects[(int)IngameSound.PickupPowerup] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\pickupPowerup");
            ingameSoundEffects[(int)IngameSound.TouchFloor] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\bottomTouch");
            ingameSoundEffects[(int)IngameSound.ExplosionBig] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\explosionBig");
            ingameSoundEffects[(int)IngameSound.ExplosionSmall] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\explosionSmall");
            ingameSoundEffects[(int)IngameSound.InvertedControl] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\invertedControl");
            ingameSoundEffects[(int)IngameSound.SmashBall] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\smashBall");
            ingameSoundEffects[(int)IngameSound.SunsetPowerUp] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\sunset");
            ingameSoundEffects[(int)IngameSound.WindPowerUp] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\windPowerUp");
            ingameSoundEffects[(int)IngameSound.MayaStongeChange] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\stoneChange");
            ingameSoundEffects[(int)IngameSound.StartWhistle] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\gameStartWhistle");
            ingameSoundEffects[(int)IngameSound.SpecialbarFull] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\specialbarFull");

            menuSoundEffects[(int)MenuSound.Select] = game.Content.Load<SoundEffect>(@"Audio\Effects\Menu\Select");
            menuSoundEffects[(int)MenuSound.Switch] = game.Content.Load<SoundEffect>(@"Audio\Effects\Menu\Switch");
            menuSoundEffects[(int)MenuSound.Warning] = game.Content.Load<SoundEffect>(@"Audio\Effects\Menu\Warning");

            musicInstance = backgroundMusic[0].CreateInstance();
            menuSoundInstanceUsage = 0;
            ingameSoundInstanceUsage = 0;
            disableMusic = true;

            currentSong = -1;

        }
        
        #endregion


        #region Methods

        /// <summary>
        /// is called by game1, all the functionality is implemented
        /// in this method or a submethod
        /// </summary>
        public void update(GameTime gametime)
        {

            getTasks();

            // Update the times for every sound so that it can be played again
            for (int i = 0; i < timeTillNextIngameSound.Length; i++)
            {
                timeTillNextIngameSound[i] -= gametime.ElapsedGameTime.Milliseconds;
                if (timeTillNextIngameSound[i] < 0)
                {
                    timeTillNextIngameSound[i] = 0;
                }
            }

            for (int i = 0; i < timeTillNextMenuSound.Length; i++)
            {
                timeTillNextMenuSound[i] -= gametime.ElapsedGameTime.Milliseconds;
                if (timeTillNextMenuSound[i] < 0)
                {
                    timeTillNextMenuSound[i] = 0;
                }
            }

            //after a song is played its state will be stopped so that this clause plays the next song
            if (musicInstance.State == SoundState.Stopped && !disableMusic)
                nextSong();


        }


        /// <summary>
        /// Plays the ingame SoundEffect with the given number.
        /// </summary>
        /// <param name="n">The enum/int of the sound effect</param>
        public void playIngameSoundEffect(int n)
        {
            if (n < 0) return;

            if (n != (int)IngameSound.ExplosionSmall)
            {

                if (timeTillNextIngameSound[n] > 0) return;

                // timeBetweenTwoEqualSounds: 500 ms (Ingame)
                timeTillNextIngameSound[n] = 500;

            }

            if (ingameSoundInstance[ingameSoundInstanceUsage] != null)
                ingameSoundInstance[ingameSoundInstanceUsage].Dispose();

            ingameSoundInstance[ingameSoundInstanceUsage] = ingameSoundEffects[n].CreateInstance();

           switch (n)
           {
               case (int)IngameSound.ExplosionSmall:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = smallExplosionSoundEffectVolume;
                   break;
               case (int)IngameSound.ExplosionBig:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = bigExplosionSoundEffectVolume;
                   break;
               default:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = soundEffectVolume;
                   break;
           }

           ingameSoundInstance[ingameSoundInstanceUsage].Play();


           ingameSoundInstanceUsage = (ingameSoundInstanceUsage + 1) % ingameSoundInstance.Length;
        }


        /// <summary>
        /// Plays the menu SoundEffect with the given number.
        /// </summary>
        /// <param name="n">The enum/int of the sound effect</param>
        public void playMenuSoundEffect(int n)
        {
            if (n < 0) return;


            if (timeTillNextMenuSound[n] > 0) return;

            // timeBetweenTwoEqualSounds: 200 ms (Menu)
            timeTillNextMenuSound[n] = 200;



            if (menuSoundInstance[menuSoundInstanceUsage] != null)
                menuSoundInstance[menuSoundInstanceUsage].Dispose();

            menuSoundInstance[menuSoundInstanceUsage] = menuSoundEffects[n].CreateInstance();

            menuSoundInstance[menuSoundInstanceUsage].Volume = soundEffectVolume;



            menuSoundInstance[menuSoundInstanceUsage].Play();


            menuSoundInstanceUsage = (menuSoundInstanceUsage + 1) % menuSoundInstance.Length;
        }


        /// <summary>
        /// You can change the SoundState of the musicInstance with this function so that the music
        /// is paused, resumed or stopped.
        /// </summary>
        /// <param name="state">SoundState of the musicInstance.</param>
        public void changeMusicState(SoundState state)
        {
            switch (state)
            {
                case SoundState.Paused:
                    musicInstance.Pause();
                    break;

                case SoundState.Playing:
                    musicInstance.Play();
                    break;

                case SoundState.Stopped:
                    disableMusic = true;
                    musicInstance.Stop();
                    break;
            }
        }

        /// <summary>
        /// You can change the SoundState of the soundInstances with this function so that all sounds
        /// are paused, resumed or stopped.
        /// </summary>
        /// <param name="state">SoundState of the musicInstance.</param>
        public void changeSoundState(SoundState state)
        {
            switch (state)
            {
                case SoundState.Paused:
                    for (int i = 0; i < ingameSoundInstance.Length; i++)
                    {
                        ingameSoundInstance[i].Pause();
                    }
                    for (int i = 0; i < menuSoundInstance.Length; i++)
                    {
                        menuSoundInstance[i].Pause();
                    }
                    break;

                case SoundState.Playing:
                    musicInstance.Play();
                    break;

                case SoundState.Stopped:
                    disableSound = true;
                    for (int i = 0; i < ingameSoundInstance.Length; i++)
                    {
                        ingameSoundInstance[i].Stop(true);
                    }
                    for (int i = 0; i < menuSoundInstance.Length; i++)
                    {
                        menuSoundInstance[i].Stop(true);
                    }
                    break;
            }
        }

        /// <summary>
        /// Plays the next song in the playlist.
        /// </summary>
        public void nextSong()
        {
            //creates a new playlist order if every song is played once
            if (currentSong == playlist.Length || currentSong == -1) createPlaylistOrder();

            //sets the musicInstance to new song
            musicInstance = backgroundMusic[playlist[currentSong]].CreateInstance();
            musicInstance.Volume = musicVolume;
            musicInstance.Play();

            currentSong++;
        }

        /// <summary>
        /// Plays the song in the playlist which corresponds to the given number.
        /// </summary>
        /// <param name="number">Song number that you want to Play</param>
        public void nextSong(int number)
        {
            if (number >= playlist.Length) number = playlist.Length - 1;
            currentSong = number;

            //creates a new playlist order if every song is played once
            if (currentSong == playlist.Length) createPlaylistOrder();

            //sets the musicInstance to new song
            musicInstance = backgroundMusic[playlist[currentSong]].CreateInstance();
            musicInstance.Volume = musicVolume;
            musicInstance.Play();

            currentSong++;
        }


        #endregion

        #region HelperMethods

        /// <summary>
        /// Processes all tasks that are sent to the soundManager by other Controllers.
        /// </summary>
        private void getTasks()
        {
            foreach(SoundTask task in TaskManager.Instance.SoundTasks)
            {
                playIngameSoundEffect(task.SoundEffectNumber);
            }

            // Clear list after executing all tasks
            TaskManager.Instance.SoundTasks.Clear();
        }
        

        /// <summary>
        /// Creates a new Playlist for the background music. Result is storred in "playlist[]".
        /// </summary>
        private void createPlaylistOrder()
        {
            currentSong = 0;
            Random rand = new Random();
            int temp;

            //init with invalid song numbers
            for (int i = 0; i < playlist.Length; i++)
            {
                playlist[i] = -1;
            }

            //fill array of songs with numbers that don't appear twice
            for (int i = 0; i < playlist.Length; i++)
            {
                //insert number at random position
                temp = rand.Next(playlist.Length-1);

                //search next availible position from the random position
                for (int j = 0; j < playlist.Length; j++)
                {
                    if (playlist[ (temp+j) % playlist.Length] == -1)
                    {
                        playlist[(temp + j) % playlist.Length] = i;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Has to be Implemented to Listen to the SettingsManager
        /// </summary>
        /// <param name="o">The Observable Object that sends the Message</param>
        public void notify(ObservableObject o)
        {
            //Check if we are Dealing with the right ObservableObject
            if (o is SettingsManager)
            {
                logger.log(Sender.SoundEngine, this.ToString() + " received a notification from the Settings Manager", PriorityLevel.Priority_2);
                soundEffectVolume = (float)((SettingsManager)o).get("soundVolume");
                bigExplosionSoundEffectVolume = (float)((SettingsManager)o).get("bigExplosionSoundVolume");
                smallExplosionSoundEffectVolume = (float)((SettingsManager)o).get("smallExplosionSoundVolume");

                musicVolume = (float)((SettingsManager)o).get("musicVolume");
 
            }
        }

        #endregion



        /// <summary>
        /// Does all necessary action, to bring controller back to the state after initialization
        /// </summary>
        public void clear()
        {
            changeMusicState(SoundState.Stopped);
            musicInstance = null;
            ingameSoundInstanceUsage = 0;
            menuSoundInstanceUsage = 0;
            disableMusic = true;

            currentSong = -1;

            TaskManager.Instance.SoundTasks.Clear();
        }

        /// <summary>
        /// Forces controller to pause, has to make sure that all timers etc. pause
        /// </summary>
        /// <param name="on">true: controller has to pause, false: controller has to work </param>
        public void pause(bool on)
        {
            if (on) changeMusicState(SoundState.Paused);
            else changeMusicState(SoundState.Playing);
        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        public void initialize()
        {
            //throw new NotImplementedException();
        }
    }
}
