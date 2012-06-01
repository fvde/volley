using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManhattanMorning.Misc;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Misc.Tasks;

using Microsoft.Xna.Framework;

namespace ManhattanMorning.Controller
{
    /// <summary>
    /// The TaskManager serves as a medium for controller interaction. All controllers can send messages to the TM
    /// whenever they need a GUI element shown, a effect created or some other logic event.
    /// The TM then manages that message and eventually sends it to the specified recipient.
    /// 
    /// HOW TO ADD A TASK:
    /// Make sure the class yourClassTask exists. If it doesnt create it in ManhattenMorning/Misc/Tasks. Make sure it inherits from Task.
    /// Now you can create Tasks and add them to the TaskManager.
    /// 
    /// HOW TO RECEIVE TASKS:
    /// In your update() method call getTasks() and then do with your messages whatever you want.
    /// 
    /// For more instructions have a look at: "Dokumentation des Taskmanager.pdf" in ManhattanMorning/Documentation
    /// 
    /// </summary>
    class TaskManager : IController
    {

        #region Properties

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static TaskManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TaskManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// List of all the tasks for that graphics that have to be processed in the next update(). MAKE SURE TO REMOVE PROCESSED TASKS!
        /// </summary>
        public List<GraphicsTask> GraphicTasks { get { return graphicTasks; } set { graphicTasks = value; } }

        /// <summary>
        /// List of all the tasks for the game logic that have to be processed in the next update(). MAKE SURE TO REMOVE PROCESSED TASKS!
        /// </summary>
        public List<GameLogicTask> GameLogicTasks { get { return gameLogicTasks; } set { gameLogicTasks = value; } }

        /// <summary>
        /// List of all the tasks for the input processor that have to be processed in the next update(). MAKE SURE TO REMOVE PROCESSED TASKS!
        /// </summary>
        public List<InputManagerTask> InputManagerTasks { get { return inputManagerTasks; } set { inputManagerTasks = value; } }

        /// <summary>
        /// List of all the tasks for the physics that have to be processed in the next update(). MAKE SURE TO REMOVE PROCESSED TASKS!
        /// </summary>
        public List<PhysicsTask> PhysicsTasks { get { return physicsTasks; } set { physicsTasks = value; } }

        /// <summary>
        /// List of all the tasks for the power up manager that have to be processed in the next update(). MAKE SURE TO REMOVE PROCESSED TASKS!
        /// </summary>
        public List<PowerUpManagerTask> PowerUpManagerTasks { get { return powerUpManagerTasks; } set { powerUpManagerTasks = value; } }

        /// <summary>
        /// List of all the tasks for the sound manager that have to be processed in the next update(). MAKE SURE TO REMOVE PROCESSED TASKS!
        /// </summary>
        public List<SoundTask> SoundTasks { get { return soundTasks; } set { soundTasks = value; } }

        /// <summary>
        /// List of all the tasks for the particle system manager that have to be processed in the next update(). MAKE SURE TO REMOVE PROCESSED TASKS!
        /// </summary>
        public List<ParticleSystemTask> ParticleSystemTasks { get { return particleSystemTasks; } set { particleSystemTasks = value; } }

        #endregion

        #region Members

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        private static TaskManager instance;

        /// <summary>
        /// Indicates whether this controller is paused.
        /// </summary>
        private bool paused;

        /// <summary>
        /// List of all tasks that are currently stored and not yet at curentTime == 0.
        /// </summary>
        private List<Task> pendingTasks;

        /// <summary>
        /// List of all the tasks that have to be executed in the next update() of the Graphic.
        /// </summary>
        private List<GraphicsTask> graphicTasks;

        /// <summary>
        /// List of all the tasks that have to be executed in the next update() of the game logic.
        /// </summary>
        private List<GameLogicTask> gameLogicTasks;

        /// <summary>
        /// List of all the tasks that have to be executed in the next update() of the input manager.
        /// </summary>
        private List<InputManagerTask> inputManagerTasks;

        /// <summary>
        /// List of all the tasks that have to be executed in the next update() of the pyhsics.
        /// </summary>
        private List<PhysicsTask> physicsTasks;

        /// <summary>
        /// List of all the tasks that have to be executed in the next update() of the power up manager.
        /// </summary>
        private List<PowerUpManagerTask> powerUpManagerTasks;

        /// <summary>
        /// List of all the tasks that have to be executed in the next update() of the sound manager.
        /// </summary>
        private List<SoundTask> soundTasks;

