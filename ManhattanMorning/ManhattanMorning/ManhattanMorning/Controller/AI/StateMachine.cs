using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using ManhattanMorning.Misc;
using ManhattanMorning.Model.GameObject;


namespace ManhattanMorning.Controller.AI
{
    /// <summary>
    /// a state machine that handles the state-switching of one agent
    /// </summary>
    /// <typeparam name="entity_type">agent-type that is handled by the state. e.g. player</typeparam>
    public class StateMachine<entity_type>
    {

        public aState<entity_type> CurrentState { get { return currentState; } }
        public aState<entity_type> GlobalState { get { return globalState; } }
        public aState<entity_type> PreviousState { get { return PreviousState; } }





        //agent that owns this instance
        private entity_type owner;
        
        //the current state of the owner/agent
        private aState<entity_type> currentState;

        //a record of the last state the agent/owner was in
        private aState<entity_type> previousState;

        //this state logic is called every time the FSM is updated
        private aState<entity_type> globalState;

        /// <summary>
        /// constructor of a specific state-machine
        /// </summary>
        /// <param name="owner">agent that is handled by the state machine. e.g. the player</param>
        public StateMachine(entity_type owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// sets the state of the agent, use for initializing. For changing states see changeState(State...)
        /// </summary>
        /// <param name="s">the new state</param>
        public void setCurrentState(aState<entity_type> s)
        {
            currentState = s;
            s.enter();
        }

        /// <summary>
        /// sets the global state of the agent; use for initializing
        /// </summary>
        /// <param name="s">the new global state</param>
        public void setGlobalState(aState<entity_type> s)
        {
            globalState = s;
        }


        /// <summary>
        /// sets the previous state of the agent; use for initializing
        /// </summary>
        /// <param name="s">the new previous state</param>
        public void setPreviousState(aState<entity_type> s)
        {
            previousState = s;
        }

        /// <summary>
        /// updates the stateMachine
        /// </summary>
        /// <param name="ballList">List of all balls in the level</param>
        public void update(LayerList<Ball> ballList)
        {
            // if a global state exists, call its execute method
            if (globalState != null) globalState.execute(ballList);

            // if a currentstate exists, call its execute method
            if (currentState != null) currentState.execute(ballList);
            
        }


        /// <summary>
        /// change the currentState of the agent
        /// if Agent.cs of ManhattanMorning ist used it is used like this:
        /// StateMachine.setCurrentState(AvailableStates[(int)StateName.AIDefault]);
        /// 
        /// </summary>
        /// <param name="newState"> the new state</param>
        public void changeState(aState<entity_type> newState)
        {
            if (newState == null)
            {
                Logger.Instance.log(Sender.AI, "trying to change to a null state", PriorityLevel.Priority_5);
                return;
            }

            //keep a record of the previous state
            previousState = currentState;

            //call the exit method of the existing stae
            if(currentState != null)
            currentState.exit();

            //change state to new state
            currentState = newState;

            //call the entry method of the new state
            currentState.enter();


        }

        /// <summary>
        /// change state back to previous state
        /// </summary>
        /// <param name="levelSize">The size of the level</param>
        public void revertToPreviousState(Vector2 levelSize)
        {
            changeState(previousState);
        }


        /// <summary>
        /// returns true if the current state's type is equal to the type of the class passed as a parameter. 
        /// CAUTION: NOT SURE IF THIS METHOD WORKS CORRECTLY!
        /// </summary>
        /// <param name="st">State of which the type is tested</param>
        /// <returns>true if type of state parameter = class type parameter</returns>
        public bool isInState(aState<entity_type> st)
        {
            return (st.GetType() == this.GetType());

        }


    }
}
