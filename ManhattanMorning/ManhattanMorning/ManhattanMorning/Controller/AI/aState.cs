using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using ManhattanMorning.Misc;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.View;

namespace ManhattanMorning.Controller.AI
{



    /// <summary>
    /// model for a state. should be inherited by all states desinging the ai
    /// Every state inheriting from this class should be enlistet in the enum "StateName" in Agent.cs in order to be usable by the AI.
    /// </summary>
    /// <typeparam name="entity_type">agent-type that is handled by the state. e.g. player</typeparam>
   public abstract class aState<entity_type>
   {

       public Agent agent;

       public Ball observedBall;

       private Vector2 lastMovementVec = new Vector2 (0,1f);

        /// <summary>
        /// The Size of the current Level
        /// </summary>
        public Vector2 levelSize;

        /// <summary>
        /// actions to perform when entering the state
        /// </summary>
         public abstract void enter();

         /// <summary>
         /// behavior of the agent when it is in this state
         /// </summary>
         /// <param name="ballList">list of all balls</param>
         public abstract void execute( LayerList<Ball> ballList);

         /// <summary>
         /// actions to perform when executing the state
         /// </summary>
        public abstract  void exit();



        #region AIBasicFunctions

       

        /// <summary>
       /// triggers a single jump for a given player
       /// </summary>
       /// <param name="p">the player</param>
        public void jump(){
            InputManager.Instance.triggerJump(agent.ControlledPlayer.PlayerIndex);
        }


       /// <summary>
       /// let a player hit a ball
       /// </summary>
       /// <param name="player">player</param>
       /// <param name="ball">ball</param>
        public void hitBall()
        {
            InputManager.Instance.setHandAmplitude(agent.ControlledPlayer.PlayerIndex, observedBall.Body.Position - agent.ControlledPlayer.HandBody.Position);
        }



        
        /// <summary>
        /// the estimated Position of the ball when it reaches a specific height
        /// </summary>
        /// <param name="ball">the ball</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public  float estimatedBallXPosition(Ball ball, float height)
        {
            //pls do NOT DELETE the OUTCOMMENTED lines, they are for the stupid programmer (Bob)

            
            float b = -ball.Body.LinearVelocity.Y;
            float g = ((Vector2)SettingsManager.Instance.get("gravity")).Y;
            float a = -g/2;
            float c = (-ball.Body.Position.Y) + (levelSize.Y * 0.95f - height);

            //handle the case that sqrt returns NaN because b*b - 4*a*c < 0 => c<0
            if (c < 0)
            {

                c = (-ball.Body.Position.Y) + (levelSize.Y * 0.95f - height / 2);
                if (c < 0)
                {

                    c = (-ball.Body.Position.Y) + (levelSize.Y * 1.042f);
                }

            }
            
            float x = 0;
            
           // if(ball.Body.LinearVelocity.X >0)
             x = ball.Body.LinearVelocity.X * ((-b - (float)Math.Sqrt(b*b - 4*a*c))/(2*a)) + ball.Body.Position.X;
           // else
            // x = ball.Body.LinearVelocity.X * ((-b - (float)Math.Sqrt(b*b - 4*a*c))/(2*a)) + ball.Body.Position.X;

           // float x = ball.Body.Position.X + (ball.Body.LinearVelocity.X * ((ball.Body.LinearVelocity.Y + (float)Math.Sqrt(ball.Body.LinearVelocity.Y + 2 * ((Vector2)SettingsManager.Instance.get("gravity")).Y * (ball.Body.Position.Y - y))) / ((Vector2)SettingsManager.Instance.get("gravity")).Y));
            //Logger.Instance.log(Sender.AI, "estimated:   " + x, PriorityLevel.Priority_5)


            //if the ball will bounce againts the level borders, calculate the position as if it will be reflected with no friction
            //actually there is friction, but it's
            //close enough...

            //bouncing against the right border
             if (x > levelSize.X)
                 x = levelSize.X - (x - levelSize.X);

            //left border
             else if (x < 0)
                 x *= -1;

            return x;
        }

