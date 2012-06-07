using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ManhattanMorning.Misc.Curves
{
    class Curve2DManipulator : Manipulator
    {

        Curve2D curve;
        private int selectedNode;

        private bool isActive = false;
    
        /// <summary>
        /// To see if the manipulator is active at the moment
        /// </summary>
        public override bool IsActive
        {
            get { return this.isActive; }
        }

        public Curve2DManipulator(Curve2D curve)
        {
            this.curve = curve;
            selectedNode = -1;          
        }

        /// <summary>
        /// Handles the mouse input and moves points when necessary
        /// </summary>
        /// <param name="mouseState">the current updated mouse state</param>
        override public void handleInteraction(MouseState mouseState)
        {

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (selectedNode==-1)
                {
                    foreach (Vector2 v in this.curve.getPoints())
                    {
                        if (Math.Abs(v.X*100f - mouseState.X) <= 20 && Math.Abs(v.Y*100f - mouseState.Y) <= 20)
                        {
                            selectedNode = curve.getPoints().IndexOf(v);
                            isActive = true;
                        }

                    }
                }
            }
            else
            {
                if (selectedNode != -1)
                {
                    Console.WriteLine("ID: " + selectedNode +  "   New Position: X:" + mouseState.X*0.01f + "  Y:" + mouseState.Y*0.01f);
                }
                
                selectedNode = -1;
                isActive = false;
            }

            if (selectedNode >= 0)
            {
                mouseState = Mouse.GetState();
                curve.changeCurvePoint(selectedNode, new Vector2(mouseState.X*0.01f, mouseState.Y*0.01f));
            }

        }

        public bool pointSelected()
        {
            if (selectedNode == -1) return false;
            else return true;
        }


    }
}
