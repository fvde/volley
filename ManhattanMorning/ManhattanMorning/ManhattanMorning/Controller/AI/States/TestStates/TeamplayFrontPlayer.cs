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
    class TeamplayFrontPlayer : aState<Agent>
    {
        /// <summary>
        /// time before the ai is allowed to switch in milliseconds
        /// </summary>
        private double positionSwitchTime;

        public override void enter()
        {
            // position switch every 3-7 sec
            positionSwitchTime = 3000 + AI.Random.Next(4000);
            agent.lastStateChange = AI.gameTime.TotalGameTime.TotalMilliseconds;
            Logger.Instance.log(Sender.AI, "Player " + agent.ControlledPlayer.PlayerIndex + " entering state: TeamplayFrontPlayer", PriorityLevel.Priority_4);
        }

        public override void execute(LayerList<Ball> ballList)
        {
            

            // if there is no ball, do nothing
            if (ballList.Count == 0)
                return;

            //adjust position if the human takes over the front or back position
            if (agent.Teammate.isHuman)
            {
                if (agent.ControlledPlayer.Team == 1 && agent.Teammate.ControlledPlayer.Body.Position.X > levelSize.X * 0.25f + agent.ControlledPlayer.Size.X
                    || agent.ControlledPlayer.Team == 2 && agent.Teammate.ControlledPlayer.Body.Position.X < levelSize.X * 0.75 - agent.ControlledPlayer.Size.X)
                {
                    agent.StateMachine.changeState(agent.AvailableStates[(int)StateName.TeamplayBackPlayer]);
                }
            }
            // switch position with teammate when close to each other and the last switch happend more than positionSwitchTime ago.
            else
            {
                if (AI.gameTime.TotalGameTime.TotalMilliseconds - agent.lastStateChange > positionSwitchTime
                    && Math.Abs(agent.Teammate.ControlledPlayer.Body.Position.X - agent.ControlledPlayer.Body.Position.X) < agent.ControlledPlayer.Size.X *2)
                                  
                {
                    agent.StateMachine.changeState(agent.AvailableStates[(int)StateName.TeamplayBackPlayer]);
                    agent.Teammate.StateMachine.changeState(agent.Teammate.AvailableStates[(int)StateName.TeamplayFrontPlayer]);
                }

            }

            
            //select ball to handle
            selectBall(ballList);
      
            handleMovement();
            handleHand();
            handleJump();
            

        }

        /// <summary>
        /// Handles movement of a AI player (covering the front of the teams levelhalf)
        /// </summary>
        public override void handleMovement()
        {
            float estBallPosition = estimatedBallXPosition(observedBall, (float)SettingsManager.Instance.get("playerSize") * 2);
            int a = confusionHandling();


            //let the player run towards the estimated ball position
            if (agent.ControlledPlayer.Team == 1)
            {
                if (estBallPosition <= levelSize.X * 0.5 - observedBall.Size.X && (estBallPosition >= levelSize.X * 0.25 || (agent.Teammate.StateMachine.CurrentState.observedBall != observedBall && !agent.Teammate.isHuman)))
                {

                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2((estBallPosition - (agent.ControlledPlayer.Size.X / 4) - agent.ControlledPlayer.Body.Position.X), 0));
                }
                else
                {
                    //searching for powerups in the level which are on the players side
                    if (SuperController.Instance.getAllPowerups() != null)
                    {
                        if (SuperController.Instance.getAllPowerups().Count != 0)
                        {
                            
                            foreach (PowerUp p in SuperController.Instance.getAllPowerups())
                            {
                                if (p.Body.Position.X < levelSize.X * 0.5 + p.Size.X && p.Body.Position.Y < levelSize.Y * 0.5)
                                {
                                   
                                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2(p.Body.Position.X - agent.ControlledPlayer.Body.Position.X, 0));
                                    break;
                                }
                            }
                        }
                        else
                        {
                            InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * getSmoothRandomMovementVec(levelSize.X * 0.375f, agent.ControlledPlayer.Body.Position.X));

                        }
                    }
                }
            }
            else if (agent.ControlledPlayer.Team == 2)
            {
                if (estBallPosition >= levelSize.X * 0.5 + observedBall.Size.X && (estBallPosition <= levelSize.X * 0.75 || (agent.Teammate.StateMachine.CurrentState.observedBall != observedBall && !agent.Teammate.isHuman)))
                {

                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2((estBallPosition + (agent.ControlledPlayer.Size.X / 4) - agent.ControlledPlayer.Body.Position.X), 0));
                }
                else
                {
                    if (SuperController.Instance.getAllPowerups().Count != 0)
                    {

                        foreach (PowerUp p in SuperController.Instance.getAllPowerups())
                        {
                            if (p.Body.Position.X > levelSize.X * 0.5 - p.Size.X && p.Body.Position.Y < levelSize.Y * 0.5)
                            {
                                
                                InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex,a *  new Vector2(p.Body.Position.X - agent.ControlledPlayer.Body.Position.X, 0));
                                break;
                            }
                        }
                    }
                    else
                    {

                        InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex,a * getSmoothRandomMovementVec(levelSize.X * 0.625f, agent.ControlledPlayer.Body.Position.X));
                    }
                }
            }
        }


        private void determineAssignement()
        {

        }

        public override void exit()
        {
            Logger.Instance.log(Sender.AI, "Player " + agent.ControlledPlayer.PlayerIndex + " exit state TeamplayFrontPlayer", PriorityLevel.Priority_4);
        }


        public TeamplayFrontPlayer(Agent agent, Vector2 levelSize)
        {
            this.agent = agent;
            this.levelSize = levelSize;
        }


    }
}