       /// <summary>
       /// selects a ball from the ballList in the level.
       /// Ai will chase a ball which will land in its half or the first one in the list
       /// </summary>
       /// <param name="ballList">the ball this agent is going after</param>
       public void selectBall(List<Ball> ballList){
           
           //return null if ballList doesn't exist
           if(ballList == null || ballList.Count == 0){
               throw new ArgumentException("BallList is 'null' or does not contain any balls, check if a ballList with at least one ball"
               + "exist before handing it over to selectBall()!");
           }
           
           //choose first ball of list
           observedBall = ballList[0];


           //left team
           if (agent.ControlledPlayer.Team == 1)
           {
               //if currently chosen ball will not land in the agent half search for another ball which will do so
               if (estimatedBallXPosition(observedBall, (float)SettingsManager.Instance.get("playerSize") * 2) > levelSize.X * 0.5)
               {
                   foreach (Ball b in ballList)
                   {
                       if (estimatedBallXPosition(b, (float)SettingsManager.Instance.get("playerSize") * 2) <= levelSize.X * 0.5 && b.Body.Position.X < levelSize.X + b.Size.X)
                       {
                          
                               if (agent.Teammate == null || agent.Teammate.StateMachine.CurrentState.observedBall != b || ballList.Count == 1)
                               {
                                   observedBall = b;
                                   break;
                               }
                           
                        

                       }
                   }
               }
           }
           // right team
           else
           {
               //if currently chosen ball will not land in the agent half search for another ball which will do so
               if (estimatedBallXPosition(observedBall, (float)SettingsManager.Instance.get("playerSize") * 2) < levelSize.X * 0.5)
               {
                   foreach (Ball b in ballList)
                   {
                       if (estimatedBallXPosition(b, (float)SettingsManager.Instance.get("playerSize") * 2) >= levelSize.X * 0.5  && b.Body.Position.X < levelSize.X - b.Size.X)
                       {
                           if (agent.Teammate == null || agent.Teammate.StateMachine.CurrentState.observedBall != b || ballList.Count ==1)
                           {
                               observedBall = b;
                               break;
                           }

                       }
                   }
               }


           }


   
       }


       /// <summary>
       /// handles inverted control powerup. The AI gehts confused if the control is inverted.
       /// the method calculates if the ai steers in the wrong or right direction
       /// </summary>
       /// <returns>1 if the ai should run the right direction, -1 if the ai should run in the wrong direction</returns>
        public int confusionHandling()
        {
            int a = 1;

            //only calculate if inverted control is active
            if (agent.ControlledPlayer.Flags.Contains(PlayerFlag.InvertedControl))
            {
                //
                if (agent.confusionState == ConfusionState.None)
                {
                    agent.confusedMovementTime = 1000 + AI.Random.Next(1000);
                    agent.confusionState = ConfusionState.Confused;
                }
                // amount of time for confusion was set, decrease the time every step, set a = -1 for movement in opposite direction,
                // switch to a phase of normal movement afterwards
                else if (agent.confusionState == ConfusionState.Confused)
                {
                    agent.confusedMovementTime -= AI.gameTime.ElapsedGameTime.TotalMilliseconds;
                    a = -1;
                    if (agent.confusedMovementTime <= 0)
                    {
                        agent.confusionState = ConfusionState.temporarilyRight;
                        agent.confusedMovementTime = 1000 + AI.Random.Next(2000);
                    }
                }
                // ai is moving normaly for an amount of time, then switches to ConfusionState.None for calculating a new timespan of confusion
                else if (agent.confusionState == ConfusionState.temporarilyRight)
                {
                    a = 1;

                    agent.confusedMovementTime -= AI.gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (agent.confusedMovementTime <= 0)
                    {
                        agent.confusionState = ConfusionState.None;
                    }
                }


            }
            //inverted control not active, reset flag and timespan
            else
            {
                agent.confusionState = ConfusionState.None;
                agent.confusedMovementTime = -1;
            }
            return a;
        }


