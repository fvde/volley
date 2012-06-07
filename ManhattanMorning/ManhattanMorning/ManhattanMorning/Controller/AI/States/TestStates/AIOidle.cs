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
    class AIOidle : aState<Agent>
    {
       

        public  AIOidle(Agent agent,Vector2 levelSize)
        {
            this.agent = agent;
            this.levelSize = levelSize;
        }

        public override void enter()
        {
            throw new NotImplementedException();
        }

        public override void execute( LayerList<Ball> ballList)
        {
            throw new NotImplementedException();
        }

        public override void exit()
        {
            throw new NotImplementedException();
        }
    }
}
