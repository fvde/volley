using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Controller.AI.States;
using ManhattanMorning.Controller.AI.States.TestStates;
using ManhattanMorning.Misc;



namespace ManhattanMorning.Controller.AI
{

    /// <summary>
    /// enum used for organizing the array with the available states
    /// this is usefull to avoid ugly magic state-changes like statemachine.changeState(AvailableStates[1])
    /// </summary>
   public enum StateName { MasterAImadeByBoss, TeamplayActivePlayer, TeamplayPassivePlayer, AIOidle, testState, AIDefault, TeamplayFrontPlayer, TeamplayBackPlayer }


   /// <summary>
   /// indicates the confusion state for inverted control
   /// </summary>
   public enum ConfusionState {

       /// <summary>
       /// inverted control not active
       /// </summary>
       None,

       /// <summary>
       /// heading in the wrong direction
       /// </summary>
       Confused,

       /// <summary>
       /// inverted control avtive but heading in the right direction (e.g. for one move)
       /// </summary>
       temporarilyRight
   
   };

    public class Agent
    {
        /// <summary>
        /// The player controlled by the agent
        ///
        /// </summary>
        public Player ControlledPlayer;


        /// <summary>
        /// true if the assigned player is not controlled by the ai
        /// </summary>
        public bool isHuman = false;
        

        /// <summary>
        /// the player wich is in the same team as the controlled player, this will (hopefully) be used for playing two on two
        /// </summary>
        public Agent Teammate;


        /// <summary>
        /// the moment of the last state-change in milliseconds (from game start)
        /// </summary>
        public double lastStateChange;



        /// <summary>
        /// The stateMachine that handles the states of the player/agent (it's logic)
        /// </summary>
        public StateMachine<Agent> StateMachine;


        /// <summary>
        /// The states which can be entered by the agent, length is the number of states available
        /// </summary>
        public aState<Agent>[] AvailableStates = new aState<Agent>[10];


        /// <summary>
        /// the time in ms a player will continue to move in the wrong or right direction depending on the confusionState (with inverted control active)
        /// 
        public double confusedMovementTime;


        /// <summary>
        /// confusionstate handling inverted control
        /// </summary>
        public ConfusionState confusionState = ConfusionState.None;


        /// <summary>
        /// sets the teammate player of the agent
        /// </summary>
        /// <param name="teammate">the agents teammate</param>
        public void setTeammate(Agent teammate)
        {
            this.Teammate = teammate;
        }

        /// <summary>
        /// creates a new agent representing one players ai
        /// </summary>
        /// <param name="controlledPlayer">the player controller by this ai-agent</param>
        /// <param name="levelSize">The size of the actal level</param>
        public Agent(Player controlledPlayer, Vector2 levelSize)
        {
            //initializing the used States
            AvailableStates[(int)StateName.MasterAImadeByBoss] = new MasterAImadeByBoss(this,levelSize);
            AvailableStates[(int)StateName.TeamplayActivePlayer] = new TeamplayActivePlayer(this,levelSize);
            AvailableStates[(int)StateName.TeamplayPassivePlayer] = new TeamplayPassivePlayer(this,levelSize);
            AvailableStates[(int)StateName.AIOidle] = new AIOidle(this,levelSize);
            AvailableStates[(int)StateName.testState] = new testState(this,levelSize);
            AvailableStates[(int)StateName.AIDefault] = new AIDefault(this,levelSize);
            AvailableStates[(int)StateName.TeamplayFrontPlayer] = new TeamplayFrontPlayer(this, levelSize);
            AvailableStates[(int)StateName.TeamplayBackPlayer] = new TeamplayBackPlayer(this, levelSize);

            lastStateChange = AI.gameTime.TotalGameTime.TotalMilliseconds;

             this.ControlledPlayer = controlledPlayer;
             StateMachine = new StateMachine<Agent>(this);
            //StateMachine.setCurrentState(AvailableStates[(int)StateName.testState]);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ballList">list of all balls of the current level</param>
        public void update(LayerList<Ball> ballList)
        {
            if (isHuman)
                return;

            StateMachine.update(ballList);
        }

    }
}
