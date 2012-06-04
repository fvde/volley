using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ManhattanMorning.Misc;
using ManhattanMorning.Controller;
using ManhattanMorning.Model.GameObject;

namespace ManhattanMorning.Controller.AI.States.AIOne
{
    class AIOblock : aState<Agent>
    {




        public  AIOblock(Vector2 levelSize)
        {
            this.levelSize = levelSize;
        }

        public override void enter(Agent e)
        {
            throw new NotImplementedException();
        }

        public override void execute(Agent e, LayerList<Ball> ballList)
        {
            throw new NotImplementedException();
        }

        public override void exit(Agent e)
        {
            throw new NotImplementedException();
        }
    }
}