        /// <summary>
        /// let the player move to the bomb and hit it (NO check if the bomb is in the players half, should be done before calling this method!)
        /// Method only handles one bomb.
        /// </summary>
        /// <param name="a">factor for applying confusion (1 or -1)</param>
        /// <returns> true if a bomb exists and is chased, false if no bomb exists</returns>
        public bool chaseBomb(int a)
        {
            //search for a bomb in the Gameobjects
            ActiveObject bomb = (ActiveObject) SuperController.Instance.GameInstance.GameObjects.GetObjectByName("Bomb");
            if (bomb != null)
            {

                if ((Math.Abs(bomb.Body.Position.X - agent.ControlledPlayer.Body.Position.X) < 1) &&
                (agent.ControlledPlayer.Body.Position.Y - bomb.Body.Position.Y < 2.5f))
                {
                    // let player jump
                    jump();
                }

                //move towards the bomb
                if (agent.ControlledPlayer.Team == 1)
                {
                    if (agent.ControlledPlayer.Body.Position.Y - bomb.Body.Position.Y < 0)
                    {

                        InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2(bomb.Body.Position.X - bomb.Size.X - agent.ControlledPlayer.Body.Position.X, 0));

                    }
                    else
                    {
                        InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2(bomb.Body.Position.X - agent.ControlledPlayer.Body.Position.X, 0));
                        if (Math.Abs(bomb.Body.Position.X - agent.ControlledPlayer.Body.Position.X) < bomb.Size.X && (agent.ControlledPlayer.Body.Position.Y - bomb.Body.Position.Y < 1.5f))
                        {

                            InputManager.Instance.setHandAmplitude(agent.ControlledPlayer.PlayerIndex, bomb.Body.Position - agent.ControlledPlayer.HandBody.Position);

                        }
                    }

                }
                else
                {

                    if (agent.ControlledPlayer.Body.Position.Y - bomb.Body.Position.Y < 0 )
                    {

                        InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2(bomb.Body.Position.X + bomb.Size.X - agent.ControlledPlayer.Body.Position.X, 0));

                    }
                    else
                    {
                        InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * new Vector2(bomb.Body.Position.X - agent.ControlledPlayer.Body.Position.X, 0));
                        if (Math.Abs(bomb.Body.Position.X - agent.ControlledPlayer.Body.Position.X) < bomb.Size.X &&  (agent.ControlledPlayer.Body.Position.Y - bomb.Body.Position.Y < 1.5f))
                        {

                            InputManager.Instance.setHandAmplitude(agent.ControlledPlayer.PlayerIndex, bomb.Body.Position - agent.ControlledPlayer.HandBody.Position);

                        }
                    }

                }


                                
                return true;

            }else{
                //no bomb in level
                return false;

            }
        }


        /// <summary>
        /// Handles movement of a AI player (running to estimated ball position)
        /// </summary>
        public virtual void handleMovement()
        {

            float estBallPosition = estimatedBallXPosition(observedBall, (float)SettingsManager.Instance.get("playerSize") * 2);

            //used for confused movement; should be 1 or -1 (-1 for movement in wrong direction)
            int a = 1;
            a = confusionHandling();
            List<PowerUp> powerUps = SuperController.Instance.getAllPowerups();


            //let the player run towards the estimated ball position (left team)
            if (agent.ControlledPlayer.Team == 1)
            {
                if (estBallPosition <= levelSize.X * 0.5 + observedBall.Size.X)
                {
                    // offset player.Size.X / 4 is used to move the player in a good position for hitting the ball...
                    InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a *  new Vector2((estBallPosition - (agent.ControlledPlayer.Size.X / 4) - agent.ControlledPlayer.Body.Position.X), 0));
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
                                if (p.Body.Position.X < levelSize.X * 0.5 + p.Size.X)
                                {
                                  
                                    InputManager.Instance.setMovement( agent.ControlledPlayer.PlayerIndex, a *  new Vector2(p.Body.Position.X - agent.ControlledPlayer.Body.Position.X, 0));
                                    break;
                                }
                            }
                        }
                        else
                        { 
                            InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a *  getSmoothRandomMovementVec(levelSize.X * 0.75f, agent.ControlledPlayer.Body.Position.X));

                        }
                    }




                }
            }
                //right team
            else if (agent.ControlledPlayer.Team == 2)
            {

                //if the ball will land in the players side try to hit it, otherwise look for powerups or idle
                if (estBallPosition >= levelSize.X * 0.5 - observedBall.Size.X)
                {
                   
                    
                    // offset player.Size.X / 4 is used to move the player in a good position for hitting the ball...
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
                            InputManager.Instance.setMovement(agent.ControlledPlayer.PlayerIndex, a * getSmoothRandomMovementVec(levelSize.X * 0.75f, agent.ControlledPlayer.Body.Position.X));

                        }
                    }

                }
            }
        }



        /// <summary>
        /// Handles hand movement of each player
        /// </summary>
        /// <param name="player">Player that should be controlled</param>
        /// <param name="ball">Ball to which the player should react</param>
        public void handleHand()
        {

            // if the ball is above the player try to hit it from the right side
            if ((Math.Abs(observedBall.Body.Position.X - agent.ControlledPlayer.Body.Position.X) < 2) &&
                (agent.ControlledPlayer.Body.Position.Y - observedBall.Body.Position.Y < 1.5f))
            {

                // if the player is in the left half of the playing field,
                // try to hit the ball from the left side (not yet implemented)
                // currently handled via movement
                if (agent.ControlledPlayer.Body.Position.X < (levelSize.X / 2))
                {
                    // move the hand
                    hitBall();
                }
                // the opposite if the player is on the right
                else
                {
                    // move the hand
                    hitBall();
                }

            }
            // Reset hand position if it shouldn't be used
            else
            {
                //InputManager.Instance.setHandAmplitude(agent.ControlledPlayer.PlayerIndex, Vector2.Zero);
            }

        }

        /// <summary>
        /// Handles jumps of each player
        /// </summary>
        /// <param name="player">Player that should be controlled</param>
        /// <param name="ball">Ball to which the player should react</param>
        public void handleJump()
        {

            // if the ball is over the player (less then 3 meters)
            if ((Math.Abs(observedBall.Body.Position.X - agent.ControlledPlayer.Body.Position.X) < 1) &&
                (agent.ControlledPlayer.Body.Position.Y - observedBall.Body.Position.Y < 2.5f))
            {
                // let player jump
                jump();
            }
            else if (observedBall.Body.LinearVelocity.X == 0)
            {
                jump();
            }

        }


       /// <summary>
       /// 
       /// calculate a random movement vector which causes "smooth" moving
       /// 
       /// </summary>
       /// <returns>A vector which represents the new moving direction</returns>
        public Vector2 getSmoothRandomMovementVec()
        {
            float randomJitter = 0.1f;
            Vector2 newdirection =  lastMovementVec + new Vector2(((float)AI.Random.NextDouble() - 0.5f) * randomJitter, ((float)AI.Random.NextDouble() - 0.5f) * randomJitter);
            newdirection.Normalize();
            lastMovementVec = newdirection;
            newdirection.Y = 0f;
            return newdirection;

        }


        /// <summary>
        /// 
        /// calculate a random movement vector which causes "smooth" moving around a desired Position
        /// 
        /// </summary>
        /// <returns>A vector which represents the new moving direction</returns>
        public Vector2 getSmoothRandomMovementVec(float DesiredXPos, float currentXPos)
        {
            float randomJitter = 0.1f;
            Vector2 newdirection = lastMovementVec + new Vector2(((float)AI.Random.NextDouble() - 0.5f) * randomJitter, ((float)AI.Random.NextDouble() - 0.5f) * randomJitter);
            if (newdirection.X < 0 && currentXPos - DesiredXPos < 1)
            {

                newdirection.X /= (currentXPos - DesiredXPos) * (currentXPos - DesiredXPos);


            }
            else if (newdirection.X > 0 && currentXPos - DesiredXPos > 1)
            {


                newdirection.X /= (currentXPos - DesiredXPos) * (currentXPos - DesiredXPos);


            }
            newdirection.Normalize();
            lastMovementVec = newdirection;
            newdirection.Y = 0f;

            
            

            return newdirection;

        }

        #endregion

   }
}
