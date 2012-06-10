using System;
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
        ExplosionBig,
        ExplosionSmall,
        InvertedControl,
        SmashBall,
        SunsetPowerUp,
        WindPowerUp,
        MayaStongeChange,
        StartWhistle,
        SpecialbarFull,
        ApplauseGameEnd,
        PowerUpSpawn,
        MatchballSignal,
        BottomTouchBeach,
        BottomTouchMaya,
        BottomTouchForest,
        BombTick,
        Countdown,
        VolcanoEruption
    };

    /// <summary>
    /// This is used to simplify playing menu sound effects. You don't have to know every number of the files,
    /// just use this enum to identify the right effect.
    /// </summary>
    public enum MenuSound
    {
        Select,
        Switch,
        Warning,
        SelectBack
    };

    /// <summary>
    /// This is used to simplify playing songs. You don't have to know every number of the files,
    /// just use this enum to identify the right song.
    /// </summary>
    public enum MusicTitle
    {
        Intro,
        MainMenu,
        Beach,
        Forest,
        Maya
    };

    /// <summary>
    /// The MusicState tells the SoundManager which music has to be played
    /// </summary>
    public enum MusicState
    {
        Intro,
        MainMenu,
        Beach,
        Forest,
        Maya
    }


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
        VolleySound[] ingameSoundInstance = new VolleySound[13];

        /// <summary>
        /// Holds the duration and the remaining time in MS if a sound is looped
        /// </summary>
        int[,] ingameSoundInstanceDuration = new int[13,2];

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
        /// Stores all of our Background music.
        /// </summary>
        SoundEffect[] Music = new SoundEffect[11];

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
        /// Stores the music volume.
        /// </summary>
        float musicVolume;

        /// <summary>
        /// Stores the sound effect volume.
        /// </summary>
        float soundEffectVolume;

        /// <summary>
        /// Stores the jump powerup effect volume.
        /// </summary>
        float jumpPowerUpEffectVolume;

        /// <summary>
        /// Stores the hit ball effect volume.
        /// </summary>
        float hitBallEffectVolume;

        /// <summary>
        /// Stores the hit net effect volume.
        /// </summary>
        float hitNetEffectVolume;

        /// <summary>
        /// Stores the pick up powerup effect volume.
        /// </summary>
        float pickUpPowerUpEffectVolume;

        /// <summary>
        /// Stores the bigExplosionSound effect volume.
        /// </summary>
        float bigExplosionSoundEffectVolume;

        /// <summary>
        /// Stores the bigExplosionSound effect volume.
        /// </summary>
        float smallExplosionSoundEffectVolume;

        /// <summary>
        /// Stores the inverted control effect volume.
        /// </summary>
        float invertedControlEffectVolume;
        
        /// <summary>
        /// Stores the smashball effect volume.
        /// </summary>
        float smashballEffectVolume;

        /// <summary>
        /// Stores the sunset effect volume.
        /// </summary>
        float sunsetEffectVolume;

        /// <summary>
        /// Stores the sunset effect volume.
        /// </summary>
        float windEffectVolume;

        /// <summary>
        /// Stores the stone change effect volume.
        /// </summary>
        float mayaStoneChangeEffectVolume;

        /// <summary>
        /// Stores the start whistle effect volume.
        /// </summary>
        float startWhistleEffectVolume;

        /// <summary>
        /// Stores the specialbar full effect volume.
        /// </summary>
        float specialbarFullUpEffectVolume;

        /// <summary>
        /// Stores the applause effect volume.
        /// </summary>
        float applauseEffectVolume;

        /// <summary>
        /// Stores the powerUp spawn effect volume.
        /// </summary>
        float powerUpSpawnEffectVolume;

        /// <summary>
        /// Stores the powerUp spawn effect volume.
        /// </summary>
        float matchballHeartbeatEffectVolume;

        /// <summary>
        /// Stores the bottom touch effect volume.
        /// </summary>
        float bottomTouchEffectVolume;

        /// <summary>
        /// Stores the bomb tick effect volume.
        /// </summary>
        float bombTickEffectVolume;

        /// <summary>
        /// Stores the bomb tick effect volume.
        /// </summary>
        float countdownEffectVolume;

        /// <summary>
        /// Stores the bomb tick effect volume.
        /// </summary>
        float volcanoEruptionEffectVolume;

        /// <summary>
        /// Stores the bomb tick effect volume.
        /// </summary>
        float menueButtonsVolume;

        /// <summary>
        /// The maximum Volume for each sound
        /// </summary>
        float[] maximumSoundVolumes;
        


        /// <summary>
        /// The time (in MS) for a loop sound to fade in and out
        /// </summary>
        int fadingTimeLoopSounds = 1500;

        /// <summary>
        /// The time (in MS) for a complete fade out and in
        /// </summary>
        int fadingDurationCompleteFadeOutIn = 1000;

        /// <summary>
        /// The remaining fading time (in MS) when the complete fade out/in is active
        /// </summary>
        int remainingFadingDurationCompleteFadeOutIn = 0;


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
            Music[0] = game.Content.Load<SoundEffect>(@"Audio\Music\Intro");
            Music[1] = game.Content.Load<SoundEffect>(@"Audio\Music\MainMenu");
            Music[2] = game.Content.Load<SoundEffect>(@"Audio\Music\Music_Beach");
            Music[3] = game.Content.Load<SoundEffect>(@"Audio\Music\Music_Forest");
            Music[4] = game.Content.Load<SoundEffect>(@"Audio\Music\Music_Maya");

            ingameSoundEffects[(int)IngameSound.Jump] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\jump");
            ingameSoundEffects[(int)IngameSound.HitBall] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\hitBall");
            ingameSoundEffects[(int)IngameSound.HitNet] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\hitNet");
            ingameSoundEffects[(int)IngameSound.PickupPowerup] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\pickupPowerup");
            ingameSoundEffects[(int)IngameSound.ExplosionBig] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\explosionBig");
            ingameSoundEffects[(int)IngameSound.ExplosionSmall] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\explosionSmall");
            ingameSoundEffects[(int)IngameSound.InvertedControl] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\invertedControl");
            ingameSoundEffects[(int)IngameSound.SmashBall] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\smashBall");
            ingameSoundEffects[(int)IngameSound.SunsetPowerUp] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\sunset");
            ingameSoundEffects[(int)IngameSound.WindPowerUp] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\windPowerUp");
            ingameSoundEffects[(int)IngameSound.MayaStongeChange] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\stoneChange");
            ingameSoundEffects[(int)IngameSound.StartWhistle] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\gameStartWhistle");
            ingameSoundEffects[(int)IngameSound.SpecialbarFull] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\specialbarFull");
            ingameSoundEffects[(int)IngameSound.ApplauseGameEnd] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\applauseGameEnd");
            ingameSoundEffects[(int)IngameSound.PowerUpSpawn] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\powerUpSpawn");
            ingameSoundEffects[(int)IngameSound.MatchballSignal] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\matchballSignal");
            ingameSoundEffects[(int)IngameSound.BottomTouchBeach] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\bottomTouchBeach");
            ingameSoundEffects[(int)IngameSound.BottomTouchMaya] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\bottomTouchMaya");
            ingameSoundEffects[(int)IngameSound.BottomTouchForest] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\bottomTouchForest");
            ingameSoundEffects[(int)IngameSound.BombTick] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\bombtick");
            ingameSoundEffects[(int)IngameSound.Countdown] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\countdown");
            ingameSoundEffects[(int)IngameSound.VolcanoEruption] = game.Content.Load<SoundEffect>(@"Audio\Effects\Ingame\volcanoEruption");

            menuSoundEffects[(int)MenuSound.Select] = game.Content.Load<SoundEffect>(@"Audio\Effects\Menu\Select");
            menuSoundEffects[(int)MenuSound.Switch] = game.Content.Load<SoundEffect>(@"Audio\Effects\Menu\Switch");
            menuSoundEffects[(int)MenuSound.Warning] = game.Content.Load<SoundEffect>(@"Audio\Effects\Menu\Warning");
            menuSoundEffects[(int)MenuSound.SelectBack] = game.Content.Load<SoundEffect>(@"Audio\Effects\Menu\SelectBack");

            musicInstance = null;
            menuSoundInstanceUsage = 0;
            ingameSoundInstanceUsage = 0;
            disableMusic = false;
            maximumSoundVolumes = new float[30];


        }
        
        #endregion


        #region Methods

        /// <summary>
        /// is called by game1, all the functionality is implemented
        /// in this method or a submethod
        /// </summary>
        public void update(GameTime gameTime)
        {

            getTasks();

            // Update the times for every sound so that it can be played again
            for (int i = 0; i < timeTillNextIngameSound.Length; i++)
            {
                timeTillNextIngameSound[i] -= gameTime.ElapsedGameTime.Milliseconds;
                if (timeTillNextIngameSound[i] < 0)
                {
                    timeTillNextIngameSound[i] = 0;
                }
            }

            for (int i = 0; i < timeTillNextMenuSound.Length; i++)
            {
                timeTillNextMenuSound[i] -= gameTime.ElapsedGameTime.Milliseconds;
                if (timeTillNextMenuSound[i] < 0)
                {
                    timeTillNextMenuSound[i] = 0;
                }
            }

            // Check if Intro is over (the only one that isn't looped)
            if (musicInstance != null)
                if (musicInstance.IsLooped == false)
                    if (musicInstance.State == SoundState.Stopped)
                        // Then play the MainMenu Music
                        playMusic(MusicState.MainMenu);

            updateLoopingSounds(gameTime);

            //updateCompleteFading(gameTime);

        }

        /// <summary>
        /// Updates the looping sounds' times and stop them if the time is over
        /// </summary>
        /// <param name="gameTime">The gametime</param>
        public void updateLoopingSounds(GameTime gameTime)
        {

            // Go through every ingame sound
            // (ingameSoundInstanceDuration.Length / 2 because the array is 2D and has 2x the size)
            for (int i = 0; i < (ingameSoundInstanceDuration.Length / 2); i++)
            {

                // If the sound is null or paused, do nothing
                if ((ingameSoundInstance[i] == null) || (ingameSoundInstance[i].SoundEffect.State == SoundState.Paused))
                    continue;

                // Check if it has a looping
                if (ingameSoundInstanceDuration[i, 0] > 0)
                {

                    // Subtract elapsed time
                    ingameSoundInstanceDuration[i, 1] -= gameTime.ElapsedGameTime.Milliseconds;

                    // Set sound volume
                    ingameSoundInstance[i].SoundEffect.Volume = ingameSoundInstance[i].MaximumVolume;

                    // If the time is over, stop the sound and remove it
                    if (ingameSoundInstanceDuration[i, 1] < 0)
                    {

                        ingameSoundInstanceDuration[i, 0] = 0;
                        ingameSoundInstanceDuration[i, 1] = 0;

                        ingameSoundInstance[i].SoundEffect.Stop();
                        ingameSoundInstance[i].SoundEffect.Dispose();
                        ingameSoundInstance[i] = null;

                    }
                    // Check if the sound has to be faded in
                    else if ((ingameSoundInstanceDuration[i, 0] - ingameSoundInstanceDuration[i, 1]) < fadingTimeLoopSounds)
                    {

                        // Fade linear
                        float percentOfFading = ((float)(ingameSoundInstanceDuration[i, 0] - ingameSoundInstanceDuration[i, 1])) / ((float)fadingTimeLoopSounds);

                        // apply to sound
                        ingameSoundInstance[i].SoundEffect.Volume = percentOfFading * ingameSoundInstance[i].MaximumVolume;

                    }
                    else if (ingameSoundInstanceDuration[i, 1] < fadingTimeLoopSounds)
                    {

                        // Fade linear
                        float percentOfFading = ((float)ingameSoundInstanceDuration[i, 1]) / ((float)fadingTimeLoopSounds);

                        // apply to sound
                        ingameSoundInstance[i].SoundEffect.Volume = percentOfFading * ingameSoundInstance[i].MaximumVolume;
                    
                    }
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void updateCompleteFading(GameTime gameTime)
        {

            
            // If the complete fading is active
            if (remainingFadingDurationCompleteFadeOutIn > 0)
            {

                // Update time
                remainingFadingDurationCompleteFadeOutIn -= gameTime.ElapsedGameTime.Milliseconds;
                if (remainingFadingDurationCompleteFadeOutIn < 0)
                    remainingFadingDurationCompleteFadeOutIn = 0;

                // Fade out
                if (remainingFadingDurationCompleteFadeOutIn > ((float)fadingDurationCompleteFadeOutIn / 2.0f))
                {

                    // Calculate percent of fading
                    float percentOfFading = (((float)remainingFadingDurationCompleteFadeOutIn - ((float)fadingDurationCompleteFadeOutIn / 2.0f)) / ((float)fadingDurationCompleteFadeOutIn / 2.0f));
                    

                    // Apply it to every sound and music 
                    float newVolume = percentOfFading * soundEffectVolume;

                    for (int i = 0; i < ingameSoundInstance.Length; i++)
                    {
                        if (ingameSoundInstance[i] != null)
                            ingameSoundInstance[i].SoundEffect.Volume = newVolume;
                    }

                    for (int i = 0; i < menuSoundInstance.Length; i++)
                    {
                        if (menuSoundInstance[i] != null)
                            menuSoundInstance[i].Volume = newVolume;
                    }

                    newVolume = percentOfFading * musicVolume;

                    if (musicInstance != null)
                        musicInstance.Volume = newVolume;

                }
                // Fade in
                else
                {

                    // Calculate percent of fading
                    float percentOfFading = 1.0f - (((float)remainingFadingDurationCompleteFadeOutIn) / ((float)fadingDurationCompleteFadeOutIn / 2.0f));


                    // Apply it to every sound and music 
                    float newVolume = percentOfFading * soundEffectVolume;

                    for (int i = 0; i < ingameSoundInstance.Length; i++)
                    {
                        if (ingameSoundInstance[i] != null)
                            ingameSoundInstance[i].SoundEffect.Volume = newVolume;
                    }

                    for (int i = 0; i < menuSoundInstance.Length; i++)
                    {
                        if (menuSoundInstance[i] != null)
                            menuSoundInstance[i].Volume = newVolume;
                    }

                    newVolume = percentOfFading * musicVolume;

                    if (musicInstance != null)
                        musicInstance.Volume = newVolume;

                }

            }

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

            // Make sure that at the position in the array is not a looped sound which is still be played
            int temp_ingameSoundInstanceUsage = ingameSoundInstanceUsage;

            while (ingameSoundInstanceDuration[ingameSoundInstanceUsage, 1] > 0)
            {
                ingameSoundInstanceUsage = (ingameSoundInstanceUsage + 1) % ingameSoundInstance.Length;
                // Make sure that it's not an endless loop:
                // If there are just looped sounds, take the first one and override it
                if (ingameSoundInstanceUsage == temp_ingameSoundInstanceUsage)
                    break;
            }

            if (ingameSoundInstance[ingameSoundInstanceUsage] != null)
                ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.Dispose();

            // Create instance
            ingameSoundInstance[ingameSoundInstanceUsage] = new VolleySound(ingameSoundEffects[n].CreateInstance(), (IngameSound)n, maximumSoundVolumes[n]) ;

            // Set duration time to 0
            ingameSoundInstanceDuration[ingameSoundInstanceUsage, 0] = 0;
            ingameSoundInstanceDuration[ingameSoundInstanceUsage, 1] = 0;

            // Disable looping
            ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.IsLooped = false;


            /*
           switch (n)
           {
               case (int)IngameSound.Jump:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = jumpPowerUpEffectVolume;
                   break;
               case (int)IngameSound.HitBall:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = hitBallEffectVolume;
                   break;
               case (int)IngameSound.HitNet:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = hitNetEffectVolume;
                   break;
               case (int)IngameSound.PickupPowerup:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = pickUpPowerUpEffectVolume;
                   break;
               case (int)IngameSound.ExplosionSmall:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = smallExplosionSoundEffectVolume;
                   break;
               case (int)IngameSound.ExplosionBig:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = bigExplosionSoundEffectVolume;
                   break;
               case (int)IngameSound.InvertedControl:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = invertedControlEffectVolume;
                   break;
               case (int)IngameSound.SmashBall:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = smashballEffectVolume;
                   break;
               case (int)IngameSound.SunsetPowerUp:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = sunsetEffectVolume;
                   break;
               case (int)IngameSound.WindPowerUp:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = windEffectVolume;
                   break;
               case (int)IngameSound.MayaStongeChange:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = mayaStoneChangeEffectVolume;
                   break;
               case (int)IngameSound.StartWhistle:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = startWhistleEffectVolume;
                   break;
               case (int)IngameSound.SpecialbarFull:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = specialbarFullUpEffectVolume;
                   break;
               case (int)IngameSound.ApplauseGameEnd:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = applauseEffectVolume;
                   break;
               case (int)IngameSound.PowerUpSpawn:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = powerUpSpawnEffectVolume;
                   break;
               case (int)IngameSound.MatchballSignal:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = matchballHeartbeatEffectVolume;
                   break;
               case (int)IngameSound.BottomTouchBeach:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = bottomTouchEffectVolume;
                   break;
               case (int)IngameSound.BottomTouchMaya:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = bottomTouchEffectVolume;
                   break;
               case (int)IngameSound.BottomTouchForest:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = bottomTouchEffectVolume;
                   break;
               case (int)IngameSound.BombTick:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = bombTickEffectVolume;
                   break;
               case (int)IngameSound.Countdown:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = countdownEffectVolume;
                   break;
               case (int)IngameSound.VolcanoEruption:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = volcanoEruptionEffectVolume;
                   break;
               default:
                   ingameSoundInstance[ingameSoundInstanceUsage].Volume = soundEffectVolume;
                   break;
           }
            */
            ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.Volume = maximumSoundVolumes[n];

            ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.Play();


           ingameSoundInstanceUsage = (ingameSoundInstanceUsage + 1) % ingameSoundInstance.Length;
        }

        /// <summary>
        /// Plays the ingame SoundEffect with the looped for a specified time
        /// </summary>
        /// <param name="n">The enum/int of the sound effect</param>
        /// <param name="loopingDuration">The duration of the sound (in MS), it is looped till the time is over</param>
        public void playIngameSoundEffect(int n, int loopingDuration)
        {
            if (n < 0) return;

            if (n != (int)IngameSound.ExplosionSmall)
            {

                if (timeTillNextIngameSound[n] > 0) return;

                // timeBetweenTwoEqualSounds: 500 ms (Ingame)
                timeTillNextIngameSound[n] = 500;

            }

            // Make sure that at the position in the array is not a looped sound which is stil be player
            int temp_ingameSoundInstanceUsage = ingameSoundInstanceUsage;

            while (ingameSoundInstanceDuration[ingameSoundInstanceUsage, 1] > 0)
            {
                ingameSoundInstanceUsage = (ingameSoundInstanceUsage + 1) % ingameSoundInstance.Length;
                // Make sure that it's not an endless loop:
                // If there are just looped sounds, take the first one and override it
                if (ingameSoundInstanceUsage == temp_ingameSoundInstanceUsage)
                    break;
            }

            if (ingameSoundInstance[ingameSoundInstanceUsage] != null)
                ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.Dispose();

            // Create instance
            ingameSoundInstance[ingameSoundInstanceUsage] = new VolleySound(ingameSoundEffects[n].CreateInstance(), (IngameSound)n, maximumSoundVolumes[n]);
            
            // Save time
            ingameSoundInstanceDuration[ingameSoundInstanceUsage, 0] = loopingDuration;
            ingameSoundInstanceDuration[ingameSoundInstanceUsage, 1] = loopingDuration;

            // Enable looping
            ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.IsLooped = true;

            /*
            switch (n)
            {
                case (int)IngameSound.Jump:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = jumpPowerUpEffectVolume;
                    break;
                case (int)IngameSound.HitBall:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = hitBallEffectVolume;
                    break;
                case (int)IngameSound.HitNet:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = hitNetEffectVolume;
                    break;
                case (int)IngameSound.PickupPowerup:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = pickUpPowerUpEffectVolume;
                    break;
                case (int)IngameSound.ExplosionSmall:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = smallExplosionSoundEffectVolume;
                    break;
                case (int)IngameSound.ExplosionBig:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = bigExplosionSoundEffectVolume;
                    break;
                case (int)IngameSound.InvertedControl:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = invertedControlEffectVolume;
                    break;
                case (int)IngameSound.SmashBall:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = smashballEffectVolume;
                    break;
                case (int)IngameSound.SunsetPowerUp:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = sunsetEffectVolume;
                    break;
                case (int)IngameSound.WindPowerUp:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = windEffectVolume;
                    break;
                case (int)IngameSound.MayaStongeChange:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = mayaStoneChangeEffectVolume;
                    break;
                case (int)IngameSound.StartWhistle:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = startWhistleEffectVolume;
                    break;
                case (int)IngameSound.SpecialbarFull:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = specialbarFullUpEffectVolume;
                    break;
                case (int)IngameSound.ApplauseGameEnd:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = applauseEffectVolume;
                    break;
                case (int)IngameSound.PowerUpSpawn:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = powerUpSpawnEffectVolume;
                    break;
                case (int)IngameSound.MatchballSignal:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = matchballHeartbeatEffectVolume;
                    break;
                case (int)IngameSound.BottomTouchBeach:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = bottomTouchEffectVolume;
                    break;
                case (int)IngameSound.BottomTouchMaya:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = bottomTouchEffectVolume;
                    break;
                case (int)IngameSound.BottomTouchForest:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = bottomTouchEffectVolume;
                    break;
                case (int)IngameSound.BombTick:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = bombTickEffectVolume;
                    break;
                case (int)IngameSound.Countdown:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = countdownEffectVolume;
                    break;
                case (int)IngameSound.VolcanoEruption:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = volcanoEruptionEffectVolume;
                    break;
                default:
                    ingameSoundInstance[ingameSoundInstanceUsage].Volume = soundEffectVolume;
                    break;
            }
            */

            ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.Volume = maximumSoundVolumes[n];
            ingameSoundInstance[ingameSoundInstanceUsage].SoundEffect.Play();


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

            menuSoundInstance[menuSoundInstanceUsage].Volume = menueButtonsVolume;



            menuSoundInstance[menuSoundInstanceUsage].Play();


            menuSoundInstanceUsage = (menuSoundInstanceUsage + 1) % menuSoundInstance.Length;
        }

        /// <summary>
        /// Plays the song for the given state
        /// </summary>
        /// <param name="musicState">The state indicates which song has to be played</param>
        public void playMusic(MusicState musicState)
        {

            // Throw old instance away if there is one
            if (musicInstance != null)
            {
                musicInstance.Dispose();
                musicInstance = null;
            }

            musicInstance = Music[(int)musicState].CreateInstance();
            musicInstance.Volume = musicVolume;

            // Loop all except intro
            if (musicState != MusicState.Intro)
                musicInstance.IsLooped = true;

            musicInstance.Play();

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
        /// Stops all menu sounds immediately and throws the sound instances away
        /// </summary>
        public void discardMenuSounds()
        {

            for (int i = 0; i < menuSoundInstance.Length; i++)
            {

                // Release instance
                if (menuSoundInstance[i] != null)
                {
                    menuSoundInstance[i].Stop();
                    menuSoundInstance[i].Dispose();
                }

                menuSoundInstance[i] = null;
            }

            // Reset array
            menuSoundInstanceUsage = 0;

        }

        /// <summary>
        /// Stops all ingame sounds immediately and throws the sound instances away
        /// </summary>
        public void discardIngameSounds()
        {

            for (int i = 0; i < ingameSoundInstance.Length; i++)
            {

                // Release instance
                if (ingameSoundInstance[i] != null)
                {
                    ingameSoundInstance[i].SoundEffect.Stop();
                    ingameSoundInstance[i].SoundEffect.Dispose();
                }

                ingameSoundInstance[i] = null;
            }

            // Reset array
            ingameSoundInstanceUsage = 0;
            ingameSoundInstanceDuration = new int[13, 2];

        }

        /// <summary>
        /// Pauses all ingame sounds and the music
        /// </summary>
        /// <param name="pause">True: Sounds will be paused, False: Sounds will continue</param>
        public void pauseIngameSounds(bool pause)
        {

            for (int i = 0; i < ingameSoundInstance.Length; i++)
            {

                // Check if it's a SoundInstance. If so, pause/continue
                if ((ingameSoundInstance[i] != null) && (ingameSoundInstance[i].SoundEffect.State != SoundState.Stopped))
                {
                    if (pause)
                        ingameSoundInstance[i].SoundEffect.Pause();
                    else
                        ingameSoundInstance[i].SoundEffect.Resume();
                }

                // Pause music
                if (musicInstance != null)
                {
                    if (pause)
                        musicInstance.Pause();
                    else
                        musicInstance.Resume();
                }
            }

        }

        /// <summary>
        /// The method fades all sounds and the music out and then in again
        /// </summary>
        public void completeyFadeOutIn()
        {

            // Set time
            //remainingFadingDurationCompleteFadeOutIn = fadingDurationCompleteFadeOutIn;

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

                // Check if sound should be looped
                if (task.LoopingDuration > 0)
                    playIngameSoundEffect(task.SoundEffectNumber, task.LoopingDuration);
                else
                    playIngameSoundEffect(task.SoundEffectNumber);

            }

            // Clear list after executing all tasks
            TaskManager.Instance.SoundTasks.Clear();
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
                jumpPowerUpEffectVolume = (float)((SettingsManager)o).get("jumpPowerUpSoundVolume");
                hitBallEffectVolume = (float)((SettingsManager)o).get("hitBallSoundVolume");
                hitNetEffectVolume = (float)((SettingsManager)o).get("hitNetSoundVolume");
                pickUpPowerUpEffectVolume = (float)((SettingsManager)o).get("pickupPowerUpSoundVolume");
                invertedControlEffectVolume = (float)((SettingsManager)o).get("invertedControlSoundVolume");
                smashballEffectVolume = (float)((SettingsManager)o).get("smashBallSoundVolume");
                sunsetEffectVolume = (float)((SettingsManager)o).get("sunsetPowerUpSoundVolume");
                windEffectVolume = (float)((SettingsManager)o).get("windPowerUpSoundVolume");
                mayaStoneChangeEffectVolume = (float)((SettingsManager)o).get("mayaStoneChangeSoundVolume");
                startWhistleEffectVolume = (float)((SettingsManager)o).get("startWhistleSoundVolume");
                specialbarFullUpEffectVolume = (float)((SettingsManager)o).get("specialbarFullSoundVolume");
                applauseEffectVolume = (float)((SettingsManager)o).get("applauseGameEndSoundVolume");
                powerUpSpawnEffectVolume = (float)((SettingsManager)o).get("powerUpSpawnSoundVolume");
                matchballHeartbeatEffectVolume = (float)((SettingsManager)o).get("matchballHeartbeatSoundVolume");
                bottomTouchEffectVolume = (float)((SettingsManager)o).get("bottomTouchSoundVolume");
                bombTickEffectVolume = (float)((SettingsManager)o).get("bombTickSoundVolume");
                countdownEffectVolume = (float)((SettingsManager)o).get("countdownSoundVolume");
                volcanoEruptionEffectVolume = (float)((SettingsManager)o).get("volcanoEruptionSoundVolume");

                maximumSoundVolumes = new float[30];

                maximumSoundVolumes[(int)IngameSound.ExplosionBig] = bigExplosionSoundEffectVolume;
                maximumSoundVolumes[(int)IngameSound.ExplosionSmall] = smallExplosionSoundEffectVolume;
                maximumSoundVolumes[(int)IngameSound.Jump] = jumpPowerUpEffectVolume;
                maximumSoundVolumes[(int)IngameSound.HitBall] = hitBallEffectVolume;
                maximumSoundVolumes[(int)IngameSound.HitNet] = hitNetEffectVolume;
                maximumSoundVolumes[(int)IngameSound.PickupPowerup] = pickUpPowerUpEffectVolume;
                maximumSoundVolumes[(int)IngameSound.InvertedControl] = invertedControlEffectVolume;
                maximumSoundVolumes[(int)IngameSound.SmashBall] = smashballEffectVolume;
                maximumSoundVolumes[(int)IngameSound.SunsetPowerUp] = sunsetEffectVolume;
                maximumSoundVolumes[(int)IngameSound.WindPowerUp] = windEffectVolume;
                maximumSoundVolumes[(int)IngameSound.MayaStongeChange] = mayaStoneChangeEffectVolume;
                maximumSoundVolumes[(int)IngameSound.StartWhistle] = startWhistleEffectVolume;
                maximumSoundVolumes[(int)IngameSound.SpecialbarFull] = specialbarFullUpEffectVolume;
                maximumSoundVolumes[(int)IngameSound.ApplauseGameEnd] = applauseEffectVolume;
                maximumSoundVolumes[(int)IngameSound.PowerUpSpawn] = powerUpSpawnEffectVolume;
                maximumSoundVolumes[(int)IngameSound.MatchballSignal] = matchballHeartbeatEffectVolume;
                maximumSoundVolumes[(int)IngameSound.BottomTouchBeach] = bottomTouchEffectVolume;
                maximumSoundVolumes[(int)IngameSound.BottomTouchForest] = bottomTouchEffectVolume;
                maximumSoundVolumes[(int)IngameSound.BottomTouchMaya] = bottomTouchEffectVolume;
                maximumSoundVolumes[(int)IngameSound.BombTick] = bombTickEffectVolume;
                maximumSoundVolumes[(int)IngameSound.Countdown] = countdownEffectVolume;
                maximumSoundVolumes[(int)IngameSound.VolcanoEruption] = volcanoEruptionEffectVolume;


                menueButtonsVolume = (float)((SettingsManager)o).get("menuSoundsSoundVolume");
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

            ingameSoundInstanceDuration = new int[13, 2];

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
