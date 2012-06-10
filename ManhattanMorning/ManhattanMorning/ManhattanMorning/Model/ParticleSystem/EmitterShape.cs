using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ManhattanMorning.Model.ParticleSystem
{
    /// <summary>
    /// Possible Shapes of an Emitter are derived from this class
    /// </summary>
    public abstract class EmitterShape
    {
        abstract public Vector2 randomPositionWithinShape();
    }
}
