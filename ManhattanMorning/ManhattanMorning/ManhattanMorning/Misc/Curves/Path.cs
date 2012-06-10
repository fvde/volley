using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;



namespace ManhattanMorning.Misc.Curves
{
    /// <summary>
    /// Defines a Path for a PathAnimation
    /// </summary>
    public abstract class Path
    {
        abstract public Manipulator Manipulator { get; }
        abstract public Vector2 evaluate(float t);
        abstract public Vector2 getDirectionAtValue(float t);
    }
}