        /// <summary>
        /// List of all the tasks that have to be executed in the next update() of the particle system manager.
        /// </summary>
        private List<ParticleSystemTask> particleSystemTasks;


        #endregion


        #region Initialization

        /// <summary>
        /// Initializes TaskManager
        /// hidden because of Singleton Pattern
        /// </summary>
        private TaskManager()
        {
            pendingTasks = new List<Task>();
            graphicTasks = new List<GraphicsTask>();
            physicsTasks = new List<PhysicsTask>();
            inputManagerTasks = new List<InputManagerTask>();
            gameLogicTasks = new List<GameLogicTask>();
            powerUpManagerTasks = new List<PowerUpManagerTask>();
            soundTasks = new List<SoundTask>();
            particleSystemTasks = new List<ParticleSystemTask>();
            paused = false;
        }

        #endregion


        #region Methods

        /// <summary>
        /// is called by game1, all the functionality is implemented
        /// in this method or a submethod
        /// </summary>
        public void update(GameTime gameTime)
        {
            if (!paused)
            {
                updateTaskTimes(gameTime.ElapsedGameTime.Milliseconds);
            }
        }

        /// <summary>
        /// Updates the time of all tasks.
        /// </summary>
        /// <param name="passedMilliseconds"></param>
        private void updateTaskTimes(int passedMilliseconds)
        {
            for (int x = 0; x < pendingTasks.Count; x++)
            {
                pendingTasks[x].CurrentTime -= passedMilliseconds;
                if (pendingTasks[x].CurrentTime <= 0)
                {
                    processTask(pendingTasks[x]);
                }
            }
        }

        /// <summary>
        /// Adds a task to the tasks that have to be processed in the next update().
        /// </summary>
        /// <param name="task"></param>
        private void processTask(Task task)
        {
            pendingTasks.Remove(task);
            addTaskToCorrespondingList(task);
            Logger.Instance.log(Sender.TaskManager, "Started Task", PriorityLevel.Priority_2);
        }

        /// <summary>
        /// Add a task to the Task Manager.
        /// </summary>
        /// <param name="task"></param>
        public void addTask(Task task)
        {
            //if (!paused)
            {
                if (task.CurrentTime == 0)
                {
                    addTaskToCorrespondingList(task);
                    Logger.Instance.log(Sender.TaskManager, "Started Task", PriorityLevel.Priority_2);
                }
                else
                {
                    pendingTasks.Add(task);
                    Logger.Instance.log(Sender.TaskManager, "Added task", PriorityLevel.Priority_2);
                }
            }
        }

        /// <summary>
        /// Remove a task from the list of pending tasks. To remove a task from the list of all the tasks that will
        /// be processed in the next update() you have to access the corresponding list directly.
        /// </summary>
        /// <param name="task"></param>
        public void removeTask(Task task)
        {
            pendingTasks.Remove(task);
        }

        /// <summary>
        /// Resets the TaskManager.
        /// </summary>
        public void clear()
        {
            pendingTasks.Clear();
            graphicTasks.Clear();
            physicsTasks.Clear();
            inputManagerTasks.Clear();
            gameLogicTasks.Clear();
            powerUpManagerTasks.Clear();
            soundTasks.Clear();
            particleSystemTasks.Clear();
        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        public void initialize()
        {

        }

        /// <summary>
        /// Pauses the TaskManager.
        /// </summary>
        /// <param name="on">true := paused.</param>
        public void pause(bool on)
        {
            paused = on;
        }

        #endregion

        #region Helper Methods

        private void addTaskToCorrespondingList(Task task)
        {
            if (task is GraphicsTask)
            {
                graphicTasks.Add((GraphicsTask)task);
            }
            else if (task is InputManagerTask){
                inputManagerTasks.Add((InputManagerTask)task);
            }
            else if (task is GameLogicTask){
                gameLogicTasks.Add((GameLogicTask)task);
            }
            else if (task is PhysicsTask){
                physicsTasks.Add((PhysicsTask)task);
            }
            else if (task is PowerUpManagerTask){
                powerUpManagerTasks.Add((PowerUpManagerTask)task);
            }
            else if (task is SoundTask){
                soundTasks.Add((SoundTask)task);
            }
            else if (task is ParticleSystemTask){
                particleSystemTasks.Add((ParticleSystemTask)task);
            }
            else {
                Logger.Instance.log(Sender.TaskManager, "Couldn't identify sender!", PriorityLevel.Priority_5);
            }

        }

        #endregion


    }
}
