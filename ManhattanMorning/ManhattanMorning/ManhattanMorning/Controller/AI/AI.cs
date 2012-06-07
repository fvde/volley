using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using ManhattanMorning.Misc;
using ManhattanMorning.Controller;
using ManhattanMorning.Model.GameObject;

namespace ManhattanMorning.Controller.AI
{
    /// <summary>
    /// KI Engine is responsible for
    /// controlling all KI players
    /// </summary>
    class AI : IController
    {

        #region Properties

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static AI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AI();
                }
                return instance;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// a random generator
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        private static AI instance;

        /// <summary>
        /// Indicates if controller is paused at the moment
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// a list that contains one agent for every ai-player that is active
        /// </summary>
        private List<Agent> agentList = new List<Agent>();

        public static GameTime gameTime;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes KI Manager
        /// hidden because of Singleton Pattern
        /// </summary>
        private AI()
        {

            // set members
            isPaused = true;
            gameTime = new GameTime();
        }

        /// <summary>
        /// Called by game1
        /// Creates the instance at the beginning of the program
        /// </summary>
        public void Init()
        {
            Logger.Instance.log(Sender.KI, "initialized",PriorityLevel.Priority_2);

        }

        /// <summary>
        /// Necessary for interface, but mustn't be called
        /// </summary>
        public void initialize()
        {
            throw new Exception("Wrong AI initialization");
        }

        /// <summary>
        /// initialized the ai, especially the agents for each ai-player in a match
        /// <param name="playerList">List with all players of the level</param>
        /// <param name="levelSize">Size of the actual level</param>
        /// </summary>
        public void initialize(LayerList<Player> playerList, Vector2 levelSize)
        {

            foreach (Player player in playerList)
            {
              
                
                    Agent agent = new Agent(player, levelSize);
                    if (player.Status != PlayerStatus.KIPlayer)
                        agent.isHuman = true;
                    agentList.Add(agent);                   
                
            }

            foreach (Agent agent in agentList)
            {
                foreach (Agent a in agentList)
                {
                    if (a.ControlledPlayer.Team == agent.ControlledPlayer.Team && agent.ControlledPlayer.PlayerIndex != a.ControlledPlayer.PlayerIndex)
                    {
                        agent.setTeammate(a);
                        break;
                    }
                }

                agent.StateMachine.setCurrentState(agent.AvailableStates[(int)StateName.testState]);
            }
            return;
        }

        #endregion

        /// <summary>
        /// clears the ki-controller; deletes the agents in the agent-list
        /// sets isPaused = true
        /// </summary>
        public void clear()
        {
            this.agentList = new List<Agent>();
            this.isPaused = true;
        }

        #region Methods

        /// <summary>
        /// is called by game1, all the functionality is implemented
        /// in this method or a submethod
        /// </summary>
        /// <param name="ballList">list of all current balls</param>
        /// <param name="gameTime">The game time.</param>
        public void update(LayerList<Ball> ballList, GameTime gameTime)
        {

            // check if controller is paused
            if (!isPaused)
            {
               AI.gameTime = gameTime;
                foreach (Agent agent in agentList)
                {

                    agent.update(ballList);
                }

            }

        }


        /// <summary>
        /// Forces controller to pause, has to make sure that all timers etc. pause
        /// </summary>
        /// <param name="on">true: controller has to pause, false: controller has to work </param>
        public void pause(bool on)
        {
            isPaused = on;
        }

        #endregion

       
        #region HelperMethods
        // KI Processing should be divided in different actions for every player


        #endregion


    }
}
