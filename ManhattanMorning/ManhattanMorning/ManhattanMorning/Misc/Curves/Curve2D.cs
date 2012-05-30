using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Misc.Curves
{
    public class Curve2D : Path
    {
        private Curve curveX;
        private Curve curveY;

        private  Manipulator manipulator;

        /// <summary>
        /// The Manipulator to interactively change the points
        /// </summary>
        public override Manipulator Manipulator
        {
            get { return this.manipulator; }
        }

        public Curve2D()
        {
            this.curveX = new Curve();
            this.curveY = new Curve();
            #if DEBUG
            this.manipulator = new Curve2DManipulator(this);
            #endif
        }

        /// <summary>
        /// Adds a point to the curve
        /// </summary>
        /// <param name="time">Time Key between 0 and 1!!</param>
        /// <param name="point">Position of the point</param>
        public void addPoint(float time, Vector2 point)
        {
            this.curveX.Keys.Add(new CurveKey(time, point.X));
            this.curveY.Keys.Add(new CurveKey(time, point.Y));
        }

        /// <summary>
        /// Return the position of a point in the curve
        /// </summary>
        /// <param name="time">point to avaluate</param>
        /// <returns></returns>
        public override Vector2 evaluate(float time)
        {
            Vector2 point = new Vector2();
            point.X = curveX.Evaluate(time);
            point.Y = curveY.Evaluate(time);
            return point;
        }

        /// <summary>
        /// Get List of all the points in the Curve
        /// </summary>
        /// <returns>a list of all points</returns>
        public List<Vector2> getPoints()
        {
            List<Vector2> list = new List<Vector2>();

            for (int i = 0; i < curveX.Keys.Count; i++)
            {
                Vector2 v = new Vector2(curveX.Keys[i].Value,curveY.Keys[i].Value);
                list.Add(v);
            }
            return list;
        }



        public void SetTangents()
        {
            CurveKey prev;
            CurveKey current;
            CurveKey next;
            int prevIndex;
            int nextIndex;
            for (int i = 0; i < curveX.Keys.Count; i++)
            {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = i;

                nextIndex = i + 1;
                if (nextIndex == curveX.Keys.Count) nextIndex = i;

                prev = curveX.Keys[prevIndex];
                next = curveX.Keys[nextIndex];
                current = curveX.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveX.Keys[i] = current;

            }
            for (int i = 0; i < curveY.Keys.Count; i++)
            {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = i;

                nextIndex = i + 1;
                if (nextIndex == curveX.Keys.Count) nextIndex = i;

                prev = curveY.Keys[prevIndex];
                next = curveY.Keys[nextIndex];
                current = curveY.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveY.Keys[i] = current;

            }
        }

        static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur, ref CurveKey next)
        {
            float dt = next.Position - prev.Position;
            float dv = next.Value - prev.Value;
            if (Math.Abs(dv) < float.Epsilon)
            {
                cur.TangentIn = 0;
                cur.TangentOut = 0;
            }
            else
            {
                // The in and out tangents should be equal to the slope between the adjacent keys.
                cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
                cur.TangentOut = dv * (next.Position - cur.Position) / dt;
            }
        }

        /// <summary>
        /// Change a Point within the Curve
        /// </summary>
        /// <param name="id">id of the point</param>
        /// <param name="vec">New Position</param>
        public void changeCurvePoint(int id, Vector2 vec)
        {
            this.curveX.Keys[id].Value = vec.X;
            this.curveY.Keys[id].Value = vec.Y;
            this.SetTangents();
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


    }
}
