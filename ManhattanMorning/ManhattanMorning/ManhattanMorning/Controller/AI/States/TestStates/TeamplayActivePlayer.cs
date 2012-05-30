using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ManhattanMorning.Misc;
using ManhattanMorning.Controller;
using ManhattanMorning.Model.GameObject;

namespace ManhattanMorning.Controller.AI.States.TestStates
{
    /// <summary>
    /// Represents the logic for the ai Player in the team wich is currently going for the ball
    /// </summary>
    class TeamplayActivePlayer : aState<Agent>
    {

        public  TeamplayActivePlayer(Agent agent, Vector2 levelSize)
        {
            this.agent = agent;
            this.levelSize = levelSize;
        }

        public override void enter()
        {
            Logger.Instance.log(Sender.AI,"Player " + agent.ControlledPlayer.PlayerIndex + " entering state: TeamplayActivePlayer", PriorityLevel.Priority_5);
        }

        public override void execute( LayerList<Ball> ballList)
        {
            // if there is no ball, do nothing
            if (ballList.Count == 0)
                return;

            

            // (Maybe there is another one because of the powerup -> must be handled by KI somehow)
            observedBall = ballList[0];

            //testcomment for hg

            handleMovement();
            handleHand();
            handleJump();
            // throw new NotImplementedException();

        }


        private void determineAssignement()
        {
            
        }

        public override void exit()
        {
            Logger.Instance.log(Sender.AI, "Player " + agent.ControlledPlayer.PlayerIndex + " exit state TeamplayActivePlayer", PriorityLevel.Priority_5);
        }

        /// <summary>
        /// Switches the assignements of the players in one team (active/passive).
        /// </summary>
        private void switchTeamAssignements(Agent agent)
        {
            agent.Teammate.StateMachine.changeState(agent.Teammate.AvailableStates[(int)StateName.TeamplayActivePlayer]);
            agent.StateMachine.changeState(agent.AvailableStates[(int)StateName.TeamplayPassivePlayer]);
        }


        

        

        
    }
}
