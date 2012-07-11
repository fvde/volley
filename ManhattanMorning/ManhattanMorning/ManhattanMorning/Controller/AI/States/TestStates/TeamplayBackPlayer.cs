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
    class TeamplayBackPlayer : aState<Agent>
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
            Logger.Instance.log(Sender.AI, "Player " + agent.ControlledPlayer.PlayerIndex + " entering state: TeamplayBackPlayer", PriorityLevel.Priority_4);
        }

        public override void execute(LayerList<Ball> ballList)
        {
            // if there is no ball, do nothing
            if (ballList.Count == 0)
                return;

            //adjust position if the human takes over the front or back position
            if (agent.Teammate.isHuman)
            {
                
                if (agent.ControlledPlayer.Team == 1 && agent.Teammate.ControlledPlayer.Body.Position.X < levelSize.X * 0.25f - agent.ControlledPlayer.Size.X
                    || agent.ControlledPlayer.Team == 2 && agent.Teammate.ControlledPlayer.Body.Position.X > levelSize.X * 0.75 + agent.ControlledPlayer.Size.X)
                {
                    agent.StateMachine.changeState(agent.AvailableStates[(int)StateName.TeamplayFrontPlayer]);
                }
            }
            // switch position with teammate when close to each other and the last switch happend more than positionSwitchTime ago.
            else
            {

                if (AI.gameTime.TotalGameTime.TotalMilliseconds -  agent.lastStateChange> positionSwitchTime
                    && Math.Abs(agent.Teammate.ControlledPlayer.Body.Position.X - agent.ControlledPlayer.Body.Position.X) < agent.ControlledPlayer.Size.X * 2)
                {
                    agent.StateMachine.changeState(agent.AvailableStates[(int)StateName.TeamplayFrontPlayer]);
                    agent.Teammate.StateMachine.changeState(agent.Teammate.AvailableStates[(int)StateName.TeamplayBackPlayer]);
                }

            }

            //select a ball to handle
            selectBall(ballList);
            
            handleMovement();
            handleHand();
            handleJump();
            // throw new NotImplementedException();

        }

       
        private float direction, lastRandomPos;

        /// <summary>
        /// Handles movement of a AI player (covering the front of the teams levelhalf)
        /// </summary>
        public override void handleMovement()
        {
            float estBallPosition = estimatedBallXPosition(observedBall, (float)SettingsManager.Instance.get("playerSize") * 2);

            int a = confusionHandling();
            List<PowerUp> powerUps = SuperController.Instance.getAllPowerups();

            //let the player run towards the estimated ball position
            if (agent.ControlledPlayer.Team == 1)
            {
                if (estBallPosition <= levelSize.X * 0.25 || (estBallPosition < levelSize.X * 0.5 && (observedBall != agent.Teammate.StateMachine.CurrentState.observedBall && !agent.Teammate.isHuman)))
                {
                  
                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex,a *  new Vector2((estBallPosition - (agent.ControlledPlayer.Size.X / 4) - agent.ControlledPlayer.Body.Position.X), 0));
                }
                else
                {
                    //searching for powerups in the level which are on the players side
                    if (!chaseBomb(a))
                    {
                        if (powerUps != null && powerUps.Count != 0)
                        {
                            
                            foreach (PowerUp p in powerUps)
                            {
                                if (p.Body.Position.X < levelSize.X * 0.5 + p.Size.X )
                                {
                                 
                                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a* new Vector2(p.Body.Position.X - agent.ControlledPlayer.Body.Position.X, 0));
                                    break;
                                }
                            }
                        }
                        else
                        {
                            InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex,a * getSmoothRandomMovementVec(levelSize.X * 0.125f, agent.ControlledPlayer.Body.Position.X));

                        }
                    }
                }
            }
            else if (agent.ControlledPlayer.Team == 2)
            {
                if (estBallPosition >= levelSize.X * 0.75 || (estBallPosition > levelSize.X * 0.5 && (observedBall != agent.Teammate.StateMachine.CurrentState.observedBall && !agent.Teammate.isHuman)))
                {

                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2((estBallPosition + (agent.ControlledPlayer.Size.X / 4) - agent.ControlledPlayer.Body.Position.X), 0));
                }
                else
                {
                    //searching for powerups in the level which are on the players side
                    if (!chaseBomb(a))
                    {
                        if (powerUps != null && powerUps.Count != 0)
                        {

                            foreach (PowerUp p in powerUps)
                            {
                                if (p.Body.Position.X > levelSize.X * 0.5 - p.Size.X )
                                {

                                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2(p.Body.Position.X - agent.ControlledPlayer.Body.Position.X, 0));
                                    break;
                                }
                            }
                        }
                        else
                        {

                            InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * getSmoothRandomMovementVec(levelSize.X * 0.875f, agent.ControlledPlayer.Body.Position.X));
                        }
                    }
                }
            }
        }



        public override void exit()
        {
            Logger.Instance.log(Sender.AI, "Player " + agent.ControlledPlayer.PlayerIndex + " exit state TeamplayBackPlayer", PriorityLevel.Priority_4);
        }


        public TeamplayBackPlayer(Agent agent, Vector2 levelSize)
        {
            this.agent = agent;
            this.levelSize = levelSize;
        }


    }
}
