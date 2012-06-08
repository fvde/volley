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
    class MasterAImadeByBoss : aState<Agent>
    {




       public override void enter()
        {
            Logger.Instance.log(Sender.AI, "Hey, i'm the master-ai designed by our boss \n "
                + "                                 if yout beat me, he will fire you!!! \n", PriorityLevel.Priority_5);
        }

       public override void execute( LayerList<Ball> ballList)
        {
            // if there is no ball, do nothin
            if (ballList.Count == 0)
                return;

            // (Maybe there is another one because of the powerup -> must be handled by KI somehow)
            observedBall = ballList[0];


            handleMovement();
            handleHand( );
            handleJump();
           // throw new NotImplementedException();

        }

        public override void exit()
        {
            Logger.Instance.log(Sender.AI, "Hey, I won!!", PriorityLevel.Priority_5);
            //throw new NotImplementedException();
        }



     

        public  MasterAImadeByBoss(Agent agent,Vector2 levelSize)
        {
            this.agent = agent;
            this.levelSize = levelSize;
        }
    }
}
