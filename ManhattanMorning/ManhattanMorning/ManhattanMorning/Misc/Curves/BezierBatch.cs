using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace ManhattanMorning.Misc.Curves
{
    class BezierBatch : Path
    {
        #region Variables 
        private List<Bezier> bezierList;
        private List<Vector2> vectorList;

        private Manipulator manipulator;
    
        /// <summary>
        /// The Manipulator to interactively change the points
        /// </summary>
        public override Manipulator Manipulator
        {
            get { return this.manipulator; }
        }
        #endregion

        #region Init
        public BezierBatch(List<Vector2> vectorList)
        {
            
            this.vectorList = vectorList;
            bezierList = new List<Bezier>();
            #if DEBUG
            this.manipulator = new BezierManipulator(this);
            #endif
            this.update();
        }

        public BezierBatch(List<Bezier> bList)
        {
            bezierList = new List<Bezier>();
            this.bezierList = bList;

            vectorList = new List<Vector2>();

            vectorList.Add(bList.ElementAt<Bezier>(0).P0);
            foreach (Bezier b in bList)
            {
                vectorList.Add(b.P1);
                vectorList.Add(b.P2);
                vectorList.Add(b.P3);
            }


            foreach(Vector2 v in this.getPoints())
            {
                Console.WriteLine(v);
            }
        }

        #endregion

        #region Methods
        public List<Bezier> getCurves()
        {
            return this.bezierList;
        }

        public List<Vector2> getPoints()
        {
            return this.vectorList;
        }

        public void update()
        {
            bezierList = new List<Bezier>();

            for (int i = 0; i < this.vectorList.Count-3; i+=3)
            {
                bezierList.Add(new Bezier(this.vectorList.ElementAt(i), this.vectorList.ElementAt(i + 1), this.vectorList.ElementAt(i + 2), this.vectorList.ElementAt(i + 3)));
            }

        }

        /// <summary>
        /// Returns the position of a point in the bezierbatch for a given t.
        /// t = 0 is the first point of the batch
        /// t = 1 is the last point of the batch
        /// </summary>
        /// <param name="t">position in curve between 0 and 1</param>
        /// <returns></returns>
        public override Vector2 evaluate(float t)
        {
            if (t >= 1.0f) t = 0.999999f;

            Vector2 pos = new Vector2();

            int bezierNumber = (int)(t * bezierList.Count);
            float newT = (t * bezierList.Count) - bezierNumber;

            pos = bezierList[bezierNumber].evaluate(newT);

            return pos;
        }
        


        /// <summary>
        /// Get the direction Vector at the given Point
        /// </summary>
        /// <param name="t">the point to evaluate between 0 and 1</param>
        /// <returns></returns>
        public override Vector2 getDirectionAtValue(float t)
        {
            Vector2 dir = new Vector2();
            dir = this.evaluate(t + 0.001f) - this.evaluate(t);

            return Vector2.Normalize(dir);
        }
        #endregion

    }
}
