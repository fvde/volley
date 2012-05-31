using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using ManhattanMorning.Misc;
using ManhattanMorning.Controller;
using ManhattanMorning.Model.GameObject;


namespace ManhattanMorning.Controller.AI.States
{
    class testState : aState<Agent>
    {


        public override void enter() {

            


            Logger.Instance.log(Sender.AI, "Player " + agent.ControlledPlayer.PlayerIndex + " entering state: testState", PriorityLevel.Priority_4);

            if (agent.Teammate != null)
            {
                if (agent.Teammate.StateMachine.CurrentState == null)
                {
                    agent.StateMachine.changeState(agent.AvailableStates[(int)StateName.TeamplayFrontPlayer]);
                }
                else
                {
                    agent.StateMachine.changeState(agent.AvailableStates[(int)StateName.TeamplayBackPlayer]);
                }
            }
        }

        public override void execute( LayerList<Ball> ballList)
        {
            

            // if there is no ball, do nothin
            if (ballList.Count == 0)
                return;

            //select the ball to handle

            selectBall(ballList);

            

            handleMovement();
            handleHand();
            handleJump();
            // throw new NotImplementedException();

        }

        public override void exit()
        {
            Logger.Instance.log(Sender.AI, "Player " + agent.ControlledPlayer.PlayerIndex + " exit state: testState", PriorityLevel.Priority_4);
            //throw new NotImplementedException();
        }




        public testState(Agent agent, Vector2 levelSize)
        {
            this.agent = agent;
            this.levelSize = levelSize;
        }
    }
}
