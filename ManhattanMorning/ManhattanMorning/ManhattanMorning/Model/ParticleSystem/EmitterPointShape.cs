using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Model.ParticleSystem
{
    /// <summary>
    /// A Single Point with no dimensions
    /// </summary>
    class EmitterPointShape : EmitterShape
    {

        public EmitterPointShape()
        {
        }

         /// <summary>
        ///Calculates a Random Position within the Shape relative to the center. (0f,0f) is Center
        /// </summary>
        /// <returns>Position as Vector2</returns>
        public override Vector2 randomPositionWithinShape()
        {
            //Always return (0,0) because it's a point
            return new Vector2(0f, 0f);
        }
    }
}
