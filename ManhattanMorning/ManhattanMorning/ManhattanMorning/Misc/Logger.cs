using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Origins that can send log messages to our Logger
    /// </summary>
    /// 
    public enum Sender
    {
        Graphics,
        Physics,
        InputManager,
        KI,
        GameLogic,
        SoundEngine,
        Animationmanager,
        GameState,
        Animation,
        Collision,
        ParticleSystem,
        HUD,
        Game1,
        ObservableObject,
        SettingsManager,
        Model,
        StorageManager,
        ParticleSystemsManager,
        PowerupManager,
        TaskManager,
        AI,
        SuperController,
        Other
    };

    enum PriorityLevel
    {
        /// <summary>
        /// Information that is logged very often and is not very important. for example position of the player every 0.1 seconds
        /// </summary>
        Priority_0,
        /// <summary>
        /// events which influence the game directly and occur fairly often. e.g. user-input (keys, buttons) or collisions
        /// </summary>
        Priority_1,
        /// <summary>
        /// e.g. initialization of new objects or making objects visible
        /// </summary>
        Priority_2,
        /// <summary>
        /// events which influence the game but occure unfrequent e.g. game won...
        /// </summary>
        Priority_3,
        /// <summary>
        /// important messages or low priority errors
        /// </summary>
        Priority_4,
        /// <summary>
        /// critical errors e.g. texture couldn't be loaded, player is at a position where he's not allowed to be
        /// </summary>
        Priority_5,
    };

    /// <summary>
    /// Class giving an easy possibility to log messages.
    /// Standard: Only Messages of Level 3 and higher will be displayed!
    /// Singleton Pattern => use      Logger newLogger = Logger.Instance;
    /// </summary>
    class Logger
    {

        #region Properties

        /// <summary>
        /// Level of importance. No Message under the current Level will be displayed
        /// Clamped between 0 and 5, 5 is most important, 
        /// </summary>
        private int CurrentImportanceLevel { 
            get{return currentImportanceLevel;} 
            set{
                if(value<0) currentImportanceLevel = 0;
                else if(value>5) currentImportanceLevel = 5;
                else currentImportanceLevel = value;
                }
        }

        

       

        #endregion

        #region Members
        /// <summary>
        /// Level of importance. No Message under the current Level will be displayed
        /// Clamped between 0 and 5, 5 is most important, 
        /// </summary>
        private int currentImportanceLevel;


        /// <summary>
        /// List of Senders that are supposed to be displayed
        /// If list is empty, everything will be displayed!
        /// </summary>
        private List<Sender> viewSender;

        /// <summary>
        /// Instance of the Logger. Needed for singleton-pattern.
        /// </summary>
        private static Logger instance;

        /// <summary>
        /// Systemtime for logging the messages with the right timestamp
        /// </summary>
        private DateTime dt;
        #endregion

        #region Initialization

        /// <summary>
        /// Hidden Constructor as we use Singleton here.
        /// </summary>
       private Logger()
        {
            this.CurrentImportanceLevel = 5;
            this.viewSender = new List<Sender>();

           //select what Logs should be displayed
            //this.viewSender.Add(Sender.Game1);
            //this.viewSender.Add(Sender.GameState);
            //this.viewSender.Add(Sender.Model);
            //this.viewSender.Add(Sender.Test);
            //this.viewSender.Add(Sender.ObservableObject);
            //this.viewSender.Add(Sender.GameLogic);
            //this.viewSender.Add(Sender.PowerupManager);
           // this.viewSender.Add(Sender.TaskManager);
            //this.viewSender.Add(Sender.Physics);
            this.viewSender.Add(Sender.AI);

            dt = new DateTime();
            dt = DateTime.Now;

            Console.WriteLine("\nStarted Logging @ "+dt.Hour.ToString()+":"+dt.Minute.ToString()+":"+dt.Second.ToString()+":"+dt.Millisecond.ToString());
            Console.WriteLine("==========================================");
        }

        #endregion


        #region Methods

       /// <summary>
        /// Returns an Instance of the Logger
        /// </summary>
       public static Logger Instance
       {
           get
           {
               if (instance == null)
               {
                   instance = new Logger();
               }
               return instance;
           }
       }



        /// <summary>
        /// Print a logmessage to the console
        /// </summary>
        /// <param name="s">ID of the Sender</param>
        /// <param name="message"> The message</param>
        /// <param name="importance">Level of importance between 0 and 5. 5 Being most important. Messages of Level 5 will always be displayed</param>
        public void log(Sender s, String message, PriorityLevel importance)
        {
            //Always display level 5
            //Otherwise selected senders with a level bigger then current ImportanceLevel
            if ((int)importance == 5 || (int)importance >= this.CurrentImportanceLevel && (viewSender.Contains(s) || viewSender.Count==0))
            {
                if (message == "") message =senderToString(s)+ "-coder was too stupid to type a message for the logger";

                dt = DateTime.Now;
                Console.WriteLine(dt.Hour.ToString()+":"+dt.Minute.ToString()+":"+dt.Second.ToString()+":"+dt.Millisecond.ToString()+"\t"
                + senderToString(s)+"("+importance.ToString()+")\t"+message);
            }
        }



        /// <summary>
        /// Converts a Sender ID to a string
        /// </summary>
        /// <param name="s">ID of the sender</param>
        /// <returns>Name of Sender as String</returns>
        private String senderToString(Sender s)
        {
            switch (s)
            {
                case Sender.Graphics:
                    return "Graphics";
                case Sender.Physics:
                    return "Physics";
                case Sender.KI:
                    return "KI";
                case Sender.GameLogic:
                    return "GameLogic";
                case Sender.SoundEngine:
                    return "SoundEngine";
                case Sender.Animationmanager:
                    return "AnimationManager";
                case Sender.GameState:
                    return "GameState";
                case Sender.Animation:
                    return "Animation";
                case Sender.Collision:
                    return "Collision";
                case Sender.ParticleSystem:
                    return "ParticleSystem";
                case Sender.HUD:
                    return "HUD";
                case Sender.Game1:
                    return "Game1";
                case Sender.ObservableObject:
                    return "ObservableObject";
                case Sender.SettingsManager:
                    return "SettingsManager";
                case Sender.Model:
                    return "Model";
                case Sender.StorageManager:
                    return "StorageManager";
                case Sender.ParticleSystemsManager:
                    return "ParticleSystemsManager";
                case Sender.PowerupManager:
                    return "PowerUpManager";
                case Sender.TaskManager:
                    return "TaskManager";
                case Sender.AI:
                    return "AI";
                case Sender.SuperController:
                    return "SuperControler";
                case Sender.Other:
                    return "Other";
            }
            return "Undefined Sender";
        }

       #endregion


    }
}
