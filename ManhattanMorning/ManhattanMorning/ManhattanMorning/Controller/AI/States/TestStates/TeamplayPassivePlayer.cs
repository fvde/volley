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
    /// represents the logic of the ai player in a team which is currently not trying to hit the ball
    /// </summary>
    class TeamplayPassivePlayer : aState<Agent>
    {




        public  TeamplayPassivePlayer(Agent agent, Vector2 levelSize)
        {
            this.levelSize = levelSize;
            this.agent = agent;
        }

        public override void enter()
        {
            Logger.Instance.log(Sender.AI,"player "+ agent.ControlledPlayer.PlayerIndex + "entering state: TeamplayPassivePlayer",PriorityLevel.Priority_5);
        }

        public override void execute(LayerList<Ball> ballList)
        {
            
        }

        public override void exit()
        {
            Logger.Instance.log(Sender.AI, "player " + agent.ControlledPlayer.PlayerIndex + " exit state: TeamplayPassivePlayer", PriorityLevel.Priority_5);
        }
    }
}
