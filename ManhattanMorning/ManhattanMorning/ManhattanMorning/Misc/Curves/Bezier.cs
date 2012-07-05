using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ManhattanMorning.Misc.Curves
{
    /// <summary>
    /// Class to represent a bezier Curve
    /// </summary>
    class Bezier
    {
        //Members
        private Vector2 p0;
        private Vector2 p1;
        private Vector2 p2;
        private Vector2 p3;

        //Properties
        //The 4 Vectors that define a cubic bezier curve
        public Vector2 P0 { get { return this.p0; } set { this.p0 = value; } }
        public Vector2 P1 { get { return this.p1; } set { this.p1 = value; } }
        public Vector2 P2 { get { return this.p2; } set { this.p2 = value; } }
        public Vector2 P3 { get { return this.p3; } set { this.p3 = value; } }

        /// <summary>
        /// Create a new Bezier Curve
        /// </summary>
        /// <param name="p0">start point</param>
        /// <param name="p1">control point 1</param>
        /// <param name="p2">control point 2</param>
        /// <param name="p3">end point</param>
        public Bezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        /// <summary>
        /// Evaluate the Bezier equation at a certain point between 0 and 1
        /// 0: start
        /// 1: end
        /// </summary>
        /// <param name="t">point, where to evaluate</param>
        /// <returns></returns>
        public Vector2 evaluate(float t)
        {
            Vector2 point = new Vector2();

            //The Cubic bezier formula
            point = ((1.0f - t) * (1.0f - t) * (1.0f - t) * this.p0) +
                    (3.0f*t*(1.0f-t)*(1.0f-t)*this.p1) +
                    (3.0f*t*t*(1-t)*this.p2)+
                    (t*t*t*this.p3);
           
            return point;
        }


    }
}
