using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ManhattanMorning.Misc.Curves
{

    class BezierManipulator : Manipulator
    {
        
        BezierBatch bezierBatch;
        private int selectedNode = -1;
        private bool isActive = false;


        /// <summary>
        /// To see if the manipulator is active at the moment
        /// </summary>
        public override bool IsActive
        {
            get { return this.isActive; }
        }

        public BezierManipulator(BezierBatch bezierBatch)
        {
            this.bezierBatch = bezierBatch;
            
        }


        /// <summary>
        /// Handles the mouse input and moves points when necessary
        /// </summary>
        /// <param name="mouseState">the current updated mouse state</param>
        override public void handleInteraction(MouseState mouseState)
        {
            
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                
                foreach (Vector2 v in this.bezierBatch.getPoints())
                {
                    if (selectedNode == -1)
                    {
                        if (Math.Abs(v.X*100f - mouseState.X) <= 20 && Math.Abs(v.Y*100f - mouseState.Y) <= 20)
                        {
                            this.isActive = true;
                            selectedNode = bezierBatch.getPoints().IndexOf(v);
                        }
                    }

                }
            }
            else
            {
                if (selectedNode != -1)
                {
                    Console.WriteLine("ID: " + selectedNode + "   New Position: X:" + mouseState.X*0.01f + "  Y:" + mouseState.Y*0.01f);
                }
                selectedNode = -1;
                this.isActive = false;
            }
            if (selectedNode >= 0)
            {
                mouseState = Mouse.GetState();
                Vector2  tmp = this.bezierBatch.getPoints().ElementAt(selectedNode);
                tmp.X = mouseState.X*0.01f;
                tmp.Y = mouseState.Y*0.01f;
                this.bezierBatch.getPoints()[selectedNode] = tmp;
                this.bezierBatch.update();

            }
        }


        public bool pointSelected()
        {
            if (selectedNode == -1) return false;
            else return true;
        }

    }
}
